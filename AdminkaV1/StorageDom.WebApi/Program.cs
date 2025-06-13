
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.ApplicationInsights.Extensibility;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.OData;

namespace AdminkaV1.StorageDom.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Get Authority and Audience
            var jwtSettings = builder.Configuration.GetSection("Jwt");

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
#if DEVDEBUG // managed by environment variable and MSBUILD, define ENV_DEVDEBUG=DEBUG to mockup JWT token for dev time
                StaticTools.ConfigureDevTimeJwtBarrierOptions(options, jwtSettings["Audience"]);
                
#else
                options.Authority = jwtSettings["Authority"];
                options.TokenValidationParameters.ValidAudience = jwtSettings["Audience"];
#endif
            });
            
            // currently odata is not oblgatory, but will be used to support pagination and filtering when we have more than 100 records tables
            builder.Services.AddControllers().AddOData(opt =>
                    opt.Select().Filter().OrderBy().Expand().Count().SetMaxTop(100)
                    .AddRouteComponents("ui", StaticTools.GetEdmModel())
            );

            var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
            var allowSpecificOrigins = "AllowReactApp";

            if (allowedOrigins!=null && allowedOrigins.Length > 0)
            {
                // Define CORS policy
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy(allowSpecificOrigins, policy =>
                    {
                        policy.WithOrigins(allowedOrigins)
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
                });
            }
#if !DEVDEBUG // this slower a startup for debug
            builder.Logging.AddApplicationInsights();
            builder.Services.AddApplicationInsightsTelemetry();
#endif
            builder.Services.AddControllers();
            var app = builder.Build();

            // TODO: Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            // Note: OpenAPI could be used for CRUD forms validation schema generation "Zod for react-hook-form"
            // This can be done once in development and there are no need to publish it in production.
            // if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Staging")
            // {
            //     app.UseSwagger();
            //     app.UseSwaggerUI();
            // }

            //controls errors, set env variable ASPNETCORE_ENVIRONMENT = Development to enable
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); // Shows full stack traces
            }
            else
            {
                app.UseExceptionHandler("/Error"); // Or a generic fallback
            }
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors(allowSpecificOrigins); // always after UseRouting() and before both UseAuthorization() and MapControllers()
            

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
