using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;

namespace DashboardCode.Routines.Configuration.NETFramework
{
    public class AppSettings : IAppSettings
    {
        public string GetValue(string key)
        {
            var value = ConfigurationManager.AppSettings[key];
            return value;
        }
    }

    public class ConnectionStringMap : IConnectionStringMap
    {
        public string GetConnectionString(string name = "ConnectionString")
        {
            var connectionString = ConfigurationManager.ConnectionStrings[name].ConnectionString;
            return connectionString;
        }
    }

    public class ConfigurationManagerLoader : IConfigurationManagerLoader
    {
        const string key = "routinesConfiguration";

        public ConfigurationManagerLoader(string sectionName = key)
        {
            var section = ConfigurationManager.GetSection(sectionName);
            if (section == null)
                throw new System.Exception($"Configuration section '{sectionName}' was not found");
            var routinesConfigurationSection = (RoutinesConfigurationSection)section;
            SetGetRoutineConfigurationRecords(((IEnumerable)routinesConfigurationSection.Routines).Cast<IRoutineConfigurationRecord>());
        }

        private IEnumerable<IRoutineConfigurationRecord> getRoutineConfigurationRecords;

        public IEnumerable<IRoutineConfigurationRecord> GetGetRoutineConfigurationRecords()
        {
            return getRoutineConfigurationRecords;
        }

        private void SetGetRoutineConfigurationRecords(IEnumerable<IRoutineConfigurationRecord> value)
        {
            getRoutineConfigurationRecords = value;
        }

        #region Experimental: update and save configuration
        [System.Obsolete]
        public static void ClearConfigurationFor(
            string routineFor,
            string sectionName = key)
        {
            var routinesConfigurationSection = (RoutinesConfigurationSection)ConfigurationManager.GetSection(sectionName);
            var elements = new List<RoutineElement>();
            foreach (RoutineElement routineElement in routinesConfigurationSection.Routines)
            {
                if (StringExtensions.AsterixEquals(routineFor, routineElement.For))
                {
                    elements.Add(routineElement);
                }
            }
            foreach (var e in elements)
            {
                routinesConfigurationSection.Routines.Remove(e);
            }
        }
        [System.Obsolete]
        public static void UpdateConfiguration(
            string routineNamespace,
            string routineClass,
            string routineMember,
            string routineFor,
            string resolvableNamespace,
            string resolvableType,
            string resolvableValue,
            string sectionName = key)
        {
            var routinesConfigurationSection = (RoutinesConfigurationSection)ConfigurationManager.GetSection(sectionName);
            RoutineElement ourRoutineElement = null;
            ResolvableElement ourResolvableElement = null;
            foreach (RoutineElement routineElement in routinesConfigurationSection.Routines)
            {
                if (StringExtensions.AsterixEquals(routineNamespace, routineElement.Namespace))
                    if (StringExtensions.AsterixEquals(routineClass, routineElement.Type))
                        if (StringExtensions.AsterixEquals(routineMember, routineElement.Member))
                            if (StringExtensions.AsterixEquals(routineFor, routineElement.For))
                            {
                                ourRoutineElement = routineElement;
                                foreach (ResolvableElement resolvableElement in routineElement.Resolvables)
                                {
                                    if (StringExtensions.AsterixEquals(resolvableNamespace, resolvableElement.Namespace))
                                        if (StringExtensions.AsterixEquals(resolvableType, resolvableElement.Type))
                                        {
                                            ourResolvableElement = resolvableElement;
                                            break;
                                        }
                                }
                                break;
                            }
            }
            if (ourRoutineElement == null)
            {
                ourRoutineElement = new RoutineElement() { Namespace = routineNamespace, Type = routineClass, Member = routineMember, For = routineFor };
                ourRoutineElement.Validate();
                routinesConfigurationSection.Routines.Add(ourRoutineElement);
            }
            if (ourResolvableElement == null)
            {
                ourResolvableElement = new ResolvableElement() { Namespace = resolvableNamespace, Type = resolvableType, Value = resolvableValue };
                ourResolvableElement.Validate();
                ourRoutineElement.Resolvables.Add(ourResolvableElement);
            }
            else
            {
                ourResolvableElement.Value = resolvableValue;
            }
        }

        // TODO: understand how SetRawXml works in Unit Test
        //public static void UpdateConfiguration(string fileMapName, string routinesConfigurationSectionXml, string sectionName = key)
        //{
        //    ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
        //    fileMap.ExeConfigFilename = fileMapName;
        //    var systemConfiugration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
        //    //var systemConfiugration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        //    var routinesConfigurationSection = (RoutinesConfigurationSection)ConfigurationManager.GetSection(sectionName);
        //    routinesConfigurationSection.SectionInformation.SetRawXml(routinesConfigurationSectionXml);
        //    systemConfiugration.Save();
        //    ConfigurationManager.RefreshSection(sectionName);
        //}
        [System.Obsolete]
        public static string GetConfigurationXml(string sectionName = key)
        {
            var routinesConfigurationSection = (RoutinesConfigurationSection)ConfigurationManager.GetSection(sectionName);
            return routinesConfigurationSection.GetXml(sectionName);
        }
        [System.Obsolete]
        public static string TimeStamps(string sectionName = key)
        {
            var routinesConfigurationSection = (RoutinesConfigurationSection)ConfigurationManager.GetSection(sectionName);
            return routinesConfigurationSection.ToString();
        }
        [System.Obsolete]
        public static string GetConfigurationManualXml(string sectionName = key)
        {
            var routinesConfigurationSection = (RoutinesConfigurationSection)ConfigurationManager.GetSection(sectionName);
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"<{sectionName}>");
            foreach (RoutineElement routineElement in routinesConfigurationSection.Routines)
            {
                var namespaceAttr = (string.IsNullOrWhiteSpace(routineElement.Namespace)) ? "" : $"namespace=\"{routineElement.Namespace}\"";
                var classAttr = (string.IsNullOrWhiteSpace(routineElement.Type)) ? "" : $"type=\"{routineElement.Type}\"";
                var memberAttr = (string.IsNullOrWhiteSpace(routineElement.Member)) ? "" : $"member=\"{routineElement.Member}\"";
                var forAttr = (string.IsNullOrWhiteSpace(routineElement.For)) ? "" : $"for=\"{routineElement.For}\"";
                stringBuilder.AppendLine($"<{RoutineElementCollection.RoutineElementName} {namespaceAttr} {classAttr} {memberAttr} {forAttr} >");
                foreach (ResolvableElement resolvableElement in routineElement.Resolvables)
                {
                    var namespaceResAttr = (string.IsNullOrWhiteSpace(resolvableElement.Namespace)) ? "" : $"namespace=\"{resolvableElement.Namespace}\"";
                    var typeAttr = $"type=\"{resolvableElement.Type}\"";
                    var valueAttr = $"value=\"{resolvableElement.Value}\"";
                    stringBuilder.AppendLine($"<{ResolvableElementCollection.ResolvableElementName} {namespaceResAttr} {typeAttr} {valueAttr} />");
                }
                stringBuilder.AppendLine($"</{RoutineElementCollection.RoutineElementName}>");
            }
            stringBuilder.AppendLine($"</{sectionName}>");
            var xml = stringBuilder.ToString();
            return xml;
        }
        #endregion
    }
}