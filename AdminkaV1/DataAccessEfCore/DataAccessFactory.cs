using System;
using System.Linq;
using System.Collections.Generic;

using DashboardCode.Routines;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Storage.EfCore;
using DashboardCode.AdminkaV1.DataAccessEfCore;
using DashboardCode.AdminkaV1.DataAccessEfCore.Services;
using DashboardCode.AdminkaV1.DataAccessEfCore.SqlServer;
using DashboardCode.AdminkaV1.DataAccessEfCore.InMemory;

namespace DashboardCode.AdminkaV1.Injected
{
    public class DataAccessFactory
    {
        readonly Routine<UserContext> state;
        readonly StorageMetaService storageMetaService;
        readonly AdminkaStorageConfiguration adminkaStorageConfiguration;
        readonly Func<Exception, StorageModel, List<FieldError>> analyze;
        public DataAccessFactory(
            Routine<UserContext> state,
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            StorageMetaService storageMetaService,
            Func<Exception, StorageModel, List<FieldError>> analyze)
        {
            this.state = state;
            this.adminkaStorageConfiguration = adminkaStorageConfiguration;
            this.storageMetaService = storageMetaService;
            this.analyze = analyze;
        }

        private void SetAuditProperties(object o)
        {
            if (o is IVersioned versionedEntity)
            {
                versionedEntity.RowVersionBy = state.UserContext.AuditStamp;
                versionedEntity.RowVersionAt = DateTime.Now;
            }
        }

        private IDbContextOptionsBuilder CreateDbContextOptionsBuilder()
        {
            IDbContextOptionsBuilder buidler=null;
            if (adminkaStorageConfiguration.StorageType== StorageType.INMEMORY)
            {
                buidler = new InMemoryAdminkaOptionsBuilder("AdminkaV1_InMemmory");
            }
            else
            {
                var connectionString  = adminkaStorageConfiguration.ConnectionString;
                var migrationAssembly = adminkaStorageConfiguration.MigrationAssembly;
                buidler = new SqlServerAdminkaOptionsBuilder(connectionString, migrationAssembly);
            }
            return buidler;
        }

        public AdminkaDbContext CreateAdminkaDbContext()
        {
            var optionsBuilder = CreateDbContextOptionsBuilder();
            var dbContextFactory = new AdminkaDbContextFactory(optionsBuilder, state);
            var dbContext = dbContextFactory.CreateAdminkaDbContext();
            return dbContext;
        }

        public AdminkaDbContextHandler CreateDbContextHandler()
        {
            var optionsBuilder = CreateDbContextOptionsBuilder();
            var dbContextHandler = new AdminkaDbContextHandler(state, SetAuditProperties, optionsBuilder);
            return dbContextHandler;
        }

        public IRepositoryHandler<TEntity> CreateRepositoryHandler<TEntity>(bool noTracking = true) where TEntity : class
        {
            var dbContextHandler = CreateDbContextHandler();
            var storageModels = storageMetaService.GetStorageModels();
            var storageModel  = storageModels.FirstOrDefault(e=>e.Entity.Namespace+"."+ e.Entity.Name==typeof(TEntity).FullName);

            var repositoryHandler = new RepositoryHandler<TEntity>(
                dbContextHandler,
                ex=> (storageModel==null)?new List<FieldError>():analyze(ex,storageModel),
                noTracking
            );
            return repositoryHandler;
        }

        #region IRepositoryHandler<TEntity> in place
        public void Handle<TEntity>(Action<IRepository<TEntity>> action, bool noTracking = true) where TEntity : class
        {
            var handler = CreateRepositoryHandler<TEntity>(noTracking);
            handler.Handle(action);
        }

        public void Handle<TEntity>(Action<IRepository<TEntity>, IOrmStorage<TEntity>> action, bool noTracking = true) where TEntity : class
        {
            var handler = CreateRepositoryHandler<TEntity>(noTracking);
            handler.Handle(action);
        }

        public void Handle<TEntity>(Action<IRepository<TEntity>, IOrmStorage<TEntity>, IModel<TEntity>> action, bool noTracking = true) where TEntity : class
        {
            var handler = CreateRepositoryHandler<TEntity>(noTracking);
            handler.Handle(action);
        }

        //TOutput Handle<TOutput>(Func<IRepository<TEntity>, TOutput> func);
        //Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, TOutput> func);

        //TOutput Handle<TOutput>(Func<IRepository<TEntity>, IStorage<TEntity>, TOutput> func);
        //Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, IStorage<TEntity>, TOutput> func);
        #endregion
    }
}