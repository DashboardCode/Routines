using System.Collections.Generic;
using DashboardCode.Routines;
using DashboardCode.Routines.Configuration;

namespace DashboardCode.AdminkaV1.Injected.NETFramework
{
    public class ConfigurationContainerFactory : IConfigurationContainerFactory
    {
        readonly IEnumerable<IRoutineConfigurationRecord> routineConfigurationRecords;

        public ConfigurationContainerFactory(IConfigurationManagerLoader configurationManagerLoader) =>
            this.routineConfigurationRecords = configurationManagerLoader.GetGetRoutineConfigurationRecords();

        public ConfigurationContainerFactory(IEnumerable<IRoutineConfigurationRecord> routineConfigurationRecords) =>
            this.routineConfigurationRecords = routineConfigurationRecords;

        public ConfigurationContainer Create(MemberTag memberTag, string @for) =>
          new ConfigurationContainer(routineConfigurationRecords, memberTag, @for);
    }
}