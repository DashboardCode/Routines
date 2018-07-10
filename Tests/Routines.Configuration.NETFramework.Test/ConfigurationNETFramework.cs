namespace DashboardCode.Routines.Configuration.Classic.Test
{
    public class ConfigurationNETFramework
    {
        ConfigurationManagerLoader configurationManagerLoader = new ConfigurationManagerLoader();

        public ConfigurationContainer Create(MemberTag memberTag) =>
            new ConfigurationContainer(configurationManagerLoader.GetGetRoutineConfigurationRecords(), memberTag);
    }
}