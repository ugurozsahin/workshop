using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Data.Initializer;
using Core.Cache;
using Core.Constants;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);

            Initialize(host);

            host.Run();
        }

        private static void Initialize(IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    //Because MySQL Container Init
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(30));
                    var databaseInitializer = services.GetRequiredService<DatabaseInitializer>();
                    databaseInitializer.Initialize();

                    var cacher = services.GetRequiredService<ICacher>();
                    cacher.RemoveAsync(GlobalConstants.CACHE_KEY_MAX_LAST_UPDATED_TIME)
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();

                    var cacheInitializer = services.GetRequiredService<CacheInitializer>();
                    cacheInitializer.Initialize();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while initializing.");
                }
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
