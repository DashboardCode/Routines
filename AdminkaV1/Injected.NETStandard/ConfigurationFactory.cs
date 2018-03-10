using Microsoft.Extensions.Configuration;

using DashboardCode.Routines;
using DashboardCode.Routines.Configuration;
using DashboardCode.Routines.Configuration.NETStandard;

namespace DashboardCode.AdminkaV1.Injected.NETStandard
{
    public class ConfigurationContainerFactory : IConfigurationContainerFactory
    {
        readonly IConfigurationManagerLoader configurationManagerLoader;
        public ConfigurationContainerFactory(IConfigurationManagerLoader configurationManagerLoader) =>
            this.configurationManagerLoader = configurationManagerLoader;

        public ConfigurationContainerFactory()
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            // false indicates that file is not optional, true force reload on change
            configurationBuilder.AddJsonFile("appsettings.json", false, true);
            var configurationRoot = configurationBuilder.Build();
            configurationManagerLoader = new ConfigurationManagerLoader(configurationRoot);
        }
        public ConfigurationContainer Create(MemberTag memberTag, string @for) =>
            new ConfigurationContainer(configurationManagerLoader.GetGetRoutineConfigurationRecords(), memberTag, @for);
    }
}