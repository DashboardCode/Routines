namespace DashboardCode.Routines.Configuration
{
    public class ConfigurationContainerFactory
    {
        readonly IConfigurationManagerLoader configurationManagerLoader;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configurationManagerLoader">Abstraction over .NET Classic and .NET Core configuration methods</param>
        public ConfigurationContainerFactory(IConfigurationManagerLoader configurationManagerLoader) =>
            this.configurationManagerLoader = configurationManagerLoader;

        public ConfigurationContainer Create(MemberTag memberTag, string @for) {
            var routineConfigurationRecords = configurationManagerLoader.GetGetRoutineConfigurationRecords();
            return new ConfigurationContainer(routineConfigurationRecords, memberTag, @for);
        }
    }
}