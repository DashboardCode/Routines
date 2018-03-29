using System.Collections.Generic;

namespace DashboardCode.Routines.Configuration
{
    public class ConfigurationContainerFactory
    {
        readonly IConfigurationManagerLoader configurationManagerLoader;
        public ConfigurationContainerFactory(IConfigurationManagerLoader configurationManagerLoader) =>
            this.configurationManagerLoader = configurationManagerLoader;

        public ConfigurationContainer Create(MemberTag memberTag, string @for) =>
            new ConfigurationContainer(configurationManagerLoader.GetGetRoutineConfigurationRecords(), memberTag, @for);
    }
}