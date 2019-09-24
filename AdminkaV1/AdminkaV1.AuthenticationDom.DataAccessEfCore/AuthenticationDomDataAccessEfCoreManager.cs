using System;
using System.Text;

using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Storage.EfCore;
using DashboardCode.Routines.Storage.EfCore.Relational.InMemory;
using DashboardCode.Routines.Storage.EfCore.Relational.SqlServer;
using DashboardCode.Routines.Storage.SqlServer;

namespace DashboardCode.AdminkaV1.AuthenticationDom.DataAccessEfCore
{
    public static class AuthenticationDomDataAccessEfCoreManager
    {

        public readonly static IEntityMetaServiceContainer AuthenticationDomEntityMetaServiceContainer = new EntityMetaServiceContainer(
            (exception, entityType, ormEntitySchemaAdapter, genericErrorField) => StorageResultBuilder.AnalyzeExceptionRecursive(
                  exception, entityType, ormEntitySchemaAdapter, genericErrorField,
                  (ex, storageResultBuilder) => {
                      AuthenticationDomDataAccessEfCoreManager.Analyze(ex, storageResultBuilder);
                      // TODO remove it for inMemory
                      SqlServerManager.Analyze(ex, storageResultBuilder);
                  }
            ),
            (modelBuilder) => AuthenticationDomDbContext.BuildModel(modelBuilder)
        );
        // just proxy which role is stop DashboardCode.Routines.Storage.EfCore reference propogation to the DashboardCode.AdminkaV1.Injected project
        public static void Analyze(Exception exception, IStorageResultBuilder storageResultBuilder)
           => EfCoreManager.Analyze(exception, storageResultBuilder);

        // another proxy which role is stop DashboardCode.Routines.Storage.EfCore reference propogation to the DashboardCode.AdminkaV1.Injected project
        public static void Append(StringBuilder sb, Exception ex)
           => EfCoreManager.Append(sb, ex);

        public static AuthenticationDomDbContext CreateAdminkaDbContext(
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            Action<string> verbose = null) {
            IDbContextOptionsFactory optionsFactory;
            if (adminkaStorageConfiguration.StorageType == StorageType.INMEMORY)
                optionsFactory = new InMemoryAdminkaOptionsFactory("AuthenticationDom_InMemmory");
            else
            {
                var connectionString = adminkaStorageConfiguration.ConnectionString;
                var migrationAssembly = adminkaStorageConfiguration.MigrationAssembly;
                optionsFactory = new SqlServerAdminkaOptionsFactory(connectionString, migrationAssembly, "AuthenticationDomDbContextMigrationHistory", "ef");
            }
            var adminkaDbContext = new AuthenticationDomDbContext((b) => optionsFactory.Create(b), verbose);
            return adminkaDbContext;
        }
    }
}