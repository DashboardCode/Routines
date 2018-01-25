using System;

namespace DashboardCode.Routines.Storage
{
    public class DataAccessFacade<TUserContext, TDbContext> where TDbContext : IDisposable
    {
        readonly IStorageMetaService storageMetaService;
        readonly Func<RoutineClosure<TUserContext>, TDbContext> dbContextFactory;
        readonly Func<RoutineClosure<TUserContext>, IAuditVisitor> getAuditVisitor;
        readonly IRepositoryGFactory<TDbContext> repositoryGFactory;
        readonly IOrmGFactory<TDbContext> ormGFactory;

        public readonly IRepositoryHandlerGFactory<TUserContext> RepositoryHandlerFactory;
        public readonly IOrmHandlerGFactory<TUserContext> OrmHandlerFactory;

        public DataAccessFacade(
            IRepositoryGFactory<TDbContext> repositoryGFactory,
            IOrmGFactory<TDbContext> ormGFactory,
            IStorageMetaService storageMetaService, 
            Func<RoutineClosure<TUserContext>, IAuditVisitor> getAuditVisitor,
            Func<RoutineClosure<TUserContext>, TDbContext> dbContextFactory
            )
        {
            this.dbContextFactory = dbContextFactory;
            this.storageMetaService = storageMetaService;
            this.getAuditVisitor = getAuditVisitor;
            this.repositoryGFactory = repositoryGFactory;
            this.ormGFactory = ormGFactory;

            RepositoryHandlerFactory = new IndependentRepositoryHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, dbContextFactory);
            OrmHandlerFactory = new IndependentOrmHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, ormGFactory, storageMetaService, getAuditVisitor, dbContextFactory);
        }
        
        public IndependentDbContextHandler<TUserContext, TDbContext> CreateDbContextHandler(RoutineClosure<TUserContext> closure) 
        {
            var dbContextHandler = new IndependentDbContextHandler<TUserContext, TDbContext>(closure, dbContextFactory);
            return dbContextHandler;
        }

        public IndependentRepositoryHandler<TUserContext, TDbContext, TEntity> CreateRespositoryHandler<TEntity>(RoutineClosure<TUserContext> closure, bool noTracking=false) where TEntity : class
        {
            Func<TDbContext, bool, IRepository<TEntity>> createRepository = repositoryGFactory.ComposeCreateRepository<TEntity>();
            var repositoryHandler = new IndependentRepositoryHandler<TUserContext, TDbContext, TEntity>(closure, dbContextFactory, dbContext=>createRepository(dbContext, noTracking));
            return repositoryHandler;
        }

        public IndependentOrmHandler<TUserContext, TDbContext, TEntity> CreateOrmHandler<TEntity>(RoutineClosure<TUserContext> closure, bool noTracking = false) where TEntity : class
        {
            IOrmEntitySchemaAdapter ormEntitySchemaAdapter = storageMetaService.GetOrmMetaAdapter<TEntity>();
            Func<Exception, StorageResult> analyzeException = storageMetaService.Analyze<TEntity>;
            Func< TDbContext, bool, IRepository< TEntity >> createRepository = null;
            Func< TDbContext,
                 Func<Exception, StorageResult>,
                 IAuditVisitor,
                 IOrmStorage < TEntity >
                 > createOrmStorage = null;
            Func<TDbContext, IOrmEntitySchemaAdapter, IOrmEntitySchemaAdapter<TEntity>> createOrmMetaAdapter = null;
            var auditVisitor = getAuditVisitor(closure);
            var ormHandler = new IndependentOrmHandler<TUserContext, TDbContext, TEntity>(
                    closure, dbContextFactory, auditVisitor, ormEntitySchemaAdapter, analyzeException, createRepository, noTracking, createOrmStorage, createOrmMetaAdapter
                );
            return ormHandler;
        }

        public Action<RoutineClosure<TUserContext>> ComposeDbContextActionHandled(Action<TDbContext, RoutineClosure<TUserContext>> action)
        {
            return closure =>
            {
                var auditVisitor = getAuditVisitor(closure);
                using (var dbContext = dbContextFactory(closure))
                    action(dbContext, closure);
            };
        }



        public Func<RoutineClosure<TUserContext>, TOutput> ComposeDbContextFuncHandled<TOutput>(Func<TDbContext, RoutineClosure<TUserContext>, TOutput> func)
        {
            return closure =>
            {
                var auditVisitor = getAuditVisitor(closure);
                using (var dbContext = dbContextFactory(closure))
                    return func(dbContext, closure);
            };
        }

        public Action<RoutineClosure<TUserContext>> ComposeDbContextActionHandled(Action<TDbContext> action)
        {
            return closure =>
            {
                var auditVisitor = getAuditVisitor(closure);
                using (var dbContext = dbContextFactory(closure))
                    action(dbContext);
            };
        }

        public Func<RoutineClosure<TUserContext>, TOutput> ComposeDbContextFuncHandled<TOutput>(Func<TDbContext, TOutput> func)
        {
            return closure =>
            {
                var auditVisitor = getAuditVisitor(closure);
                using (var dbContext = dbContextFactory(closure))
                    return func(dbContext);
            };
        }
        #region  Compose Factory

        public Action<RoutineClosure<TUserContext>> ComposeOrmFactoryActionHandled(Action<ReliantOrmHandlerGFactory<TUserContext, TDbContext>, RoutineClosure<TUserContext>> action)
        {
            return closure =>
            {
                var auditVisitor= getAuditVisitor(closure);
                using (var dbContext = dbContextFactory(closure))
                    action(new ReliantOrmHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, ormGFactory, storageMetaService, auditVisitor, dbContext), closure);
            };
        }

        public Func<RoutineClosure<TUserContext>, TOutput> ComposeOrmFactoryActionHandled<TOutput>(Func<ReliantOrmHandlerGFactory<TUserContext, TDbContext>, RoutineClosure<TUserContext>, TOutput> func)
        {
            return closure =>
            {
                var auditVisitor = getAuditVisitor(closure);
                using (var dbContext = dbContextFactory(closure))
                    return func(new ReliantOrmHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, ormGFactory, storageMetaService, auditVisitor, dbContext), closure);
            };
        }

        public Action<RoutineClosure<TUserContext>> ComposeRepositoryFactoryActionHandled(Action<ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>, RoutineClosure<TUserContext>> action)
        {
            return closure =>
            {
                var auditVisitor = getAuditVisitor(closure);
                using (var dbContext = dbContextFactory(closure))
                    action(new ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, dbContext), closure);
            };
        }

        public Func<RoutineClosure<TUserContext>, TOutput> ComposeRepositoryFactoryActionHandled<TOutput>(Func<ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>, RoutineClosure<TUserContext>, TOutput> func)
        {
            return closure =>
            {
                var auditVisitor = getAuditVisitor(closure);
                using (var dbContext = dbContextFactory(closure))
                    return func(new ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, dbContext), closure);
            };
        }

        public Action<RoutineClosure<TUserContext>> ComposeOrmFactoryActionHandled(Action<ReliantOrmHandlerGFactory<TUserContext, TDbContext>> action)
        {
            return closure =>
            {
                var auditVisitor = getAuditVisitor(closure);
                using (var dbContext = dbContextFactory(closure))
                    action(new ReliantOrmHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, ormGFactory, storageMetaService, auditVisitor, dbContext));
            };
        }

        public Func<RoutineClosure<TUserContext>, TOutput> ComposeOrmFactoryActionHandled<TOutput>(Func<ReliantOrmHandlerGFactory<TUserContext, TDbContext>, TOutput> func)
        {
            return closure =>
            {
                var auditVisitor = getAuditVisitor(closure);
                using (var dbContext = dbContextFactory(closure))
                    return func(new ReliantOrmHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, ormGFactory, storageMetaService, auditVisitor, dbContext));
            };
        }

        public Action<RoutineClosure<TUserContext>> ComposeRepositoryFactoryActionHandled(Action<ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>> action)
        {
            return closure =>
            {
                var auditVisitor = getAuditVisitor(closure);
                using (var dbContext = dbContextFactory(closure))
                    action(new ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, dbContext));
            };
        }

        public Func<RoutineClosure<TUserContext>, TOutput> ComposeRepositoryFactoryActionHandled<TOutput>(Func<ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>, TOutput> func)
        {
            return closure =>
            {
                var auditVisitor = getAuditVisitor(closure);
                using (var dbContext = dbContextFactory(closure))
                    return func(new ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, dbContext));
            };
        }
        #endregion
    }
}