using System.Configuration;
#if TEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace DashboardCode.Routines.Storage.EfModelTest.EfCore.Test
{
    #if TEST
    [TestClass]
    #endif
    public class EfCoreJsonUnitTest
    {
#if TEST
        string connectionString;
#endif
        public EfCoreJsonUnitTest()
        {
#if TEST
#if NETCOREAPP
            var configurationRoot =
                DashboardCode.Routines.Storage.EfModelTest.EfCore.NETCore.Test.
                ConfigurationManager.ResolveConfigurationRoot();
            connectionString = configurationRoot.GetSection("ConnectionString").Value;
#else
            connectionString = 
                ConfigurationManager.ConnectionStrings["EfCoreTest"].ConnectionString;
#endif
#endif
        }

#if TEST
        [TestMethod]
        public void SqlServerGetJson()
        {
            DbContextTests.SqlServerGetJson(connectionString);
        }
#endif
    }
}