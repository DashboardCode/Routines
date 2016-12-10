using System;
using System.Linq;
using Vse.AdminkaV1.DataAccessEfCore;
using Vse.AdminkaV1.DataAccessEfCore.Services;
using Vse.AdminkaV1.DataAccessEfCore.SqlServer;
using Vse.Routines;
using Vse.Routines.Storage;


namespace Vse.AdminkaV1.Injected
{
    public class DataAccessFactory
    {
        readonly RoutineState<UserContext> state;
        readonly StorageMetaService storageMetaService;
        readonly bool repositoryNoTracking;
        public DataAccessFactory(
            RoutineState<UserContext> state, 
            StorageMetaService storageMetaService,
            bool repositoryNoTracking)
        {
            this.state = state;
            this.storageMetaService = storageMetaService;
            this.repositoryNoTracking = repositoryNoTracking;
        }

        private void SetAudit(object o)
        {
            if (o is IVersionedEntity)
            {
                var versionedEntity = (IVersionedEntity)o;
                versionedEntity.RowVersionBy = state.UserContext.AuditStamp;
                versionedEntity.RowVersionAt = DateTime.Now;
            }
        }

        public AdminkaDbContext CreateAdminkaDbContext()
        {
            var connectionString = storageMetaService.GetConnectionString();
            var migrationAssembly = "Vse.AdminkaV1.DataAccessEfCore.SqlServer.Installer";
            var optionsFactory = new SqlServerAdminkaOptionsFactory(connectionString, migrationAssembly);
            var factory = new DbContextFactory(optionsFactory, state);
            var dbContext = factory.CreateDbContext();
            return dbContext;
        }
        public AdminkaDbContextHandler CreateDbContextHandler()
        {
            var connectionString = storageMetaService.GetConnectionString();
            var migrationAssembly = "Vse.AdminkaV1.DataAccessEfCore.SqlServer.Installer";
            var optionsFactory = new SqlServerAdminkaOptionsFactory(connectionString, migrationAssembly);
            var dbContextManager = new AdminkaDbContextHandler(state, SetAudit, optionsFactory);
            return dbContextManager;
        }

        public IRepositoryHandler<TEntity> CreateRepositoryHandler<TEntity>() where TEntity : class
        {
            var dbContextManager = CreateDbContextHandler();
            var storageModels = storageMetaService.GetStorageModels();
            var storageModel  = storageModels.FirstOrDefault(e=>e.Entity.Namespace+"."+ e.Entity.Name==typeof(TEntity).FullName);
                            
            var repositoryHandler = new RepositoryHandler<TEntity>(
                dbContextManager,
                (ex)=>InjectedManager.Analyze(ex,storageModel),
                repositoryNoTracking
                );
            return repositoryHandler;
        }
    }
}
