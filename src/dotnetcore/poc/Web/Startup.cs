using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Core.Data;
using Entity;
using Web.Core.Config;
using Web.Middlewares;
using Core.Config;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text;
using Data.RabbitMq;

namespace Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));
            services.AddSingleton<IApplicationConfig, ApplicationConfig>();

            var providers = new Providers();
            Configuration.GetSection("Providers").Bind(providers);
            services.AddSingleton(providers);

            services.AddMemoryCache();
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = Configuration.GetConnectionString("Redis");
                options.InstanceName = "poc";
            });

            ConfigureDataProvider(services, providers);

            services.AddSingleton<IContentProducer, ContentProducer>();
            services.AddSingleton<IContentConsumer, ContentConsumer>();

            services.AddMvc();
        }

        private void ConfigureDataProvider(IServiceCollection services, Providers providers)
        {
            switch (providers.ContentProvider)
            {
                case "InMemory":
                    {
                        services.AddDbContext<ContentContext>(options => options.UseInMemoryDatabase(databaseName: "ContextDB"));
                        services.AddScoped<IRepository<Content, int>, Data.EntityFramework.ContentRepository>();
                        break;
                    }

                case "MsSql":
                    {
                        services.AddDbContext<ContentContext>(options => options.UseSqlServer(Configuration.GetConnectionString(providers.ContentProvider)));
                        services.AddScoped<IRepository<Content, int>, Data.EntityFramework.ContentRepository>();
                        break;
                    }

                case "MySql":
                    {
                        services.AddDbContext<ContentContext>(options => options.UseMySql(Configuration.GetConnectionString(providers.ContentProvider)));
                        services.AddScoped<IRepository<Content, int>, Data.EntityFramework.ContentRepository>();
                        break;
                    }

                case "Sqlite":
                    {
                        services.AddDbContext<ContentContext>(options => options.UseSqlite(Configuration.GetConnectionString(providers.ContentProvider)));
                        services.AddScoped<IRepository<Content, int>, Data.EntityFramework.ContentRepository>();
                        break;
                    }

                case "ElasticSearch":
                    {
                        services.AddScoped<IRepository<Content, int>, Data.ElasticSearch.ContentRepository>();
                        break;
                    }

                case "Mongo":
                    {
                        services.AddScoped<IRepository<Content, int>, Data.Mongo.ContentRepository>();
                        break;
                    }

                case "Redis":
                    {
                        services.AddScoped<IRepository<Content, int>, Data.Redis.ContentRepository>();
                        break;
                    }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IDistributedCache distributedCacher)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            SetAppStartTime(distributedCacher);
            app.UseApplicationStartTimeHeader();

            app.UseExceptionLoggingHandler();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void SetAppStartTime(IDistributedCache distributedCacher)
        {
            distributedCacher.SetString("applicationStartTime", DateTime.Now.ToString(), new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromHours(1)
            });
        }
    }
}
