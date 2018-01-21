using DashboardCode.Routines;
using DashboardCode.Routines.Configuration;
using DashboardCode.Routines.Configuration.NETFramework;

using DashboardCode.AdminkaV1.DataAccessEfCore;

namespace DashboardCode.AdminkaV1.Injected.NETFramework
{
    public class SqlServerAdmikaConfigurationFacade : IAdmikaConfigurationFacade
    {
        private readonly string connectionStringName;
        readonly IConfigurationManagerLoader configurationManagerLoader;
        public SqlServerAdmikaConfigurationFacade(string connectionStringName = "adminka")
        {
            this.connectionStringName = connectionStringName;
            configurationManagerLoader = new ConfigurationManagerLoader();
        }

        public ConfigurationContainer ComposeSpecify(MemberTag memberTag, string @for) =>
            new ConfigurationContainer(configurationManagerLoader, memberTag, @for);

        public AdminkaStorageConfiguration ResolveAdminkaStorageConfiguration()
        {
            var connectionString = configurationManagerLoader.GetConnectionString(connectionStringName);
            return new AdminkaStorageConfiguration(connectionString, null, StorageType.SQLSERVER);
        }
    }
}