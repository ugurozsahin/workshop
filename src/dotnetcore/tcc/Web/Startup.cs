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
using Data.Initializer;
using Core.Cache;
using Data.Cache;

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
            services.Configure<Intervals>(Configuration.GetSection("Intervals"));
            services.AddSingleton<IApplicationConfig, ApplicationConfig>();

            services.AddMemoryCache();
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = Configuration.GetConnectionString("Redis");
                options.InstanceName = "tcc";
            });

            services.AddDbContext<ProductModelContext>(options => options.UseMySql(Configuration.GetConnectionString("MySql")));
            services.AddScoped<IRepository<ProductModel, int>, Data.EntityFramework.ProductModelRepository>();

            services.AddScoped<DatabaseInitializer>();
            services.AddScoped<CacheInitializer>();

            services.AddScoped<IProductModelCacheManager, ProductModelCacheManager>();
            services.AddSingleton<ICacher, DefaultCacher>();
            services.AddSingleton<CacheTimer>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            CacheTimer cacheTimer)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();

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

            app.UseExceptionLoggingHandler();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            cacheTimer.StartTimer();
        }
    }
}
