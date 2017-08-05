using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DashboardCode.Routines.Configuration.Test;

namespace DashboardCode.Routines.Configuration.NETFramework.Test
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
            var basicConfigContainer1 = RoutinesConfigurationManager.GetConfigurationContainer(new MemberTag(  "theNamespace", nameof(ConfigurationUnitTest),  nameof(TestConfigruationContainer) ));
            var t1 = basicConfigContainer1.Resolve<LoggingPerformanceConfiguration>();
            if (!(t1.ThresholdSec == 2))
                throw new ApplicationException("Test fails. Case 1");

            var basicConfigContainer2 = RoutinesConfigurationManager.GetConfigurationContainer(new MemberTag (  "wrongNamespace",  nameof(ConfigurationUnitTest),  nameof(TestConfigruationContainer)));
            var t2 = basicConfigContainer2.Resolve<LoggingPerformanceConfiguration>();
            if (!(t2.ThresholdSec == 0)) // default value, it means configuration was not found because of wrong Namespace
                throw new ApplicationException("Test fails. Case 2");

            var basicConfigContainer3 = RoutinesConfigurationManager.GetConfigurationContainer(new MemberTag("theNamespace",  nameof(ConfigurationUnitTest), nameof(TestConfigruationContainer)));
            var specified = basicConfigContainer3.Specify("testuser");
            var t3 = basicConfigContainer3.ResolveSerialized(null, "MyTestConfigurationZZZ");
            if (t3!=null)
                throw new ApplicationException("Test fails. Case 3");
            var t4 = specified.ResolveSerialized(null, "MyTestConfigurationZZZ");
            if (t4==null)
                throw new ApplicationException("Test fails. Case 4");
        }

        [TestMethod]
        public void TestAsterix()
        {
            var state1 = new State(nameof(ConfigurationUnitTest), nameof(TestAsterix));
            var t1 = state1.Resolve<MyTestConfiguration>();
            if (t1._Value!=10)
                throw new ApplicationException("Test fails. Case 1");

            var state2 = new State(nameof(ConfigurationUnitTest), nameof(TestAsterix), "superuser");
            var t2 = state2.Resolve<MyTestConfiguration>();
            if (t2._Value != 100)
                throw new ApplicationException("Test fails. Case 2");
        }

        [TestMethod]
        public void TestTools()
        {
            if (StringExtensions.IsLetterOrUnderscore('0'))
                throw new ApplicationException("Test fails. Case 1");
        }

        [TestMethod]
        public void UpdateConfiguration()
        {
            RoutinesConfigurationManager.UpdateConfiguration(
                routineNamespace:null,
                routineClass: "ConfigurationUnitTest",
                routineMember: null,
                routineFor: null,
                resolvableNamespace: null,
                resolvableType: "TestConfiguration",
                resolvableValue: "{_Value:-100}");
            RoutinesConfigurationManager.UpdateConfiguration(
                routineNamespace: null,
                routineClass: "ConfigurationUnitTest",
                routineMember: null,
                routineFor: "alterFor",
                resolvableNamespace: null,
                resolvableType: "TestConfiguration2",
                resolvableValue: "{_Value:-1}");
            RoutinesConfigurationManager.UpdateConfiguration(
                routineNamespace: "OtherNamespace",
                routineClass: "ConfigurationUnitTest",
                routineMember: null,
                routineFor: "alterFor",
                resolvableNamespace: "t1",
                resolvableType: "TestConfiguration3",
                resolvableValue: "{_Value:-1}");

            RoutinesConfigurationManager.UpdateConfiguration(
                routineNamespace: "OtherNamespace",
                routineClass: "ConfigurationUnitTest",
                routineMember: null,
                routineFor: "alterFor",
                resolvableNamespace: "t2",
                resolvableType: "TestConfiguration3",
                resolvableValue: "{_Value:-1}");

            string xml1 = RoutinesConfigurationManager.GetConfigurationXml();
            string xml2 = RoutinesConfigurationManager.GetConfigurationManualXml();

            RoutinesConfigurationManager.ClearConfigurationFor("alterFor");

            string xml1b = RoutinesConfigurationManager.GetConfigurationXml();
            string xml2b = RoutinesConfigurationManager.GetConfigurationManualXml();

            string track = RoutinesConfigurationManager.TimeStamps();
        }

        [TestMethod]
        public void UpdateNotValidConfiguration()
        {
            int i = 0;
            try
            {
                RoutinesConfigurationManager.UpdateConfiguration(
                   routineNamespace: null,
                   routineClass: "45ConfigurationUnitTest",
                   routineMember: null,
                   routineFor: null,
                   resolvableNamespace: null,
                   resolvableType: "TestConfiguration",
                   resolvableValue: "{_Value:-100}");
            }catch(InvalidOperationException)
            {
                i++;
            }

            try
            {
                RoutinesConfigurationManager.UpdateConfiguration(
                   routineNamespace: null,
                   routineClass: "Configuration.UnitTest",
                   routineMember: null,
                   routineFor: null,
                   resolvableNamespace: null,
                   resolvableType: "TestConfiguration",
                   resolvableValue: "{_Value:-100}");
            }
            catch (InvalidOperationException)
            {
                i++;
            }

            try
            {
                RoutinesConfigurationManager.UpdateConfiguration(
                   routineNamespace: null,
                   routineClass: "ConfigurationUnitTest",
                   routineMember: null,
                   routineFor: null,
                   resolvableNamespace: null,
                   resolvableType: "Test.Configuration",
                   resolvableValue: "{_Value:-100}");
            }
            catch (InvalidOperationException)
            {
                i++;
            }

            try
            {
                RoutinesConfigurationManager.UpdateConfiguration(
                   routineNamespace: "154",
                   routineClass: "ConfigurationUnitTest",
                   routineMember: null,
                   routineFor: null,
                   resolvableNamespace: null,
                   resolvableType: "TestConfiguration",
                   resolvableValue: "{_Value:-100}");
            }
            catch (InvalidOperationException)
            {
                i++;
            }

            try
            {
                RoutinesConfigurationManager.UpdateConfiguration(
                   routineNamespace: null,
                   routineClass: "ConfigurationUnitTest",
                   routineMember: "26",
                   routineFor: null,
                   resolvableNamespace: null,
                   resolvableType: "TestConfiguration",
                   resolvableValue: "{_Value:-100}");
            }
            catch (InvalidOperationException)
            {
                i++;
            }

            try
            {
                RoutinesConfigurationManager.UpdateConfiguration(
                   routineNamespace: null,
                   routineClass: "ConfigurationUnitTest",
                   routineMember: "26",
                   routineFor: null,
                   resolvableNamespace: null,
                   resolvableType: "TestConfiguration",
                   resolvableValue: "{_Value:-100}");
            }
            catch (InvalidOperationException)
            {
                i++;
            }

            try
            {
                RoutinesConfigurationManager.UpdateConfiguration(
                   routineNamespace: null,
                   routineClass: null,
                   routineMember: "TestMember",
                   routineFor: null,
                   resolvableNamespace: null,
                   resolvableType: "TestConfiguration",
                   resolvableValue: "{_Value:-100}");
            }
            catch (InvalidOperationException)
            {
                i++;
            }

            if (i != 7)
                throw new ApplicationException("validation error");
        }
    }
}
