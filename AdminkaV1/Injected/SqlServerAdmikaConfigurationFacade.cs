using DashboardCode.Routines.Configuration;
using DashboardCode.AdminkaV1.DataAccessEfCore;

namespace DashboardCode.AdminkaV1.Injected
{
    public class SqlServerAdmikaConfigurationFacade 
    {
        readonly IConnectionStringMap connectionStringAccess;
        readonly string connectionStringName;
        readonly string migrationAssembly;
        public SqlServerAdmikaConfigurationFacade(
            IConnectionStringMap connectionStringAccess, 
            string connectionStringName = "AdminkaConnectionString",
            string migrationAssembly = null)
        {
            this.connectionStringName = connectionStringName;
            this.connectionStringAccess = connectionStringAccess;
            this.migrationAssembly = migrationAssembly;
        }

        public AdminkaStorageConfiguration ResolveAdminkaStorageConfiguration()
        {
            var connectionString = connectionStringAccess.GetConnectionString(connectionStringName);
            return new AdminkaStorageConfiguration(connectionString, migrationAssembly, StorageType.SQLSERVER);
        }
    }
}