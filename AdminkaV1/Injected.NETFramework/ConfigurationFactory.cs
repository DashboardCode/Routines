using DashboardCode.Routines;
using DashboardCode.Routines.Configuration;

namespace DashboardCode.AdminkaV1.Injected.NETFramework
{
    public class ConfigurationFactory : IConfigurationFactory
    {
        readonly IConfigurationManagerLoader configurationManagerLoader;

        public ConfigurationFactory(IConfigurationManagerLoader configurationManagerLoader) =>
            this.configurationManagerLoader = configurationManagerLoader;

        public ConfigurationContainer ComposeSpecify(MemberTag memberTag, string @for) =>
          new ConfigurationContainer(configurationManagerLoader, memberTag, @for);
    }
}