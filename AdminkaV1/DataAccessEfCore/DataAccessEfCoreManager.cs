using System;
using System.Text;

using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Storage.EfCore;
using DashboardCode.AdminkaV1.DataAccessEfCore.InMemory;
using DashboardCode.AdminkaV1.DataAccessEfCore.SqlServer;

namespace DashboardCode.AdminkaV1.DataAccessEfCore
{
    public static class DataAccessEfCoreManager
    {
        // just proxy which role is stop DashboardCode.Routines.Storage.EfCore reference propogation to the DashboardCode.AdminkaV1.Injected project
        public static void Analyze(Exception exception, IStorageResultBuilder storageResultBuilder)
           => EfCoreManager.Analyze(exception, storageResultBuilder);

        // another proxy which role is stop DashboardCode.Routines.Storage.EfCore reference propogation to the DashboardCode.AdminkaV1.Injected project
        public static void Append(StringBuilder sb, Exception ex)
           => EfCoreManager.Append(sb, ex);

        public static AdminkaDbContext CreateAdminkaDbContext(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            Action<string> verbose = null) {
            IDbContextOptionsFactory optionsFactory;
            if (adminkaStorageConfiguration.StorageType == StorageType.INMEMORY)
                optionsFactory = new InMemoryAdminkaOptionsFactory("AdminkaV1_InMemmory");
            else
            {
                var connectionString = adminkaStorageConfiguration.ConnectionString;
                var migrationAssembly = adminkaStorageConfiguration.MigrationAssembly;
                optionsFactory = new SqlServerAdminkaOptionsFactory(connectionString, migrationAssembly);
            }
            var adminkaDbContext = new AdminkaDbContext((b) => optionsFactory.Create(b), verbose);
            return adminkaDbContext;
        }
    }
}