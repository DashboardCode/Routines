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
        IStorageMetaService storageMetaService;
        public ReliantOrmHandlerGFactory(
                IRepositoryGFactory<TDbContext> repositoryGFactory,
                IOrmGFactory<TDbContext> ormGFactory,
                IStorageMetaService storageMetaService,
                IAuditVisitor auditVisitor,
                TDbContext dbContext
            )
        {
            this.dbContext = dbContext;
            this.auditVisitor = auditVisitor;
            this.repositoryGFactory = repositoryGFactory;
            this.ormGFactory = ormGFactory;
            this.storageMetaService = storageMetaService;
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
            var ormStorage = createOrmStorage(dbContext, storageMetaService.Analyze<TEntity>, auditVisitor);
            var ormEntitySchemaAdapter = createOrmMetaAdapter(dbContext, storageMetaService.GetOrmEntitySchemaAdapter<TEntity>());

            var ormHandler = new ReliantOrmHandler<TEntity>(repository, ormStorage, ormEntitySchemaAdapter);
            return ormHandler;
        }
    }
}