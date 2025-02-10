#if TEST
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace DashboardCode.Routines.Storage.EfModelTest.EfCore.Test
{
    [TestClass]
    public class EfModelEfCoreUnitTest
    {
        readonly string connectionString;
        public EfModelEfCoreUnitTest()
        {
#if NET9_0_OR_GREATER
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