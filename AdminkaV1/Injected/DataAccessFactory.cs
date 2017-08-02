using System;
using System.Linq;
using DashboardCode.AdminkaV1.DataAccessEfCore;
using DashboardCode.AdminkaV1.DataAccessEfCore.Services;
using DashboardCode.AdminkaV1.DataAccessEfCore.SqlServer;
using DashboardCode.AdminkaV1.DataAccessEfCore.InMemory;
using DashboardCode.Routines;
using DashboardCode.Routines.Storage;

namespace DashboardCode.AdminkaV1.Injected
{
    public class DataAccessFactory
    {
        readonly Routine<UserContext> state;
        readonly StorageMetaService storageMetaService;
        public DataAccessFactory(
            Routine<UserContext> state, 
            StorageMetaService storageMetaService)
        {
            this.state = state;
            this.storageMetaService = storageMetaService;
        }

        private void SetAudit(object o)
        {
            if (o is IVersioned versionedEntity)
            {
                versionedEntity.RowVersionBy = state.UserContext.AuditStamp;
                versionedEntity.RowVersionAt = DateTime.Now;
            }
        }

        private IAdminkaOptionsFactory CreateAdminkaOptionsFactory(StorageMetaService storageMetaService)
        {
            IAdminkaOptionsFactory optionsFactory=null;
            if (storageMetaService.GetStorageType()== StorageType.INMEMORY)
            {
                optionsFactory = new InMemoryAdminkaOptionsFactory("AdminkaV1_InMemmory");
            }
            else
            {
                var connectionString  = storageMetaService.GetConnectionString();
                var migrationAssembly = storageMetaService.GetMigrationAssembly();
                optionsFactory = new SqlServerAdminkaOptionsFactory(connectionString, migrationAssembly);
            }
            return optionsFactory;
        }

        public AdminkaDbContext CreateAdminkaDbContext()
        {
            var optionsFactory = CreateAdminkaOptionsFactory(storageMetaService);
            var dbContextFactory = new AdminkaDbContextFactory(optionsFactory, state);
            var dbContext = dbContextFactory.CreateAdminkaDbContext();
            return dbContext;
        }

        public AdminkaDbContextHandler CreateDbContextHandler()
        {
            var optionsFactory = CreateAdminkaOptionsFactory(storageMetaService);
            var dbContextHandler = new AdminkaDbContextHandler(state, SetAudit, optionsFactory);
            return dbContextHandler;
        }

        public IRepositoryHandler<TEntity> CreateRepositoryHandler<TEntity>(bool noTracking = true) where TEntity : class
        {
            var dbContextHandler = CreateDbContextHandler();
            var storageModels = storageMetaService.GetStorageModels();
            var storageModel  = storageModels.FirstOrDefault(e=>e.Entity.Namespace+"."+ e.Entity.Name==typeof(TEntity).FullName);
                            
            var repositoryHandler = new RepositoryHandler<TEntity>(
                dbContextHandler,
                ex=>InjectedManager.Analyze(ex,storageModel),
                noTracking
            );
            return repositoryHandler;
        }
    }
}
