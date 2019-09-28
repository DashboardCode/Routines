using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using DashboardCode.AspNetCore.Http;
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
                // TODO: with a lot of chunks may be we will need to loop through manifest.json
                //.AddJsonFile($"./wwwroot/dist/manifest.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            // TODO:
            // updatable configuration https://stackoverflow.com/questions/40970944/how-to-update-values-into-appsetting-json
            Configuration = builder.Build();
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
            // todo: configuration container should be builded from "snapshot" that is acessed by 
            //serviceCollection.AddScoped(sp => sp.GetService<Microsoft.Extensions.Options.IOptionsSnapshot<List<RoutineResolvable>>>().Value);

            serviceCollection.AddMemoryCache(); // AddDistributedMemoryCache();
            serviceCollection.AddMvc((options) => { options.EnableEndpointRouting = false; }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            serviceCollection.AddSingleton(new DevProxyMiddlewareSettings(
                new PathString("/dist"), 
                new Uri("http://localhost:63558"))
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceCollection services)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                //app.UseMiddleware<DevProxyMiddleware>();
            }

            if (applicationSettings.UseStandardDeveloperErrorPage)
            {
                app.UseDeveloperExceptionPage();
                // FYI: DatabaseErrorPageExtensions from Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore assembly
                //DatabaseErrorPageExtensions.UseDatabaseErrorPage(app);
            }
            else
            {
                const string errorPath = "/Error";

                app.UseExceptionHandler(errorPath);

                //app.UseStatusCodePages((statusCodeContext)=> {
                //    statusCodeContext.HttpContext
                //    return Task.CompletedTask;
                //});
                //app.UseStatusCodePagesWithReExecute("/Error");

                app.Use(async (context, next) =>
                {
                    if (context.Request.Path == errorPath)
                    {
                        var ex = context.Features.Get<IExceptionHandlerFeature>().Error;
                        var originalFeature = context.Features.Get<IExceptionHandlerPathFeature>();

                        if (originalFeature != null && originalFeature.Path != null && originalFeature.Path.Contains("Api/")) // TODO: regex
                        {
                            context.Response.ContentType = "application/json";
                            var aspRequestId = System.Diagnostics.Activity.Current?.Id ?? context.TraceIdentifier;
                            await context.Response.WriteAsync(MvcAppManager.GetErrorActionJson(ex, aspRequestId, applicationSettings.ForceDetailsOnCustomErrorPage));
                            return;
                        }
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