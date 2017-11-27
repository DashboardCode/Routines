using DashboardCode.Routines.Storage.EfModelTest.EfCoreTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;

namespace Routines.Storage.EfModelTest.EfCore.NETFramework.Test
{
    [TestClass]
    public class EfModelEfCoreUnitTest
    {
        string connectionString;
        public EfModelEfCoreUnitTest()
        {
            connectionString = ConfigurationManager.ConnectionStrings["EfCoreTest"].ConnectionString;
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
