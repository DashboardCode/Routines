using DashboardCode.Routines;
using DashboardCode.Routines.Configuration;
using DashboardCode.Routines.Configuration.NETFramework;

namespace DashboardCode.AdminkaV1.Wcf.Messaging
{
    public class WcfApplicationFactory : IApplicationFactory
    {
        readonly AdminkaStorageConfiguration adminkaStorageConfiguration;
        readonly IConfigurationManagerLoader configurationManagerLoader = new ConfigurationManagerLoader();
        public WcfApplicationFactory()
        {
            var connectionString = configurationManagerLoader.GetConnectionString("adminka");
            adminkaStorageConfiguration = new AdminkaStorageConfiguration(connectionString, null, StorageType.INMEMORY);
        }

        public ConfigurationContainer ComposeSpecify(MemberTag memberTag, string @for) =>
            new ConfigurationContainer(configurationManagerLoader, memberTag, @for);

        public AdminkaStorageConfiguration CreateAdminkaStorageConfiguration() =>
            adminkaStorageConfiguration;
    }
}