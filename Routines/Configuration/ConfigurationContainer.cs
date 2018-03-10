using System.Linq;
using System.Collections.Generic;

namespace DashboardCode.Routines.Configuration
{
    public class ConfigurationContainer 
    {
        private readonly List<IResolvableConfigurationRecord> elements = new List<IResolvableConfigurationRecord>();
        public ConfigurationContainer(IEnumerable<IRoutineConfigurationRecord> routineConfigurationRecords, MemberTag memberTag)
        {
            var memberRoutineConfigurationRecords = routineConfigurationRecords.LimitRoutineConfigurationRecords(memberTag);
            foreach (var routineConfigurationRecord in memberRoutineConfigurationRecords)
            {
                if (routineConfigurationRecord.For.IsNullOrWhiteSpaceOrAsterix())
                {
                    foreach (var resolvableConfigurationRecord in routineConfigurationRecord.Resolvables)
                    {
                        if (!elements.Any(e => e.Type == resolvableConfigurationRecord.Type && e.Namespace == resolvableConfigurationRecord.Namespace))
                            elements.Add(resolvableConfigurationRecord);
                    }
                }
            }
        }

        public ConfigurationContainer(IEnumerable<IRoutineConfigurationRecord> routineConfigurationRecords, MemberTag memberTag, string @for)
        {
            var memberRoutineConfigurationRecords = routineConfigurationRecords.LimitRoutineConfigurationRecords(memberTag);
            foreach (var routineConfigurationRecord in memberRoutineConfigurationRecords)
            {
                if (routineConfigurationRecord.For.IsNullOrWhiteSpaceOrAsterix() || routineConfigurationRecord.For == @for)
                {
                    foreach (var resolvableConfigurationRecord in routineConfigurationRecord.Resolvables)
                    {
                        if (!elements.Any(e => e.Type == resolvableConfigurationRecord.Type && e.Namespace == resolvableConfigurationRecord.Namespace))
                            elements.Add(resolvableConfigurationRecord);
                    }
                }
            }
        }

        public string ResolveString<T>()
        {
            var type = typeof(T);
            var typeName = type.Name;
            var typeNamespace = type.Namespace;
            var @value = ResolveString(typeNamespace, typeName);
            return @value;
        }

        public string ResolveString(string typeNamespace, string typeName)
        {
            var @value = default(string);
            var config = elements.FirstOrDefault(e => (e.Namespace == typeNamespace || string.IsNullOrEmpty(e.Namespace)) && e.Type == typeName);
            if (config != null)
                @value = config.Value;
            return @value;
        }
    }
}