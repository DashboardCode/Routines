using System;
using System.Text;

using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Storage.EfCore;
using DashboardCode.Routines.Storage.EfCore.Relational.InMemory;
using DashboardCode.Routines.Storage.EfCore.Relational.SqlServer;
using DashboardCode.Routines.Storage.SqlServer;

namespace DashboardCode.AdminkaV1.TestDom.DataAccessEfCore
{
    public static class TestDomDataAccessEfCoreManager
    {
        // just proxy which role is stop DashboardCode.Routines.Storage.EfCore reference propogation to the DashboardCode.AdminkaV1.Injected project
        public static void Analyze(Exception exception, IStorageResultBuilder storageResultBuilder)
           => EfCoreManager.Analyze(exception, storageResultBuilder);

        // another proxy which role is stop DashboardCode.Routines.Storage.EfCore reference propogation to the DashboardCode.AdminkaV1.Injected project
        public static void Append(StringBuilder sb, Exception ex)
           => EfCoreManager.Append(sb, ex);

        public static TestDomDbContext CreateAdminkaDbContext(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            Action<string> verbose = null) {
            IDbContextOptionsFactory optionsFactory;
            if (adminkaStorageConfiguration.StorageType == StorageType.INMEMORY)
                optionsFactory = new InMemoryAdminkaOptionsFactory("AdminkaV1_InMemmory");
            else
            {
                var connectionString = adminkaStorageConfiguration.ConnectionString;
                var migrationAssembly = adminkaStorageConfiguration.MigrationAssembly;
                optionsFactory = new SqlServerAdminkaOptionsFactory(connectionString, migrationAssembly, "TestDomDbContextMigrationHistory", "ef");
            }
            var adminkaDbContext = new TestDomDbContext((b) => optionsFactory.Create(b), verbose);
            return adminkaDbContext;
        }

        public readonly static IEntityMetaServiceContainer TestDomEntityMetaServiceContainer = new EntityMetaServiceContainer(
           (exception, entityType, ormEntitySchemaAdapter, genericErrorField) => StorageResultBuilder.AnalyzeExceptionRecursive(
                 exception, entityType, ormEntitySchemaAdapter, genericErrorField,
                 (ex, storageResultBuilder) => {
                     TestDomDataAccessEfCoreManager.Analyze(ex, storageResultBuilder);
                     // TODO: remove for InMemory
                     SqlServerManager.Analyze(ex, storageResultBuilder);
                 }
           ),
           (modelBuilder) => TestDomDbContext.BuildModel(modelBuilder)
        );
    }
}