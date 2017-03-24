using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;

namespace Vse.Routines.Configuration.NETStandard.Test
{
    [TestClass]
    public class StandardConfigurationUnitTest
    {
        // https://msdn.microsoft.com/en-us/magazine/mt632279.aspx
        [TestMethod]
        public void JsonConfigurationTest()
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json", false, true); // false indicates file is not optional
            var configurationRoot = configurationBuilder.Build();
            var r = RoutinesConfigurationManager.GetConfigurationContainer(configurationRoot,"Namespace1", "Class1", "Member1");
        }
    }
}
