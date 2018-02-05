
using DashboardCode.Routines.Configuration;
using DashboardCode.AdminkaV1.DataAccessEfCore;

namespace DashboardCode.AdminkaV1.Injected.NETFramework
{
    public class SqlServerAdmikaConfigurationFacade //: IAdminkaConfigurationFacade
    {
        private readonly string connectionStringName;
        readonly IConnectionStringAccess connectionStringAccess;
        public SqlServerAdmikaConfigurationFacade(IConnectionStringAccess connectionStringAccess, string connectionStringName = "adminka")
        {
            this.connectionStringName = connectionStringName;
            this.connectionStringAccess = connectionStringAccess;
        }

        public AdminkaStorageConfiguration ResolveAdminkaStorageConfiguration()
        {
            var connectionString = connectionStringAccess.GetConnectionString(connectionStringName);
            return new AdminkaStorageConfiguration(connectionString, null, StorageType.SQLSERVER);
        }
    }
}