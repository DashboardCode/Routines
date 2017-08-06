using DashboardCode.Routines.Configuration;
using DashboardCode.Routines.Configuration.NETFramework;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.Injected.InMemory.NETFramework.Test
{
    public class ApplicationFactory : IApplicationFactory
    {
        readonly string databaseName;
        readonly IConfigurationManagerLoader configurationManagerLoader;
        public ApplicationFactory(string databaseName)
        {
            this.databaseName = databaseName;
            configurationManagerLoader = new ConfigurationManagerLoader();
        }

        public ConfigurationContainer ComposeSpecify(MemberTag memberTag, string @for) =>
            new ConfigurationContainer(configurationManagerLoader, memberTag, @for);

        public AdminkaStorageConfiguration CreateAdminkaStorageConfiguration() =>
            new AdminkaStorageConfiguration(databaseName, null, StorageType.INMEMORY);
    }
}