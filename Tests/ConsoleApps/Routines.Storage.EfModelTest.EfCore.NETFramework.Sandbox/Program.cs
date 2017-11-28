using System.Configuration;

namespace DashboardCode.Routines.Storage.EfModelTest.EfCore.NETFramework.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["EfCoreTest"].ConnectionString;
            DbContextTests.ParallelTest(connectionString);
            DbContextTests.SqlServerTest(connectionString);
            DbContextTests.InMemoryTest();
        }
    }
} 