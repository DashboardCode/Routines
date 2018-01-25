using System;

using DashboardCode.Routines;
using DashboardCode.Routines.Storage.EfCore;
using DashboardCode.AdminkaV1.DataAccessEfCore.InMemory;
using DashboardCode.AdminkaV1.DataAccessEfCore.SqlServer;
using DashboardCode.Routines.Storage;

namespace DashboardCode.AdminkaV1.DataAccessEfCore
{
    public class AdminkaDbContextFactory 
    {
        AdminkaStorageConfiguration adminkaStorageConfiguration;
        public AdminkaDbContextFactory(AdminkaStorageConfiguration adminkaStorageConfiguration) =>
            this.adminkaStorageConfiguration = adminkaStorageConfiguration;

        public AdminkaDbContext Create(Action<string> verbose)
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
            var adminkaDbContext = new AdminkaDbContext((b) => optionsFactory.Create(b), verbose);
            return adminkaDbContext;
        }

        public AdminkaDbContext Create(RoutineClosure<UserContext> closure)
        {
            var loggerProviderConfiguration = closure.Resolve<LoggerProviderConfiguration>();
            var verbose = (loggerProviderConfiguration.Enabled) ? closure.Verbose : null;
            var adminkaDbContext = Create(verbose);
            return adminkaDbContext;
        }
    }

    public class RepositoryGFactory : IRepositoryGFactory<AdminkaDbContext>
    {
        public Func<AdminkaDbContext, bool, IRepository<TEntity>> ComposeCreateRepository<TEntity>() where TEntity : class =>
            (adminkaDbContext, noTracking) => new Repository<TEntity>(adminkaDbContext, noTracking);
    }

    public class OrmGFactory : IOrmGFactory<AdminkaDbContext> 
    {
        public Func<AdminkaDbContext, IOrmEntitySchemaAdapter, IOrmEntitySchemaAdapter<TEntity>> ComposeCreateOrmMetaAdapter<TEntity>() where TEntity : class =>
            (adminkaDbContext, ormEntitySchemaAdapter) => new OrmMetaAdapter<TEntity>(adminkaDbContext.Model, ormEntitySchemaAdapter);

        public Func<AdminkaDbContext, Func<Exception, StorageResult>, IAuditVisitor, IOrmStorage<TEntity>> ComposeCreateOrmStorage<TEntity>() where TEntity : class=>
            (adminkaDbContext, analyzeException, auditVisitor) => new OrmStorage<TEntity>(adminkaDbContext, analyzeException, auditVisitor);
    }
}