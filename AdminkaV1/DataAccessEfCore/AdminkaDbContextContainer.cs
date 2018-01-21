using System;

using DashboardCode.Routines;
using DashboardCode.Routines.Storage.EfCore;
using DashboardCode.AdminkaV1.DataAccessEfCore.InMemory;
using DashboardCode.AdminkaV1.DataAccessEfCore.SqlServer;

namespace DashboardCode.AdminkaV1.DataAccessEfCore
{ 
    public class AdminkaDbContextContainer
    {
        AdminkaStorageConfiguration adminkaStorageConfiguration;
        public AdminkaDbContextContainer(AdminkaStorageConfiguration adminkaStorageConfiguration) =>
            this.adminkaStorageConfiguration = adminkaStorageConfiguration;

        public Func<RoutineClosure<UserContext>, AdminkaDbContext> ResolveAdminkaDbContextConstructor()
        {
            IDbContextOptionsFactory optionsFactory;
            if (adminkaStorageConfiguration.StorageType == StorageType.INMEMORY)
                optionsFactory = new InMemoryAdminkaOptionsFactory("AdminkaV1_InMemmory");
            else
            {
                var connectionString = adminkaStorageConfiguration.ConnectionString;
                var migrationAssembly = adminkaStorageConfiguration.MigrationAssembly;
                optionsFactory = new SqlServerAdminkaOptionsFactory(connectionString, migrationAssembly);
            }
            return (closure) =>
            {
                var loggerProviderConfiguration = closure.Resolve<LoggerProviderConfiguration>();
                var verbose = (loggerProviderConfiguration.Enabled) ? closure.Verbose : null;
                var adminkaDbContext = new AdminkaDbContext((b) => optionsFactory.Create(b), verbose);
                return adminkaDbContext;
            };
        }
    }
}