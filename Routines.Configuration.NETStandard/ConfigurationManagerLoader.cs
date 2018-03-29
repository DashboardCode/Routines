using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace DashboardCode.Routines.Configuration.NETStandard
{
    public class AppSettings : IAppSettings
    {
        const string defaultSectionName = "AppSettings";
        readonly IConfigurationRoot configurationRoot;
        readonly Dictionary<string, string> appSettings;
        public AppSettings(IConfigurationRoot configurationRoot, string sectionName = defaultSectionName)
        {
            this.configurationRoot = configurationRoot;
            var section = configurationRoot.GetSection(sectionName);
            appSettings = new Dictionary<string,string>();
            section.Bind(appSettings);
        }

        public string GetValue(string key)
        {
            return appSettings[key];
        }
    }

    public class ConnectionStringMap : IConnectionStringMap
    {
        readonly IConfigurationRoot configurationRoot;
        public ConnectionStringMap(IConfigurationRoot configurationRoot) =>
            this.configurationRoot = configurationRoot;

        public string GetConnectionString(string name = "ConnectionString")
        {
            var connectionString = configurationRoot.GetSection(name).Value;
            var password = configurationRoot["AdminkaPassword"];
            var loginName = configurationRoot["AdminkaUserName"];
            return connectionString;
        }
    }

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

        public IEnumerable<IRoutineConfigurationRecord> GetGetRoutineConfigurationRecords()
        {
            return routineResolvables;
        }
    }
}