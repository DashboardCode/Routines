using Microsoft.EntityFrameworkCore.Design;

namespace DashboardCode.Routines.Storage.EfModelTest.EfCore.Sandbox
{
    public class MyDbContextFactory : IDesignTimeDbContextFactory<MyDbContext>
    {
        public MyDbContext CreateDbContext(string[] args)
        {
            var configurationRoot = ConfigurationManager.ResolveConfigurationRoot();
            var connectionString = configurationRoot.GetSection("ConnectionString").Value;

            return new MyDbContext(MyDbContext.BuildOptionsBuilder(connectionString));
        }
    }
}