using DashboardCode.AdminkaV1.DataAccessEfCore;

namespace DashboardCode.AdminkaV1.Injected
{
    public class InMemoryAdmikaConfigurationFacade
    {
        readonly string databaseName;
        public InMemoryAdmikaConfigurationFacade(string databaseName) =>
            this.databaseName = databaseName;

        public AdminkaStorageConfiguration ResolveAdminkaStorageConfiguration() =>
            new AdminkaStorageConfiguration(databaseName, null, StorageType.INMEMORY);
    }
}