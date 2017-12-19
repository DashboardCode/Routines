using System.Configuration;

namespace DashboardCode.Routines.Storage.EfModelTest.EfCore.NETFramework.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["EfCoreTest"].ConnectionString;
            DbContextTests.SqlServerTest2(connectionString);
            //DbContextTests.ParallelTest(connectionString);
            //DbContextTests.SqlServerTest(connectionString);
            //DbContextTests.InMemoryTest();
            //DbContextTests.Try(connectionString);
            System.Console.ReadLine();
        }
    }
} 