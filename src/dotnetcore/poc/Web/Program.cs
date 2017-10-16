using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Data.Context;
using Data.Initializer;
using Web.Core.Config;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var providers = services.GetRequiredService<Providers>();
                    if (providers.ContentProvider == "MySql"
                        || providers.ContentProvider == "Sqlite"
                        || providers.ContentProvider == "MsSql"
                        || providers.ContentProvider == "InMemory")
                    {
                        var context = services.GetRequiredService<ContentContext>();
                        DbInitializer.Initialize(context);
                    }
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
