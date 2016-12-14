using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Vse.Routines.Configuration.Test
{
    [TestClass]
    public class ConfigurationUnitTest
    {
        [TestMethod]
        public void TestState()
        {
            var state = new State(nameof(ConfigurationUnitTest), nameof(TestState));
            var t1 = state.Resolve<LoggingConfiguration>();
            var t2 = state.Resolve<LoggingPerformanceConfiguration>();
            if (!(t1.Output == false && t2.ThresholdSec==(decimal)0.1))
                throw new ApplicationException("Test fails");

            var containerS = new State(nameof(ConfigurationUnitTest), nameof(TestState), "superuser");
            var t1s = containerS.Resolve<LoggingConfiguration>();
            var t2s = containerS.Resolve<LoggingPerformanceConfiguration>();
            if (!(t1s.Output == true && t2s.ThresholdSec == (decimal)0.5))
                throw new ApplicationException("Test fails");
        }

        [TestMethod]
        public void TestConfigruationContainer()
        {
            var basicConfigContainer1 = RoutinesConfigurationManager.GetConfigurationContainer("theNamespace", nameof(ConfigurationUnitTest), nameof(TestConfigruationContainer));
            var t1 = basicConfigContainer1.Resolve<LoggingPerformanceConfiguration>();
            if (!(t1.ThresholdSec == 2))
                throw new ApplicationException("Test fails. Case 1");
            var basicConfigContainer2 = RoutinesConfigurationManager.GetConfigurationContainer("wrongNamespace", nameof(ConfigurationUnitTest), nameof(TestConfigruationContainer));
            var t2 = basicConfigContainer2.Resolve<LoggingPerformanceConfiguration>();
            if (!(t2.ThresholdSec == 0)) // default value, it means configuration was not found because of wrong Namespace
                throw new ApplicationException("Test fails. Case 2");
        }

        [TestMethod]
        public void TestAsterix()
        {
            var state1 = new State(nameof(ConfigurationUnitTest), nameof(TestAsterix));
            var t1 = state1.Resolve<TestConfiguration>();
            if (t1._Value!=10)
                throw new ApplicationException("Test fails. Case 1");

            var state2 = new State(nameof(ConfigurationUnitTest), nameof(TestAsterix), "superuser");
            var t2 = state2.Resolve<TestConfiguration>();
            if (t2._Value != 100)
                throw new ApplicationException("Test fails. Case 2");
        }

        [TestMethod]
        public void TestTools()
        {
            if (StringExtensions.IsLetterOrUnderscore('0'))
                throw new ApplicationException("Test fails. Case 1");
        }
    }
}
