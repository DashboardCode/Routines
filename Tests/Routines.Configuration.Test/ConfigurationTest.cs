using System;

// https://msdn.microsoft.com/en-us/magazine/mt632279.aspx
// [DeploymentItem("appsettings.json")]

namespace DashboardCode.Routines.Configuration.Test
{
#if !(NETCOREAPP1_1 || NETCOREAPP2_0)
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
#endif
    public class ConfigurationTest
    {

#if NETCOREAPP1_1 || NETCOREAPP2_0
        [Xunit.Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void CreateConfigurationSmokeTest()
        {
            var loader = ZoningSharedSourceManager.GetLoader();
            var container = new ConfigurationContainer(loader, new MemberTag("Namespace1", "Class1", "Member1"));
        }


#if NETCOREAPP1_1 || NETCOREAPP2_0
        [Xunit.Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif

        public void TestState()
        {
            var state = new WrappedContainer(nameof(ConfigurationTest), nameof(TestState));
            var t1 = state.Resolve<LoggingConfiguration>();
            var t2 = state.Resolve<LoggingPerformanceConfiguration>();
            if (!(t1.Output == false && t2.ThresholdSec == (decimal)0.1))
                throw new Exception("Test fails");

            var containerS = new WrappedContainer(nameof(ConfigurationTest), nameof(TestState), "superuser");
            var t1s = containerS.Resolve<LoggingConfiguration>();
            var t2s = containerS.Resolve<LoggingPerformanceConfiguration>();
            if (!(t1s.Output == true && t2s.ThresholdSec == (decimal)0.5))
                throw new Exception("Test fails");
        }

#if NETCOREAPP1_1 || NETCOREAPP2_0
        [Xunit.Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif

        public void TestConfigruationContainer()
        {
            var loader = ZoningSharedSourceManager.GetLoader();
            var basicConfigContainer1 =
                new ConfigurationContainer(loader, new MemberTag("theNamespace", nameof(ConfigurationTest), nameof(TestConfigruationContainer)));

            var t1 = basicConfigContainer1.ResolveAlt<LoggingPerformanceConfiguration>();
            if (!(t1.ThresholdSec == 2))
                throw new Exception("Test fails. Case 1");

            var basicConfigContainer2 = new ConfigurationContainer(loader, new MemberTag("wrongNamespace", nameof(ConfigurationTest), nameof(TestConfigruationContainer)));
            var t2 = basicConfigContainer2.ResolveAlt<LoggingPerformanceConfiguration>();
            if (!(t2.ThresholdSec == 0)) // default value, it means configuration was not found because of wrong Namespace
                throw new Exception("Test fails. Case 2");

            var basicConfigContainer3 = new ConfigurationContainer(loader, new MemberTag("theNamespace", nameof(ConfigurationTest), nameof(TestConfigruationContainer)));
            var basicConfigContainer3s = new ConfigurationContainer(loader, new MemberTag("theNamespace", nameof(ConfigurationTest), nameof(TestConfigruationContainer)), "testuser");
            var t3 = basicConfigContainer3.ResolveSerialized(null, "MyTestConfigurationZZZ");
            if (t3 != null)
                throw new Exception("Test fails. Case 3");
            var t4 = basicConfigContainer3s.ResolveSerialized(null, "MyTestConfigurationZZZ");
            if (t4 == null)
                throw new Exception("Test fails. Case 4");
        }

#if NETCOREAPP1_1 || NETCOREAPP2_0
        [Xunit.Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif

        public void TestAsterix()
        {
            var state1 = new WrappedContainer(nameof(ConfigurationTest), nameof(TestAsterix));
            var t1 = state1.Resolve<MyTestConfiguration>();
            if (t1._Value != 10)
                throw new Exception("Test fails. Case 1");

            var state2 = new WrappedContainer(nameof(ConfigurationTest), nameof(TestAsterix), "superuser");
            var t2 = state2.Resolve<MyTestConfiguration>();
            if (t2._Value != 100)
                throw new Exception("Test fails. Case 2");
        }

#if NETCOREAPP1_1 || NETCOREAPP2_0
        [Xunit.Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void TestTools()
        {
            if (StringExtensions.IsLetterOrUnderscore('0'))
                throw new Exception("Test fails. Case 1");
        }
    }
}