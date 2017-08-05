#if NETCOREAPP1_1
    using Xunit;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif 


namespace DashboardCode.Routines.Configuration.Test
{
#if !NETCOREAPP1_1
    [TestClass]
#endif
    public class ConfigurationTest
    {

#if NETCOREAPP1_1
        [Fact]
#else
        // https://msdn.microsoft.com/en-us/magazine/mt632279.aspx
        // [DeploymentItem("appsettings.json")]
        [TestMethod]
#endif
        public void ReadConfigurationTest()
        {
           var configurationRoot = ZoningSharedSourceManager.GetConfiguration()
                .GetSpecifiableConfigurationContainer(
                    new MemberTag("Namespace1", "Class1", "Member1")
            ); 
        }
    }
}