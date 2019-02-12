using System.Configuration;

namespace DashboardCode.Routines.Storage.EfModelTest.EfCore.NETFramework.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["EfCoreTest"].ConnectionString;
            
            DbContextTests.SqlServerGetJson(connectionString);

            //DbContextTests.ParallelTest(connectionString);
            //DbContextTests.SqlServerTest(connectionString);
            //DbContextTests.InMemoryTest();
            //DbContextTests.Try(connectionString);
            System.Console.WriteLine("OK");
            System.Console.ReadLine();
        }
    }
} 