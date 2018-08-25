using DashboardCode.Routines.Configuration.Test;

namespace DashboardCode.Routines.Configuration.Classic.Test
{
    public class ConfigurationNETFramework
    {
        ConfigurationManagerLoader configurationManagerLoader = new ConfigurationManagerLoader();
        Deserializer serializer = new Deserializer();

        public ConfigurationContainer<string> Create(MemberTag memberTag) =>
            new ConfigurationContainer<string>(configurationManagerLoader.GetGetRoutineConfigurationRecords(), serializer,  memberTag);
    }
}