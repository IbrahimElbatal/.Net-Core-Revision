using Asp.net_Core_Revsion.Models;
using Asp.net_Core_Revsion.Repositories;
using Asp.net_Core_Revsion.Utilities;
using Asp.netCoreRevsion.Repositories;
using Asp.netCoreRevsion.Utilities.Policies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;

namespace Asp.net_Core_Revsion
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSession(options =>
            {
                options.IOTimeout = TimeSpan.FromHours(2);
                options.IdleTimeout = TimeSpan.FromHours(2);
                options.Cookie.Name = ".Net Core Session";
                options.Cookie.HttpOnly = false;
            });
            //lifetime of token provider
            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(5); //default is one day
            });
            //lifetime of custom token provider
            services.Configure<CustomEmailConfirmationTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(3); //default is one day
            });

            services.AddAuthentication()
                .AddGoogle(options =>
                    {

                        options.ClientId = "389027786704-eqpmecn5bbp02u6dqpd61nckk7s8chmd.apps.googleusercontent.com";
                        options.ClientSecret = "FTDJAmT61aHlJCh4B5TtvV0L";
                        options.UserInformationEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo";
                        options.ClaimActions.Clear();
                        options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                        options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                        options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                        options.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given_name");
                        options.ClaimActions.MapJsonKey(ClaimTypes.Surname, "family_name");
                        options.ClaimActions.MapJsonKey("urn:google:profile", "link");
                    })
                .AddFacebook(options =>
                {
                    options.AppId = "1403536779843653";
                    options.AppSecret = "2ec3310d8cffd80a7022476185f2c6e6";
                });

            services.AddAuthorization(options =>
            {
                //not call other handlers when one of the handler fail
                options.InvokeHandlersAfterFailure = false;
                //custom handler and requirement
                options.AddPolicy("EditOtherAdminPolicy", policy =>
                    {
                        policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement());
                    });

                options.AddPolicy("FuncCustomPolicy", policy =>
                    {
                        //using func another way is to create custom Policy
                        policy.RequireAssertion(context =>
                        {
                            return context.User.IsInRole("Admin") &&
                                   context.User.HasClaim(claim => claim.Type == "Edit Role") ||
                                   context.User.IsInRole("Super Admin");
                        });
                    });
                options.AddPolicy("CreatePolicy", policy =>
                {
                    policy.RequireClaim("Create Role");
                });
                options.AddPolicy("EditPolicy", policy =>
                {
                    policy.RequireClaim("Edit Role");
                });
                options.AddPolicy("DeletePolicy", policy =>
                {
                    policy.RequireClaim("Delete Role");
                });
            });
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Account/AccessDenied");
                options.LoginPath = new PathString("/Account/Login");
                options.LogoutPath = new PathString("/Account/Logout");
                options.Cookie.Name = "Default";
            });

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    #region password
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 6;
                    options.Password.RequiredUniqueChars = 0;

                    #endregion

                    //register custom provider
                    options.Tokens.ChangeEmailTokenProvider = "CustomEmailConfirmation";
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                // add custom provider and take the name 
                // and override default in add identity tokens option
                .AddTokenProvider<CustomEmailConfirmationTokenProvider
                    <ApplicationUser>>("CustomEmailConfirmation");

            services.AddTransient<IFileManager, FileManager>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            //custom handler that i created or multiple handler
            services.AddSingleton<IAuthorizationHandler
                , CanEditOnlyOtherAdminRolesAndClaimsHandler>();
            services.AddSingleton<IAuthorizationHandler
                , SuperAdminHandler>();
            //            services.AddTransient<IStartupFilter, CustomMiddleWareStartUpFilter>();
            //                        services.AddTransient<CustomMiddlewareUsingIMiddleWareInterface>();

            services.AddMvc(configure =>
            {
                var authorizePolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                configure.Filters.Add(new AuthorizeFilter(authorizePolicy));
            })
                .AddJsonOptions(o =>
                {
                    o.SerializerSettings.ContractResolver =
                        new CamelCasePropertyNamesContractResolver();
                    o.SerializerSettings.Formatting = Formatting.Indented;
                    o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;

                });

            services.AddTransient<IAuthorizationHandler, CustomPolicyHandler>();
            services.AddTransient<IAuthorizationPolicyProvider, CustomPolicyProvider>();
        }

        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env
         )
        {
            using (var iisReader = File.OpenText("iisRewrite.xml"))
            {
                var options = new RewriteOptions();
                options
                    .AddRedirectToHttps(301, 5001)
                    .AddRedirect("redirect-rule/(.*)", "redirected/$1")
                    .AddRewrite(@"^rewrite-rule/(\d+)/(\d+)",
                        "rewritten/var1=$1&var2=$2", true)
                    .AddIISUrlRewrite(iisReader)
                    .Add(context =>
                    {
                        var path = context.HttpContext.Request.Path;
                        if (path.StartsWithSegments(new PathString("/xmlFiles")))
                            return;
                        if (path.Value.EndsWith(".xml"))
                        {
                            var response = context.HttpContext.Response;
                            response.Headers[HeaderNames.Location] =
                                "/xmlFiles" + context.HttpContext.Request.Path +
                                context.HttpContext.Request.QueryString;
                        }
                    });
                app.UseRewriter(options);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //                app.UseExceptionHandler(new ExceptionHandlerOptions()
                //                {
                //                    ExceptionHandler = context =>
                //                        context.Response.WriteAsync("Error Handler In Production Environment")
                //                });

                app.UseExceptionHandler("/Error");
            }

            #region HandleStatusCode
            //handling status bages errors
            //            app.UseStatusCodePages(new StatusCodePagesOptions()
            //            {
            //                HandleAsync = context => context.HttpContext.Response.WriteAsync("Error")
            //            });

            //            app.UseStatusCodePagesWithRedirects("/Error/{0}");

            app.UseStatusCodePagesWithReExecute("/Error/{0}");
            #endregion

            //seed database

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider
                        .GetRequiredService<ApplicationDbContext>();

                if (!context.Employees.Any())
                {
                    context.Database.Migrate();

                    context.Employees.AddRange(new List<Employee>()
                    {
                        new Employee(){Name = "Ibrahim",DepartmentId = 1,Email = "ibrahim@gmail.com"},
                        new Employee(){Name = "Ali",DepartmentId = 2 ,Email = "ali@gmail.com"},
                        new Employee(){Name = "Ahmed",DepartmentId = 3,Email = "ahmed@gmail.com"},
                    });
                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }

            app.UseFileServer(new FileServerOptions()
            {
                //                EnableDirectoryBrowsing = true
            });

            //            app.UseMiddleware<CustomMiddleware>();
            //            app.UseMiddleware<CustomMiddlewareUsingIMiddleWareInterface>();
            //            app.UseMiddleware<CustomMiddleWareUsingFactoryInterface>();
            app.UseSession();
            app.UseAuthentication();
            app.UseMvc(routes => { routes.MapRoute(name: "Default", template: "{controller=Employee}/{action=Index}/{id?}"); });
            //            app.Run(async (context) =>
            //            {
            //                var path = context.Request.Path;
            //                var scheme = context.Request.Scheme;
            //                var queryString = context.Request.QueryString;
            //                await context.Response.WriteAsync(path + ' ' + queryString + ' ' + scheme);
            //            });
        }
    }
}
