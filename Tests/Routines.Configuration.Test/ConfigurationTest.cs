using System;
using System.Collections.Generic;
using System.Text;

#if NETCOREAPP1_1
    using Xunit;
    using DashboardCode.Routines.Configuration.NETStandard;
    using DashboardCode.Routines.Configuration.NETStandard.Test;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
    using DashboardCode.Routines.Configuration.NETFramework;
    using DashboardCode.Routines.Configuration.NETFramework.Test;
#endif 


namespace DashboardCode.Routines.Configuration.Test
{
#if !NETCOREAPP1_1
    [TestClass]
#endif
    public class ConfigurationTest
    {
#if NETCOREAPP1_1
        ConfigurationNETStandard Configuration = new ConfigurationNETStandard();
#else
        ConfigurationNETFramework Configuration = new ConfigurationNETFramework();
#endif

#if NETCOREAPP1_1
        [Fact]
#else
        // https://msdn.microsoft.com/en-us/magazine/mt632279.aspx
        // [DeploymentItem("appsettings.json")]
        [TestMethod]
#endif
        public void ReadConfigurationTest()
        {
           var configurationRoot = Configuration.GetSpecifiableConfigurationContainer("Namespace1", "Class1", "Member1"); 
        }
    }
}
