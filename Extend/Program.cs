using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Reflection;

namespace Extend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            //           return WebHost.CreateDefaultBuilder(args)
            //            .UseStartup<Startup>();

            return new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((context, config) =>
                {
                    var env = context.HostingEnvironment;
                    config
                        .AddJsonFile("appsettings.json", true, true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);

                    if (env.IsDevelopment())
                    {
                        var assembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                        config.AddUserSecrets(assembly);
                    }

                    config.AddEnvironmentVariables();
                    config.AddCommandLine(args);
                })
                .ConfigureLogging((context, logger) =>
                {
                    logger.AddConfiguration(context.Configuration.GetSection("Logging"));
                    logger.AddConsole();
                    logger.AddDebug();
                })
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseStartup(nameof(Extend));
        }
    }
}
