using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DashboardCode.Routines.Storage.EfModelTest.EfCore.NETCore.Test
{
    [TestClass]
    public class EfModelEfCoreUnitTest
    {
        string connectionString;
        public EfModelEfCoreUnitTest()
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json", false, true); // false indicates file is not optional
            var configurationRoot = configurationBuilder.Build();
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