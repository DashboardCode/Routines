using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace DashboardCode.Routines.AspNetCore.Sandbox
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(services);
            //services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceCollection serviceCollection)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.Map("/HelloWorld", builder => builder.Run(async httpContext =>
            {
                await httpContext.Response.WriteAsync("Hello World");
            }));

            // TODO: add properties tree
            // TODO: add React.js / jsx
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