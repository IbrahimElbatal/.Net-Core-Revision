using Asp.net_Core_Revsion.Models;
using Asp.net_Core_Revsion.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Asp.net_Core_Revsion.Controllers
{
    //    [Authorize(Roles = "Admin")]
    public class AdministratorController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdministratorController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Roles()
        {
            return View(_context.Roles.ToList());
        }

        [HttpGet]
        [Authorize(Policy = "CreatePolicy")]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "CreatePolicy")]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var role = new IdentityRole()
            {
                Name = model.Name
            };

            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
                return RedirectToAction("Roles");

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "EditPolicy")]
        public IActionResult EditRole(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
                return BadRequest();

            var role = _context.Roles
                .FirstOrDefault(r => r.Id == roleId);
            if (role == null)
                return NotFound();

            var usersInRole = _context.UserRoles
                .Where(ur => ur.RoleId == roleId)
                .SelectMany(ur => _userManager.Users.Where(u => u.Id == ur.UserId));

            var model = new EditRoleViewModel()
            {
                Id = role.Id,
                Name = role.Name,
                Users = usersInRole.Select(u => u.Email).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "EditPolicy")]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = _userManager.GetUserId(User);
            var usersInRole = _context.UserRoles
                .Where(ur => ur.RoleId == model.Id && ur.UserId == userId)
                .SelectMany(ur => _userManager.Users.Where(u => u.Id == ur.UserId));

            model.Users = usersInRole.Select(u => u.Email);

            var role = await _roleManager.FindByIdAsync(model.Id);
            role.Name = model.Name;

            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
                return RedirectToAction("Roles");

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "DeletePolicy")]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
                return BadRequest();

            var role = _context.Roles
                .FirstOrDefault(r => r.Id == roleId);
            if (role == null)
                return NotFound();

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
                return RedirectToAction("Roles");

            return BadRequest("Unable to remove the role");

        }

        [HttpGet]
        [Authorize(Policy = "DeletePolicy")]
        public async Task<IActionResult> AddUsersToRole(string roleId)
        {
            ViewBag.RoleId = roleId;
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return NotFound("Role Not Found");

            var model = _userManager.Users
                    .ToList()
                    .Select(u => new UsersInRoleViewModel()
                    {
                        UserId = u.Id,
                        Email = u.Email,
                        IsSelected = _userManager.IsInRoleAsync(u, role.Name).GetAwaiter().GetResult()
                    }).ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddUsersToRole(List<UsersInRoleViewModel> model, string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            foreach (var user in _userManager.Users.ToList())
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
            }

            foreach (var userInRole in model)
            {
                if (userInRole.IsSelected)
                {
                    var user = await _userManager.FindByIdAsync(userInRole.UserId);
                    var result = await _userManager.AddToRoleAsync(user, role.Name);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                            return BadRequest("Can't add users to the role");
                    }
                }
            }
            return RedirectToAction("EditRole", new { roleId = roleId });
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            return View(await _userManager.Users.ToListAsync());
        }

        [HttpGet]
        //        [Authorize(Policy = "EditOtherAdminPolicy")]
        public async Task<IActionResult> EditUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("invalid request");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("the user can't be found");

            var roles = _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .SelectMany(ur => _roleManager.Roles.Where(r => r.Id == ur.RoleId))
                .Select(r => r.Name);

            var claims = _context.UserClaims
                .Where(uc => uc.UserId == userId)
                .Select(uc => uc.ClaimValue);

            var model = new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Roles = roles,
                Claims = claims

            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "EditOtherAdminPolicy")]

        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                return NotFound("the user can't be found");

            user.Email = model.Email;
            user.UserName = model.UserName;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return RedirectToAction("Users");
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Bad Request");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound($"the user with {userId} is not found");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    return BadRequest(error.Description);
            }

            return RedirectToAction("Users");

        }

        [HttpGet]
        public async Task<IActionResult> AddRolesToUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest();

            ViewBag.UserId = userId;

            var user = await _userManager.FindByIdAsync(userId);

            var model = _roleManager.Roles.ToList()
                 .Select(r => new UserRolesViewModel()
                 {
                     RoleId = r.Id,
                     Name = r.Name,
                     IsSelected = _userManager.IsInRoleAsync(user, r.Name).GetAwaiter().GetResult()
                 });
            return View(model.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRolesToUser(string userId,
            List<UserRolesViewModel> model)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("the user you want to change data isn't found");

            foreach (var role in _roleManager.Roles.ToList())
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    var result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                            return BadRequest(error.Description);
                    }

                }
            }

            foreach (var role in model)
            {
                if (role.IsSelected)
                {
                    var result = await _userManager.AddToRoleAsync(user, role.Name);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                            return BadRequest(error.Description);
                    }
                }
            }

            await _signInManager.RefreshSignInAsync(user);

            return RedirectToAction("EditUser", new { userId = userId });

        }

        [HttpGet]
        public async Task<IActionResult> AddClaimsToUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest();

            var user = await _userManager.FindByIdAsync(userId);

            var userClaims = await _userManager.GetClaimsAsync(user);

            var model = new UserClaimsViewModel() { UserId = userId };

            foreach (var claim in ClaimsStore.AllClaims)
            {
                var userClaim = new UserClaim() { ClaimType = claim.Type };
                if (userClaims != null && userClaims.Any(uc => uc.Type == claim.Type))
                    userClaim.IsSelected = true;
                else
                    userClaim.IsSelected = false;

                model.UserClaims.Add(userClaim);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddClaimsToUser(UserClaimsViewModel model)
        {
            if (string.IsNullOrEmpty(model.UserId))
                return BadRequest();

            var user = await _userManager.FindByIdAsync(model.UserId);

            var userClaims = await _userManager.GetClaimsAsync(user);

            var result = await _userManager.RemoveClaimsAsync(user, userClaims);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            foreach (var claim in model.UserClaims)
            {
                if (claim.IsSelected)
                {
                    var resultClaim = await _userManager.AddClaimAsync(user,
                        new Claim(claim.ClaimType, claim.ClaimType));
                    if (!resultClaim.Succeeded)
                    {
                        foreach (var error in resultClaim.Errors)
                        {
                            ModelState.AddModelError(String.Empty, error.Description);
                        }

                        return View(model);
                    }
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            return RedirectToAction("EditUser", new { userId = model.UserId });
        }

    }
}