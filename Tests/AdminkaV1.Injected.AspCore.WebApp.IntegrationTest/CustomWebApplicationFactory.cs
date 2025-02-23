using System;
using System.Collections.Generic;
using System.Text;
using DashboardCode.AdminkaV1.Injected.AspCore.WebApp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
//using Web.Api.Infrastructure.Data;

namespace DashboardCode.AdminkaV1.Injected.AspCore.WebApp.IntegrationTest
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(serviceCollection =>
            {
                //// Create a new service provider.
                //var newServiceCollection = new ServiceCollection();
                //    //.AddEntityFrameworkInMemoryDatabase()
                //var newServiceProvider = newServiceCollection.BuildServiceProvider();

                // Add a database context (AppDbContext) using an in-memory database for testing.
                //services.AddDbContext<AppDbContext>(options =>
                //{
                //    options.UseInMemoryDatabase("InMemoryAppDb");
                //    options.UseInternalServiceProvider(newServiceProvider);
                //});

                // Build the service provider.
                var serviceProvider = serviceCollection.BuildServiceProvider();

                // Create a scope to obtain a reference to the database contexts
                using var serviceScope = serviceProvider.CreateScope();
                var scopedServices = serviceScope.ServiceProvider;
                //var appDb = scopedServices.GetRequiredService<AppDbContext>();

                var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                // Ensure the database is created.
                //appDb.Database.EnsureCreated();

                //try
                //{
                //    // Seed the database with some specific test data.
                //    SeedData.PopulateTestData(appDb);
                //}
                //catch (Exception ex)
                //{
                //    logger.LogError(ex, "An error occurred seeding the " +
                //                        "database with test messages. Error: {ex.Message}");
                //}
            });
        }
    }
}
