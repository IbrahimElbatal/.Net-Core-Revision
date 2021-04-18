using Asp.net_Core_Revsion.Models;
using Asp.net_Core_Revsion.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Asp.net_Core_Revsion.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
                UserManager<ApplicationUser> userManager,
                SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        [HttpGet, HttpPost]
        public async Task<IActionResult> EmailExists(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Json(true);
            return Json($"{email} is already taken.");
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser()
            {
                Email = model.Email,
                UserName = model.Email
            };
            var result = await _userManager.CreateAsync(user, password: model.Password);
            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink =
                    Url.Action("ConfirmEmail", "Account", new
                    {
                        token = token,
                        userId = user.Id
                    }, Request.Scheme);

                return Redirect(confirmationLink);
                //                await _signInManager.SignInAsync(user, false);
                //                return RedirectToAction("Index", "Employee");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(String.Empty, error.Description);
                }

                return View(model);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var model = new LoginViewModel()
            {
                RememberMe = false,
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager
                        .GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            model.ExternalLogins =
                (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && !user.EmailConfirmed &&
                (await _userManager.CheckPasswordAsync(user, model.Password)))
            {
                ModelState.AddModelError(string.Empty, "Email isn't Confirmed");
                return View("Login", model);
            }

            var result = await _signInManager
                .PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.ReturnUrl))
                {
                    if (Url.IsLocalUrl(model.ReturnUrl))
                        return Redirect(model.ReturnUrl);
                    return RedirectToAction("Index", "Employee");
                }
                return RedirectToAction("Index", "Employee");

                //                return LocalRedirect(returnUrl);
            }
            ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Employee");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied(string returnUrl)
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUri = Url.Action("ExternalLoginCallback", "Account", new
            {
                returnUrl = returnUrl
            });

            var properties = _signInManager
                .ConfigureExternalAuthenticationProperties(provider, redirectUri);

            return new ChallengeResult(provider, properties);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            var loginViewModel = new LoginViewModel()
            {
                RememberMe = false,
                ReturnUrl = returnUrl,
                ExternalLogins =
                    (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, "Error From External Provider : " + remoteError);
                return View("Login", loginViewModel);
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                ModelState.
                    AddModelError(string.Empty, "Error Loading External Login Information");
                return View("Login", loginViewModel);
            }

            var result = await _signInManager
                .ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            ApplicationUser user = null;
            if (email != null)
            {
                user = await _userManager.FindByEmailAsync(email);
                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "Email isn't Confirmed");
                    return View("Login", loginViewModel);
                }
            }

            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                if (email != null)
                {
                    if (user == null)
                    {
                        user = new ApplicationUser()
                        {
                            Email = email,
                            UserName = email
                        };

                        await _userManager.CreateAsync(user);

                        var token = await _userManager.
                            GenerateEmailConfirmationTokenAsync(user);
                        var confirmationLink =
                            Url.Action("ConfirmEmail", "Account", new
                            {
                                token = token,
                                userId = user.Id
                            }, Request.Scheme);

                        return Redirect(confirmationLink);
                    }

                    await _userManager.AddLoginAsync(user, info);
                    await _signInManager.SignInAsync(user, false);

                    return LocalRedirect(returnUrl);
                }

                ViewBag.StatusCode = "400";
                ViewBag.Message = "Email Claim Can't be found in externalLogin information";
                return View("Error");
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null | token == null)
                return RedirectToAction("Index", "Employee");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.StatusCode = "404";
                ViewBag.Message = "Can't find the user";
                return View("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Email is confirmed Please login again");
                return RedirectToAction("Login");
            }
            ViewBag.StatusCode = "400";
            ViewBag.Message = "Some thing wrong happens";
            return View("Error");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPassword(ForegetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return NotFound();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var callbackUrl = Url.Action("ResetPassword", "Account", new
            {
                email = model.Email,
                token = token
            });

            ViewBag.CallbackUrl = callbackUrl;
            return View("ForgetPasswordConfirmation");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string email, string token)
        {
            var model = new ResetPasswordViewModel()
            {
                Email = email,
                Token = token
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return NotFound();

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
                return View("ResetPasswordConfirmation");

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

    }
}