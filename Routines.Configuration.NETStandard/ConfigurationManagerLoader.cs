using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DashboardCode.Routines.Configuration.NETStandard
{
    public class ConfigurationManagerLoader: IConfigurationManagerLoader
    {
        const string defaultSectionName = "Routines";
        internal readonly List<RoutineResolvable> routineResolvables;
        readonly IConfigurationRoot configurationRoot;
        public ConfigurationManagerLoader(IConfigurationRoot configurationRoot, string sectionName = defaultSectionName)
        {
            this.configurationRoot = configurationRoot;
            var section = configurationRoot.GetSection(sectionName);
            routineResolvables = new List<RoutineResolvable>();
            section.Bind(routineResolvables);
        }

        public IEnumerable<IRoutineConfigurationRecord> RoutineResolvables => 
            routineResolvables;

        public string GetConnectionString(string name = "ConnectionString")
        {
            var connectionString = configurationRoot.GetSection(name).Value;
            return connectionString;
        }
    }
}
