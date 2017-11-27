using Microsoft.Extensions.Configuration;
using DashboardCode.Routines.Storage.EfModelTest.EfCoreTest;

namespace DashboardCode.EfCore.NETCore.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json", false, true); // false indicates file is not optional
            var configurationRoot = configurationBuilder.Build();
            var connectionString = configurationRoot.GetSection("ConnectionString").Value;
            DbContextTests.ParallelTest(connectionString);
            DbContextTests.SqlServerTest(connectionString);
            DbContextTests.InMemoryTest();
        }
    }
}