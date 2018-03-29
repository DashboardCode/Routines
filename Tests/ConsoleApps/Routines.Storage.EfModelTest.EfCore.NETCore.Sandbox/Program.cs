using Microsoft.Extensions.Configuration;

namespace DashboardCode.Routines.Storage.EfModelTest.EfCore.NETCore.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var configurationRoot = ConfigurationManager.ResolveConfigurationRoot();
            var connectionString = configurationRoot.GetSection("ConnectionString").Value;

            DbContextTests.ParallelTest(connectionString);
            DbContextTests.SqlServerTest(connectionString);
            DbContextTests.InMemoryTest();
        }
    }
}