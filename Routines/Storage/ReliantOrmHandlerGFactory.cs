using System;

namespace DashboardCode.Routines.Storage
{
    public class ReliantOrmHandlerGFactory<TUserContext, TDbContext>
        where TDbContext : IDisposable
    {
        TDbContext dbContext;
        IAuditVisitor auditVisitor;

        IRepositoryGFactory<TDbContext> repositoryGFactory;
        IOrmGFactory<TDbContext> ormGFactory;
        IEntityMetaServiceContainer entityMetaServiceContainer;
        public ReliantOrmHandlerGFactory(
                IRepositoryGFactory<TDbContext> repositoryGFactory,
                IOrmGFactory<TDbContext> ormGFactory,
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
            Func<TDbContext, bool, IRepository<TEntity>> createRepository = repositoryGFactory.ComposeCreateRepository<TEntity>();
            Func<TDbContext,
                Func<Exception, StorageResult>,
                IAuditVisitor,
                IOrmStorage<TEntity>
                > createOrmStorage = ormGFactory.ComposeCreateOrmStorage<TEntity>();
            Func<TDbContext, IOrmEntitySchemaAdapter, IOrmEntitySchemaAdapter<TEntity>> createOrmMetaAdapter = ormGFactory.ComposeCreateOrmMetaAdapter<TEntity>();

            var repository = createRepository(dbContext, noTracking);
            var entityStorageMetaService = entityMetaServiceContainer.Resolve<TEntity>();
            var ormStorage = createOrmStorage(dbContext, entityStorageMetaService.Analyze, auditVisitor);
            var ormEntitySchemaAdapter = createOrmMetaAdapter(dbContext, entityStorageMetaService.GetOrmEntitySchemaAdapter());

            var ormHandler = new ReliantOrmHandler<TEntity>(repository, ormStorage, ormEntitySchemaAdapter);
            return ormHandler;
        }
    }
}