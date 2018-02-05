using Microsoft.Extensions.Configuration;

using DashboardCode.Routines;
using DashboardCode.Routines.Configuration;
using DashboardCode.AdminkaV1.DataAccessEfCore;
using System;

namespace DashboardCode.AdminkaV1.Injected.NETStandard
{
    public class SqlServerAdmikaConfigurationFacade 
    {
        public IConfigurationRoot ConfigurationRoot { get; private set; }
        readonly string connectionStringName;
        readonly string migrationAssembly;
        readonly IConnectionStringAccess connectionStringAccess;

        public SqlServerAdmikaConfigurationFacade(
            IConnectionStringAccess connectionStringAccess,
            string connectionStringName = "AdminkaConnectionString", string migrationAssembly = null)
        {
            this.connectionStringAccess = connectionStringAccess;
            this.connectionStringName = connectionStringName;
            this.migrationAssembly = migrationAssembly;
        }

        public AdminkaStorageConfiguration ResolveAdminkaStorageConfiguration()
        {
            var connectionString = connectionStringAccess.GetConnectionString(connectionStringName);
            return new AdminkaStorageConfiguration(connectionString, migrationAssembly, StorageType.SQLSERVER);
        }
    }
}