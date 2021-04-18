using Extend.Models;
using Extend.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace Extend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection"));
                });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));
            services.AddMemoryCache();
            services.AddSession();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddMvc().AddMvcOptions(setup =>
            {
                setup.FormatterMappings
                    .SetMediaTypeMappingForFormat("xml",
                        new MediaTypeHeaderValue("application/xml"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "RouteWithCategory",
                    template: "{controller=Product}/{action=index}/{category?}/{page?}",
                    defaults: new { },
                    constraints: new
                    {
                        //                        Id = new CompositeRouteConstraint(new List<IRouteConstraint>()
                        //                        {
                        //                            new AlphaRouteConstraint(),
                        //                            new IntRouteConstraint()
                        //                        })
                    });
                routes.MapRoute(
                    name: "Default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Product", action = "Index" });
            });
        }
    }
}
