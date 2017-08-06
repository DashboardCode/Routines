using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DashboardCode.Routines.Configuration.NETFramework.Test
{
    [TestClass]
    public class NETFrameworkObsoleteConfigurationTest
    {

        [TestMethod]
        public void UpdateConfiguration()
        {
            #pragma warning disable 612, 618
            ConfigurationManagerLoader.UpdateConfiguration(
                routineNamespace:null,
                routineClass: nameof(NETFrameworkObsoleteConfigurationTest),
                routineMember: null,
                routineFor: null,
                resolvableNamespace: null,
                resolvableType: "TestConfiguration",
                resolvableValue: "{_Value:-100}");
            ConfigurationManagerLoader.UpdateConfiguration(
                routineNamespace: null,
                routineClass: "ConfigurationUnitTest",
                routineMember: null,
                routineFor: "alterFor",
                resolvableNamespace: null,
                resolvableType: "TestConfiguration2",
                resolvableValue: "{_Value:-1}");
            ConfigurationManagerLoader.UpdateConfiguration(
                routineNamespace: "OtherNamespace",
                routineClass: nameof(NETFrameworkObsoleteConfigurationTest),
                routineMember: null,
                routineFor: "alterFor",
                resolvableNamespace: "t1",
                resolvableType: "TestConfiguration3",
                resolvableValue: "{_Value:-1}");

            ConfigurationManagerLoader.UpdateConfiguration(
                routineNamespace: "OtherNamespace",
                routineClass: nameof(NETFrameworkObsoleteConfigurationTest),
                routineMember: null,
                routineFor: "alterFor",
                resolvableNamespace: "t2",
                resolvableType: "TestConfiguration3",
                resolvableValue: "{_Value:-1}");

            string xml1 = ConfigurationManagerLoader.GetConfigurationXml();
            string xml2 = ConfigurationManagerLoader.GetConfigurationManualXml();

            ConfigurationManagerLoader.ClearConfigurationFor("alterFor");

            string xml1b = ConfigurationManagerLoader.GetConfigurationXml();
            string xml2b = ConfigurationManagerLoader.GetConfigurationManualXml();

            string track = ConfigurationManagerLoader.TimeStamps();
            #pragma warning restore 612, 618    
        }

        [TestMethod]
        public void UpdateNotValidConfiguration()
        {
            #pragma warning disable 612, 618
            int i = 0;
            try
            {
                ConfigurationManagerLoader.UpdateConfiguration(
                   routineNamespace: null,
                   routineClass: "45"+ nameof(NETFrameworkObsoleteConfigurationTest),
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
                ConfigurationManagerLoader.UpdateConfiguration(
                   routineNamespace: null,
                   routineClass: "NETFrameworkObsolete.ConfigurationTest",
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
                ConfigurationManagerLoader.UpdateConfiguration(
                   routineNamespace: null,
                   routineClass: nameof(NETFrameworkObsoleteConfigurationTest),
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
                ConfigurationManagerLoader.UpdateConfiguration(
                   routineNamespace: "154",
                   routineClass: nameof(NETFrameworkObsoleteConfigurationTest),
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
                ConfigurationManagerLoader.UpdateConfiguration(
                   routineNamespace: null,
                   routineClass: nameof(NETFrameworkObsoleteConfigurationTest),
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
                ConfigurationManagerLoader.UpdateConfiguration(
                   routineNamespace: null,
                   routineClass: nameof(NETFrameworkObsoleteConfigurationTest),
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
                ConfigurationManagerLoader.UpdateConfiguration(
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
            #pragma warning restore 612, 618
        }
    }
}
