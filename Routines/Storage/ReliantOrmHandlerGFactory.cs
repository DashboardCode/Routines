using System;

namespace DashboardCode.Routines.Storage
{
    public class ReliantOrmHandlerGFactory<TUserContext, TDbContext>
        where TDbContext : IDisposable
    {
        TDbContext dbContext;
        IAuditVisitor auditVisitor;

        IRepositoryContainer<TDbContext> repositoryGFactory;
        IOrmContainer<TDbContext> ormGFactory;
        IEntityMetaServiceContainer entityMetaServiceContainer;
        public ReliantOrmHandlerGFactory(
                IRepositoryContainer<TDbContext> repositoryGFactory,
                IOrmContainer<TDbContext> ormGFactory,
                IEntityMetaServiceContainer entityMetaServiceContainer,
                IAuditVisitor auditVisitor,
                TDbContext dbContext
            )
        {
            this.dbContext = dbContext;
            this.auditVisitor = auditVisitor;
            this.repositoryGFactory = repositoryGFactory;
            this.ormGFactory = ormGFactory;
            this.entityMetaServiceContainer = entityMetaServiceContainer;
        }

        public ReliantOrmHandler<TEntity> Create<TEntity>(bool noTracking = false) where TEntity : class
        {
            Func<TDbContext, bool, IRepository<TEntity>> createRepository = repositoryGFactory.ResolveCreateRepository<TEntity>();
            Func<TDbContext,
                Func<Exception, StorageResult>,
                IAuditVisitor,
                IOrmStorage<TEntity>
                > createOrmStorage = ormGFactory.ResolveCreateOrmStorage<TEntity>();
            Func<TDbContext, IOrmEntitySchemaAdapter, IOrmEntitySchemaAdapter<TEntity>> createOrmMetaAdapter = ormGFactory.ResolveCreateOrmMetaAdapter<TEntity>();

            var repository = createRepository(dbContext, noTracking);
            var entityStorageMetaService = entityMetaServiceContainer.Resolve<TEntity>();
            var ormStorage = createOrmStorage(dbContext, entityStorageMetaService.Analyze, auditVisitor);
            var ormEntitySchemaAdapter = createOrmMetaAdapter(dbContext, entityStorageMetaService.GetOrmEntitySchemaAdapter());

            var ormHandler = new ReliantOrmHandler<TEntity>(repository, ormStorage, ormEntitySchemaAdapter);
            return ormHandler;
        }
    }
}