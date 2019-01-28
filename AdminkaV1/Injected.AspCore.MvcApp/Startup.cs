using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using DashboardCode.Routines.AspNetCore;
using DashboardCode.Routines.Configuration.Standard;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp
{
    public class Startup
    {
        public Startup(IHostingEnvironment hostingEnvironment)
        {
            // monitor configuration on changes
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/primitives/change-tokens?view=aspnetcore-2.1
            var builder = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", optional: true)
                .AddJsonFile($"./wwwroot/dist/manifest.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            // TODO:
            // updatable configuration https://stackoverflow.com/questions/40970944/how-to-update-values-into-appsetting-json
            Configuration = builder.Build();
            IConfiguration c = Configuration;
            if (hostingEnvironment.IsDevelopment())
                builder.AddUserSecrets<Startup>();
        }

        private IConfiguration Configuration { get; } // is updatable on change

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.Name = ".AdminkaV1.Session";
            });

            // Add framework services.
            services.AddMvc();
            services.AddSingleton(Configuration);
            services.Configure<List<RoutineResolvable>>(Configuration.GetSection("Routines"));
            services.AddSingleton(services);
            var applicationSettings = InjectedManager.CreateApplicationSettingsStandard(Configuration);
            services.AddSingleton(applicationSettings);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceCollection services)
        {
            var configurationSection = Configuration.GetSection("Logging");
            //loggerFactory.AddConsole(configurationSection);
            //loggerFactory.AddDebug();

            // TODO: experiment with ETW 
            // https://docs.microsoft.com/en-us/dotnet/framework/wcf/samples/etw-tracing
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?tabs=aspnetcore2x
            // logging.AddEventSourceLogger(); //  Microsoft.Extensions.Logging.EventSource 

            if (true /*env.IsDevelopment()*/)
            {

                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
#pragma warning disable CS0162
                app.UseExceptionHandler("/Home/Error");
#pragma warning restore CS0162
            }

            app.UseStaticFiles();

            app.UseSession();

            app.UseMiddleware<DurationMiddleware>("X-AdminkaV1-Duration-MSec");

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "forms",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
            });
        }
    }
}