using System;
using System.Text;

using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Storage.EfCore;
using DashboardCode.Routines.Storage.EfCore.Relational.InMemory;
using DashboardCode.Routines.Storage.EfCore.Relational.SqlServer;
using DashboardCode.Routines.Storage.SqlServer;

namespace DashboardCode.AdminkaV1.LoggingDom.DataAccessEfCore
{
    public static class LoggingDomDataAccessEfCoreManager
    {
        public readonly static IEntityMetaServiceContainer LoggingDomEntityMetaServiceContainer = new EntityMetaServiceContainer(
            (exception, entityType, ormEntitySchemaAdapter, genericErrorField) => StorageResultBuilder.AnalyzeExceptionRecursive(
                  exception, entityType, ormEntitySchemaAdapter, genericErrorField,
                  (ex, storageResultBuilder) => {
                      LoggingDomDataAccessEfCoreManager.Analyze(ex, storageResultBuilder);
                      // TODO disable for InMemory
                      SqlServerManager.Analyze(ex, storageResultBuilder);
                  }
            ),
            (modelBuilder) => LoggingDomDbContext.BuildModel(modelBuilder)
        );

        // just proxy which role is stop DashboardCode.Routines.Storage.EfCore reference propogation to the DashboardCode.AdminkaV1.Injected project
        public static void Analyze(Exception exception, IStorageResultBuilder storageResultBuilder)
           => EfCoreManager.Analyze(exception, storageResultBuilder);

        // another proxy which role is stop DashboardCode.Routines.Storage.EfCore reference propogation to the DashboardCode.AdminkaV1.Injected project
        public static void Append(StringBuilder sb, Exception ex)
           => EfCoreManager.Append(sb, ex);

        public static LoggingDomDbContext CreateLoggingDomDbContext(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            Action<string> verbose = null) {
            IDbContextOptionsFactory optionsFactory;
            if (adminkaStorageConfiguration.StorageType == StorageType.INMEMORY)
                optionsFactory = new InMemoryAdminkaOptionsFactory("AdminkaV1_InMemmory");
            else
            {
                var connectionString = adminkaStorageConfiguration.ConnectionString;
                var migrationAssembly = adminkaStorageConfiguration.MigrationAssembly;
                optionsFactory = new SqlServerAdminkaOptionsFactory(connectionString, migrationAssembly, "AdminkaDbContextMigrationHistory", "ef");
            }
            var loggingDomDbContext = new LoggingDomDbContext((b) => optionsFactory.Create(b), verbose);
            return loggingDomDbContext;
        }
    }
}