using ConsumerApp.Core.Config;
using Core.Config;
using Data.RabbitMq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;

namespace ConsumerApp
{
    class Program
    {
        public static IConfigurationRoot Configuration;
        static void Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");

            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("./appsettings.json", optional: true);
            if (environment == "Development")
            {
                builder
                    .AddJsonFile(
                        $"./appsettings.{environment}.json",
                        optional: true
                    );
            }
            else
            {
                builder
                    .AddJsonFile($"./appsettings.{environment}.json", optional: false);
            }

            Configuration = builder.Build();


            var connectionStrings = new ConnectionStrings
            {
                RabbitMq = "amqp://demo:demo@poc-rabbitmq:5672/my_vhost"
            };
            //Configuration.GetSection("ConnectionStrings").Bind(connectionStrings);

            //setup our DI
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddOptions()
                .AddSingleton(connectionStrings)
                .AddSingleton<IApplicationConfig, ApplicationConfig>()
                .AddSingleton<IContentConsumer, ContentConsumer>()
                .BuildServiceProvider();

            //configure console logging
            var loggerFactory = serviceProvider
                .GetService<ILoggerFactory>();
            loggerFactory.AddConsole();

            var logger = loggerFactory.CreateLogger<Program>();

            Thread.Sleep(TimeSpan.FromMinutes(1));

            var contentConsumer = serviceProvider.GetService<IContentConsumer>();
            contentConsumer.Consume();

            logger.LogInformation("Done.");
        }
    }
}
