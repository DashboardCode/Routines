namespace DashboardCode.Routines.Configuration.NETFramework.Test
{
    public class ConfigurationNETFramework
    {
        ConfigurationManagerLoader configurationManagerLoader = new ConfigurationManagerLoader();
        public ConfigurationContainer Create(MemberTag memberTag) =>
            new ConfigurationContainer(configurationManagerLoader, memberTag);
    }
}
