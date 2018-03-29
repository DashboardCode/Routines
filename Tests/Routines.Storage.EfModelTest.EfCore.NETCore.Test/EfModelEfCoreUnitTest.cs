using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DashboardCode.Routines.Storage.EfModelTest.EfCore.NETCore.Test
{
    [TestClass]
    public class EfModelEfCoreUnitTest
    {
        string connectionString;
        public EfModelEfCoreUnitTest()
        {
            var configurationRoot = ConfigurationManager.ResolveConfigurationRoot();
            connectionString = configurationRoot.GetSection("ConnectionString").Value;
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