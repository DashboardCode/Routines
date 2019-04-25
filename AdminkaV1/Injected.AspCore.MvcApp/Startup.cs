using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Diagnostics;

using DashboardCode.Routines.AspNetCore;
using DashboardCode.Routines.Configuration.Standard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

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
        ApplicationSettings applicationSettings;
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            applicationSettings = InjectedManager.CreateApplicationSettingsStandard(Configuration);
            serviceCollection.AddSingleton(applicationSettings);
            serviceCollection.AddSingleton(Configuration);
            serviceCollection.AddSingleton(serviceCollection);

            // for section real time update
            serviceCollection.Configure<List<RoutineResolvable>>(Configuration.GetSection("Routines"));
            // todo test alternative: 
            //serviceCollection.AddScoped(sp => sp.GetService<Microsoft.Extensions.Options.IOptionsSnapshot<List<RoutineResolvable>>>().Value);

            serviceCollection.AddMemoryCache(); // AddDistributedMemoryCache();
            serviceCollection.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceCollection services)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
            }


            if (false /*env.IsDevelopment()*/)
            {

                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                const string errorPath = "/Error";

                app.UseExceptionHandler(errorPath);

                // 
                //app.UseStatusCodePages((statusCodeContext)=> {
                //    statusCodeContext.HttpContext
                //    return Task.CompletedTask;
                //});

                app.UseStatusCodePagesWithReExecute("/Error");
                app.Use(async (context, next) =>
                {
                    if (context.Request.Path == errorPath)
                    {
                        //var ex = context.Features.Get<IExceptionHandlerFeature>().Error;
                        //var originalFeature = context.Features.Get<IExceptionHandlerPathFeature>();

                        //if (originalFeature != null && originalFeature.Path != null && originalFeature.Path.Contains("Api/")) // TODO: regex
                        //{
                        //    context.Response.ContentType = "application/json";
                        //    var aspRequestId = System.Diagnostics.Activity.Current?.Id ?? context.TraceIdentifier;
                        //    await context.Response.WriteAsync(AspCoreManager.GetErrorActionJson(ex, aspRequestId, applicationSettings.ForceDetailsOnCustomErrorPage));
                        //    return;
                        //}
                    }

                    // Request.Path is not for /Error *or* this isn't an API call.
                    await next();
                });
            }

            app.UseStaticFiles();

            //app.UseSession();

            //app.UseMiddleware<DurationMiddleware>("X-AdminkaV1-Duration-MSec");

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "defaultArea",
                    template: "{area:exists}/{controller}/{action}");
            });
        }
    }
}