using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;

namespace Vse.Routines.Configuration.NETStandard.Test
{
    [TestClass]
    public class StandardConfigurationUnitTest
    {
        public StandardConfigurationUnitTest()
        {
            string fileName = "appsettings.json";
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(this.GetType().Namespace+"."+fileName)))
                using (var fileStream = new FileStream(fileName, FileMode.Create))
                    reader.BaseStream.CopyTo(fileStream);
        }

        // https://msdn.microsoft.com/en-us/magazine/mt632279.aspx
        [TestMethod]
        [DeploymentItem("appsettings.json")]
        public void JsonConfigurationTest()
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json", false, true); // false indicates file is not optional
            var configurationRoot = configurationBuilder.Build();
            var r = RoutinesConfigurationManager.GetConfigurationContainer(configurationRoot,"Namespace1", "Class1", "Member1");
        }
    }
}
