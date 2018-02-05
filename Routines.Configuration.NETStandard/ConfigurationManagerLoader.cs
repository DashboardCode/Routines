using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace DashboardCode.Routines.Configuration.NETStandard
{
    public class ConfigurationManagerLoader: IConfigurationManagerLoader, IConnectionStringAccess
    {
        const string defaultSectionName = "Routines";
        internal readonly List<RoutineResolvable> routineResolvables;
        readonly IConfigurationRoot configurationRoot;

        public ConfigurationManagerLoader(string sectionName = defaultSectionName)
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json", false, true); // false indicates file is not optional
            this.configurationRoot = configurationBuilder.Build();
            var section = configurationRoot.GetSection(sectionName);
            routineResolvables = new List<RoutineResolvable>();
            section.Bind(routineResolvables);
        }

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
            var password = configurationRoot["AdminkaPassword"];
            var loginName = configurationRoot["AdminkaUserName"];

            return connectionString;
        }
    }
}