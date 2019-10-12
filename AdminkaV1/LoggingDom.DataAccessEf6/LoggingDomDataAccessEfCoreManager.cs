using System;
using System.Text;

using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Storage.Ef6;
using DashboardCode.Routines.Storage.SqlServer;

namespace DashboardCode.AdminkaV1.LoggingDom.DataAccessEf6
{
    public static class LoggingDomDataAccessEfCoreManager
    {
        public static IEntityMetaServiceContainer CreateLoggingDomEntityMetaServiceContainer(string connectionString) => new EntityMetaServiceContainer(
            new LoggingDomDbContext("x", null),
            (exception, entityType, ormEntitySchemaAdapter, genericErrorField) => StorageResultBuilder.AnalyzeExceptionRecursive(
                  exception, entityType, ormEntitySchemaAdapter, genericErrorField,
                  (ex, storageResultBuilder) =>
                  {
                      LoggingDomDataAccessEfCoreManager.Analyze(ex, storageResultBuilder);
                      // TODO disable for InMemory
                      SqlServerManager.Analyze(ex, storageResultBuilder);
                  }
            )
            //(entityType) => new SqlServerOrmEntitySchemaAdapter( entityType),
            //(modelBuilder) => LoggingDomDbContext.BuildModel(modelBuilder)
        );

        // just proxy which role is stop DashboardCode.Routines.Storage.EfCore reference propogation to the DashboardCode.AdminkaV1.Injected project
        public static void Analyze(Exception exception, IStorageResultBuilder storageResultBuilder)
           => Ef6Manager.Analyze(exception, storageResultBuilder);

        // another proxy which role is stop DashboardCode.Routines.Storage.EfCore reference propogation to the DashboardCode.AdminkaV1.Injected project
        public static void Append(StringBuilder sb, Exception ex)
           => Ef6Manager.Append(sb, ex);

        public static LoggingDomDbContext CreateDbContext(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            Action<string> verbose = null) {
            string connectionString;
            if (adminkaStorageConfiguration.StorageType == StorageType.INMEMORY)
            {
                throw new NotImplementedException("StorageType.INMEMORY");
            }
            else
            {
                connectionString = adminkaStorageConfiguration.ConnectionString;
                //var migrationAssembly = adminkaStorageConfiguration.MigrationAssembly;
                //optionsFactory = new SqlServerAdminkaOptionsFactory(connectionString, migrationAssembly, "AdminkaDbContextMigrationHistory", "ef");
            }
            var loggingDomDbContext = new LoggingDomDbContext(connectionString, verbose);
            return loggingDomDbContext;
        }
    }
}