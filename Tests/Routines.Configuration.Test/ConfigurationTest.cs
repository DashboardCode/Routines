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
            var loader = ZoningSharedSourceProjectManager.GetLoader();
            var container = new ConfigurationContainer(loader.GetGetRoutineConfigurationRecords(), new MemberTag("Namespace1", "Class1", "Member1"));
        }


#if NETCOREAPP1_1 || NETCOREAPP2_0
        [Xunit.Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif

        public void TestContainerResolve()
        {
            var container = new WrappedContainer(nameof(ConfigurationTest), nameof(TestContainerResolve));
            var t1 = container.Resolve<LoggingConfiguration>();
            var t2 = container.Resolve<LoggingThresholdConfiguration>();
            if (!(t1.Output == false && t2.ThresholdSec == (decimal)0.1  ))
                throw new Exception("Test fails");

            var containerS = new WrappedContainer(nameof(ConfigurationTest), nameof(TestContainerResolve), "superuser");
            var t1s = containerS.Resolve<LoggingConfiguration>();
            var t2s = containerS.Resolve<LoggingThresholdConfiguration>();
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
            var loader = ZoningSharedSourceProjectManager.GetLoader();
            var basicConfigContainer1 =
                new ConfigurationContainer(loader.GetGetRoutineConfigurationRecords(), new MemberTag("theNamespace", nameof(ConfigurationTest), nameof(TestConfigruationContainer)));

            var t1t = basicConfigContainer1.ResolveString<LoggingThresholdConfiguration>();
            var t1 = new LoggingThresholdConfiguration();
            t1.Report(t1t);
            if (!(t1.ThresholdSec == 2))
                throw new Exception("Test fails. Case 1");

            var basicConfigContainer2 = new ConfigurationContainer(loader.GetGetRoutineConfigurationRecords(), new MemberTag("wrongNamespace", nameof(ConfigurationTest), nameof(TestConfigruationContainer)));
            var t2t = basicConfigContainer2.ResolveString<LoggingThresholdConfiguration>();
            var t2 = new LoggingThresholdConfiguration();
            t2.Report(t2t);
            
            if (!(t2.ThresholdSec == 0)) // default value, it means configuration was not found because of wrong Namespace
                throw new Exception("Test fails. Case 2");

            var basicConfigContainer3 = new ConfigurationContainer(loader.GetGetRoutineConfigurationRecords(), new MemberTag("theNamespace", nameof(ConfigurationTest), nameof(TestConfigruationContainer)));
            var basicConfigContainer3s = new ConfigurationContainer(loader.GetGetRoutineConfigurationRecords(), new MemberTag("theNamespace", nameof(ConfigurationTest), nameof(TestConfigruationContainer)), "testuser");
            var t3 = basicConfigContainer3.ResolveString(null, "MyTestConfigurationZZZ");
            if (t3 != null)
                throw new Exception("Test fails. Case 3");
            var t4 = basicConfigContainer3s.ResolveString(null, "MyTestConfigurationZZZ");
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