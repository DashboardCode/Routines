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

using Microsoft.Extensions.Hosting;
using DashboardCode.Routines.Configuration.Standard;
using Microsoft.Extensions.WebEncoders;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace DashboardCode.AdminkaV1.Injected.AspNetCore.WebApp
{
    public class Startup
    {
        private IConfiguration Configuration { get; } // is updatable on change
        
        ApplicationSettings applicationSettings;

        public Startup(IWebHostEnvironment webHostEnvironment) 
        {
            // monitor configuration on changes
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/primitives/change-tokens?view=aspnetcore-2.1
            var builder = new ConfigurationBuilder()
                .SetBasePath(webHostEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{webHostEnvironment.EnvironmentName}.json", optional: true)
                // TODO: with a lot of chunks may be we will need to loop through manifest.json
                //.AddJsonFile($"./wwwroot/dist/manifest.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            // TODO:
            // updatable configuration https://stackoverflow.com/questions/40970944/how-to-update-values-into-appsetting-json
            Configuration = builder.Build();
            if (webHostEnvironment.IsDevelopment())
                builder.AddUserSecrets<Startup>();
        }



        // This method gets called by the runtime. Use this method to add services to the container.
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

            serviceCollection.AddRazorPages();

            // NOTE: without this unicode characters (non-english) are serialized as HTML char entities (&#x00..)
            serviceCollection.Configure<WebEncoderOptions>(webEncoderOptions => {
                webEncoderOptions.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
            });

            // NOTE: alternatively for MVC
            // services.AddControllersWithViews();
            // NOTE: legacy ASP Core 
            // serviceCollection.AddMvc((options) => { options.EnableEndpointRouting = false; }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            // supports webpack real time server
            //serviceCollection.AddSingleton(new DevProxyMiddlewareSettings(
            //    new PathString("/dist"), 
            //    new Uri("http://localhost:63558"))
            //);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env/*, ILoggerFactory loggerFactory, IServiceCollection services*/)
        {
            if (env.IsDevelopment())
            {
                //app.UseBrowserLink();

                // supports webpack real time server
                //app.UseMiddleware<DevProxyMiddleware>();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            if (applicationSettings.UseStandardDeveloperErrorPage)
            {
                app.UseDeveloperExceptionPage();
                // FYI: DatabaseErrorPageExtensions from Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore assembly
                //DatabaseErrorPageExtensions.UseDatabaseErrorPage(app);
            }
            else
            {
                const string errorPath = "/Error"; // NOTE: "/Home/Error" for MVC

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

                        if (originalFeature != null && originalFeature.Path != null && originalFeature.Path.Contains("Api/", StringComparison.Ordinal)) // TODO: regex
                        {
                            context.Response.ContentType = "application/json";
                            var aspRequestId = System.Diagnostics.Activity.Current?.Id ?? context.TraceIdentifier;
                            await context.Response
                                .WriteAsync(MvcAppManager.GetErrorActionJson(ex, aspRequestId, applicationSettings.ForceDetailsOnCustomErrorPage));
                            // about ConfigureAwait read there https://stackoverflow.com/questions/13489065/best-practice-to-call-configureawait-for-all-server-side-code

                            return;
                        }
                    }

                    // Request.Path is not for /Error *or* this isn't an API call.
                    await next();
                });
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // TODO: know details ASP CORE 3
            app.UseRouting();

            //app.UseAuthorization();
            //app.UseCookiePolicy();
            //app.UseSession();

            //app.UseMiddleware<DurationMiddleware>("X-AdminkaV1-Duration-MSec");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();

                //NOTE: for MVC 
                //TODO: how to configure area
                //endpoints.MapControllerRoute(
                //    name: "default",
                //    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            // legacy, important because there is area in the template
            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "defaultArea",
            //        template: "{area:exists}/{controller}/{action}");
            //});
        }
    }
}