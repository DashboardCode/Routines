using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DashboardCode.Routines.Storage.EfModelTest.EfCore
{
    public class MyDbContextFactory : IDesignTimeDbContextFactory<MyDbContext>
    {
        public MyDbContext CreateDbContext(string[] args)
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json", false, true); // false indicates file is not optional
            var configurationRoot = configurationBuilder.Build();
            var connectionString = configurationRoot.GetSection("ConnectionString").Value;

            return new MyDbContext(MyDbContext.BuildOptionsBuilder(connectionString));
        }
    }
}