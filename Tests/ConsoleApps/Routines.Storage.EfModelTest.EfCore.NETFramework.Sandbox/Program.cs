using System.Configuration;
using DashboardCode.Routines.Storage.EfModelTest.EfCoreTest;

namespace DashboardCode.EfCore.NETFramework.Sandbox
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