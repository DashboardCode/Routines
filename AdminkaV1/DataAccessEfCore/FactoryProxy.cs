using DashboardCode.Routines;
using DashboardCode.Routines.Storage;
using DashboardCode.AdminkaV1.DataAccessEfCore.Services;
using DashboardCode.Routines.Storage.EfCore;
using System;

namespace DashboardCode.AdminkaV1.DataAccessEfCore
{
    public class FactoryProxy : 
        IOrmHandlerFactory<UserContext>,
        IRepositoryHandlerFactory<UserContext>,
        
        IOrmFactoryFactory<AdminkaDbContext>,
        IRepositoryFactoryFactory<AdminkaDbContext>
    {
        readonly StorageMetaService storageMetaService;
        readonly AdminkaStorageConfiguration adminkaStorageConfiguration;

        public FactoryProxy(AdminkaStorageConfiguration adminkaStorageConfiguration, StorageMetaService storageMetaService)
        {
            this.adminkaStorageConfiguration = adminkaStorageConfiguration;
            this.storageMetaService = storageMetaService;
        }

        public Func<RoutineClosure<UserContext>, AdminkaDbContext> CreateAdminkaDbContextContainer()
        {
            var optionsFactoryContainer = new AdminkaDbContextContainer(adminkaStorageConfiguration);
            return optionsFactoryContainer.ResolveAdminkaDbContextConstructor();
        }

        public AdminkaDbContext CreateAdminkaDbContext(RoutineClosure<UserContext> closure)
        {
            var adminkaDbContextContainer = CreateAdminkaDbContextContainer();
            var adminkaDbContext = adminkaDbContextContainer(closure);
            return adminkaDbContext;
        }

        public OrmDbContextHandlerContainer<UserContext, AdminkaDbContext> CreateAdminkaDbContextHandlerContainer(RoutineClosure<UserContext> closure)
        {
            var adminkaDbContextContainer = CreateAdminkaDbContextContainer();

            Func<object, bool> hasAuditProperties= o => o is IVersioned;
            Action<object> setAuditProperties= (o) =>
            {
                if (o is IVersioned versionedEntity)
                {
                    versionedEntity.RowVersionBy = closure.UserContext.AuditStamp;
                    versionedEntity.RowVersionAt = DateTime.Now;
                }
            };
            var adminkaDbContextHandlerContainer = new OrmDbContextHandlerContainer<UserContext, AdminkaDbContext>(closure, adminkaDbContextContainer, hasAuditProperties, setAuditProperties);
            return adminkaDbContextHandlerContainer;
        }

        public RepositoryDbContextHandlerContainer<UserContext, AdminkaDbContext> CreateRepositoryAdminkaDbContextHandlerContainer(RoutineClosure<UserContext> closure)
        {
            var adminkaDbContextContainer = CreateAdminkaDbContextContainer();
            var adminkaDbContextPrimitiveHandlerContainer = new RepositoryDbContextHandlerContainer<UserContext, AdminkaDbContext>(closure, adminkaDbContextContainer);
            return adminkaDbContextPrimitiveHandlerContainer;
        }

        public AdminkaOrmHandlerFactory CreateAdminkaOrmHandlerFactory(RoutineClosure<UserContext> closure)
        {
            var adminkaDbContextHandlerContainer = CreateAdminkaDbContextHandlerContainer(closure);
            var adminkaDbContextPrimitiveHandlerContainer = CreateRepositoryAdminkaDbContextHandlerContainer(closure);
            var adminkaOrmHandlerFactory = new AdminkaOrmHandlerFactory(
                adminkaDbContextHandlerContainer,  storageMetaService, (IOrmFactoryFactory<AdminkaDbContext>)this);
            return adminkaOrmHandlerFactory;
        }

        public AdminkaRepositoryHandlerFactory CreateAdminkaRepositoryHandlerFactory(RoutineClosure<UserContext> closure)
        {
            var adminkaDbContextHandlerContainer = CreateAdminkaDbContextHandlerContainer(closure);
            var adminkaDbContextPrimitiveHandlerContainer = CreateRepositoryAdminkaDbContextHandlerContainer(closure);
            var adminkaOrmHandlerFactory = new AdminkaRepositoryHandlerFactory(
                adminkaDbContextPrimitiveHandlerContainer, (IRepositoryFactoryFactory<AdminkaDbContext>)this);
            return adminkaOrmHandlerFactory;
        }



        public IOrmHandler<TEntity> CreateAdminkaOrmHandler<TEntity>(RoutineClosure<UserContext> closure) where TEntity : class
        {
            var adminkaOrmHandlerFactory = CreateAdminkaOrmHandlerFactory(closure);
            var adminkaOrmHandler = adminkaOrmHandlerFactory.Create<TEntity>();
            return adminkaOrmHandler;
        }

        public IRepositoryHandler<TEntity> CreateAdminkaRespositoryHandler<TEntity>(RoutineClosure<UserContext> closure) where TEntity : class
        {
            var adminkaRepositoryHandlerFactory = CreateAdminkaRepositoryHandlerFactory(closure);
            var adminkaRepositoryHandler = adminkaRepositoryHandlerFactory.Create<TEntity>();
            return adminkaRepositoryHandler;
        }

        public IOrmFactory<AdminkaDbContext, TEntity> CreateOrmFactory<TEntity>() where TEntity : class
        {
            return new OrmFactory<TEntity>();
        }

        public class OrmFactory<TEntity> : IOrmFactory<AdminkaDbContext, TEntity> where TEntity : class
        {

            public IRepository<TEntity> CreateRepository(AdminkaDbContext dbContext, bool noTracking2) =>
                new Repository<TEntity>(dbContext, noTracking2);

            public IOrmStorage<TEntity> CreateOrmStorage(AdminkaDbContext dbContext, Func<Exception, StorageResult> analyzeException, Func<object, bool> hasAuditProperties, Action<object> setAuditProperties) =>
                new OrmStorage<TEntity>(dbContext, analyzeException, hasAuditProperties, setAuditProperties);

            public IOrmEntitySchemaAdapter<TEntity> CreateOrmMetaAdapter(AdminkaDbContext dbContext, IOrmEntitySchemaAdapter ormEntitySchemaAdapter) =>
                new OrmMetaAdapter<TEntity>(dbContext.Model, ormEntitySchemaAdapter);

        }

        public IRepositoryFactory<AdminkaDbContext, TEntity> CreateRepositoryFactory<TEntity>() where TEntity : class
        {
            return new RepositoryFactory<TEntity>();
        }

        public class RepositoryFactory<TEntity> : IRepositoryFactory<AdminkaDbContext, TEntity> where TEntity : class
        {

            public IRepository<TEntity> CreateRepository(AdminkaDbContext dbContext, bool noTracking2) =>
                new Repository<TEntity>(dbContext, noTracking2);

        }

        //public class AdminkaRepositoryDbContextHandler : RepositoryDbContextHandler<UserContext, AdminkaDbContext>
        //{
        //    public AdminkaRepositoryDbContextHandler(
        //        RoutineClosure<UserContext> closure,
        //        Func<RoutineClosure<UserContext>, AdminkaDbContext> dbContextFactory
        //    ): base(closure, dbContextFactory)
        //    {

        //    }
        //}
    }
}