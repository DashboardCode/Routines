#if TEST
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace DashboardCode.Routines.Storage.EfModelTest.EfCore.Test
{
    [TestClass]
    public class EfModelEfCoreUnitTest
    {
        string connectionString;
        public EfModelEfCoreUnitTest()
        {
#if NETCOREAPP
            var configurationRoot =
                DashboardCode.Routines.Storage.EfModelTest.EfCore.NETCore.Test.
                ConfigurationManager.ResolveConfigurationRoot();
            connectionString = configurationRoot.GetSection("ConnectionString").Value;
#else
            connectionString = 
                ConfigurationManager.ConnectionStrings["EfCoreTest"].ConnectionString;
#endif
        }

        [TestMethod]
        public void ParallelTest()
        {
            DbContextTests.ParallelTest(connectionString);
        }

        [TestMethod]
        public void SqlServerTest()
        {
            DbContextTests.SqlServerTest(connectionString);
        }

        [TestMethod]
        public void InMemoryTest()
        {
            DbContextTests.InMemoryTest();
        }
    }
}
#endif