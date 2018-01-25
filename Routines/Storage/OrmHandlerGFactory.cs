using System;

namespace DashboardCode.Routines.Storage
{
    public interface IOrmGFactory<TDbContext>
    {
        Func<TDbContext,
                Func<Exception, StorageResult>,
                IAuditVisitor,
                IOrmStorage<TEntity>
                > ComposeCreateOrmStorage<TEntity>() where TEntity : class;
        Func<TDbContext, IOrmEntitySchemaAdapter, IOrmEntitySchemaAdapter<TEntity>> ComposeCreateOrmMetaAdapter<TEntity>() where TEntity : class;
    }

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

        public ReliantOrmHandler<TEntity> Create<TEntity>(bool noTracking = true) where TEntity : class
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
            var ormEntitySchemaAdapter = createOrmMetaAdapter(dbContext, storageMetaService.GetOrmMetaAdapter<TEntity>());

            var ormHandler = new ReliantOrmHandler<TEntity>(repository, ormStorage, ormEntitySchemaAdapter);
            return ormHandler;
        }
    }

    public interface IOrmHandlerGFactory<TUserContext>
    {
        IIndependentOrmHandler<TUserContext, TEntity> Create<TEntity>(RoutineClosure<TUserContext> closure, bool noTracking = true) where TEntity : class;
    }

    public class IndependentOrmHandlerGFactory<TUserContext, TDbContext> : IOrmHandlerGFactory<TUserContext>
        where TDbContext : IDisposable
    {
        Func<RoutineClosure<TUserContext>, TDbContext> dbContextFactory;
        Func<RoutineClosure<TUserContext>, IAuditVisitor> getAuditVisitor;

        IRepositoryGFactory<TDbContext> repositoryGFactory;
        IOrmGFactory<TDbContext> ormGFactory;
        IStorageMetaService storageMetaService;

        public IndependentOrmHandlerGFactory(
                IRepositoryGFactory<TDbContext> repositoryGFactory,
                IOrmGFactory<TDbContext> ormGFactory,
                IStorageMetaService storageMetaService,
                Func<RoutineClosure<TUserContext>, IAuditVisitor> getAuditVisitor,
                Func<RoutineClosure<TUserContext>, TDbContext> dbContextFactory
            )
        {
            this.dbContextFactory = dbContextFactory;
            this.getAuditVisitor = getAuditVisitor;
            this.repositoryGFactory = repositoryGFactory;
            this.ormGFactory = ormGFactory;
            this.storageMetaService = storageMetaService;
        }

        public IIndependentOrmHandler<TUserContext, TEntity> Create<TEntity>(RoutineClosure<TUserContext> closure, bool noTracking = true) where TEntity : class
        {
            IOrmEntitySchemaAdapter ormEntitySchemaAdapter = storageMetaService.GetOrmMetaAdapter<TEntity>();
            Func<Exception, StorageResult> analyzeException = storageMetaService.Analyze<TEntity> ;
            Func< TDbContext, bool, IRepository< TEntity >> createRepository = repositoryGFactory.ComposeCreateRepository<TEntity>();
            var auditVisitor = getAuditVisitor(closure);
            Func< TDbContext,
                 Func<Exception, StorageResult>,
                 IAuditVisitor,
                 IOrmStorage < TEntity >
                 > createOrmStorage = ormGFactory.ComposeCreateOrmStorage<TEntity>();
            Func<TDbContext, IOrmEntitySchemaAdapter, IOrmEntitySchemaAdapter<TEntity>> createOrmMetaAdapter = ormGFactory.ComposeCreateOrmMetaAdapter<TEntity>();

            var ormHandler = new IndependentOrmHandler<TUserContext, TDbContext, TEntity>(closure, dbContextFactory, auditVisitor, ormEntitySchemaAdapter, analyzeException,
                createRepository, noTracking, createOrmStorage, createOrmMetaAdapter);
            return ormHandler;
        }
    }
}