using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace DashboardCode.Routines.Configuration.Standard
{
    public class AppSettings : IAppSettings
    {
        readonly IConfiguration configurationRoot;
        public AppSettings(IConfiguration configurationRoot) =>
            this.configurationRoot = configurationRoot;

        public string GetValue(string key)
        {
            var section = configurationRoot.GetSection(key);
            var value = section.Value;
            return value;
        }
    }

    public class ConnectionStringMap : IConnectionStringMap
    {
        readonly IConfiguration configurationRoot;
        public ConnectionStringMap(IConfiguration configurationRoot) =>
            this.configurationRoot = configurationRoot;

        public string GetConnectionString(string name = "ConnectionString")
        {
            var section = configurationRoot.GetSection(name);
            var connectionString = section.Value;
            //var password = configurationRoot["AdminkaPassword"];
            //var loginName = configurationRoot["AdminkaUserName"];
            return connectionString;
        }
    }

    public class ConfigurationManagerLoader: IConfigurationManagerLoader
    {
        const string defaultSectionName = "Routines";
        internal readonly List<RoutineResolvable> routineResolvables;
        readonly IConfiguration configuration;

        public ConfigurationManagerLoader(IConfiguration configuration, string sectionName = defaultSectionName)
        {
            this.configuration = configuration;
            var section = configuration.GetSection(sectionName);
            routineResolvables = new List<RoutineResolvable>();
            section.Bind(routineResolvables);
        }

        public ConfigurationManagerLoader(List<RoutineResolvable> routineResolvables) =>
            this.routineResolvables = routineResolvables;

        public IEnumerable<IRoutineConfigurationRecord> GetGetRoutineConfigurationRecords() =>
            routineResolvables;
    }
}