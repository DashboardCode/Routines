﻿using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;


namespace Routines.AspNetCore.Mvc.Sandbox
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(serviceCollection);
            serviceCollection.AddControllersWithViews();
            serviceCollection.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddDebug();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceCollection serviceCollection, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger("Startup");
            logger.LogWarning("Logger configured!");

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });


            app.Map("/HelloWorld", builder => builder.Run(async httpContext =>
            {
                await httpContext.Response.WriteAsync("Hello World");
            }));

            // TODO: add properties tree
            // TODO: try ServiceStack templates (promise dynamic generation!) insted of Razor http://docs.servicestack.net/why-not-razor
            app.Run(async httpContext =>
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append("<html><body>");
                stringBuilder.Append("<h1>ASP DI Container: IServiceCollection</h1>");
                stringBuilder.Append("<table><thead>");
                stringBuilder.Append("<tr><th>N</th><th>Type</th><th>ImplementationType</th><th>Lifetime</th></tr>");
                stringBuilder.Append("</thead><tbody>");
                var i = 0;
                foreach (var serviceDescriptor in serviceCollection)
                {
                    stringBuilder.Append("<tr>");
                    stringBuilder.Append($"<td>{++i}</td>");
                    stringBuilder.Append($"<td>{serviceDescriptor.ServiceType.FullName}</td>");
                    stringBuilder.Append($"<td>{serviceDescriptor.ImplementationType?.FullName}</td>");
                    stringBuilder.Append($"<td>{serviceDescriptor.Lifetime}</td>");
                    stringBuilder.Append("</tr>");
                }
                stringBuilder.Append("</tbody></table>");
                stringBuilder.Append("</body></html>");
                await httpContext.Response.WriteAsync(stringBuilder.ToString());
            });


        }
    }
}