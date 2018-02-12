using System;
using System.Threading.Tasks;

using DashboardCode.Routines.Injected;

namespace DashboardCode.Routines.Storage
{
    public class StorageRoutineHandler<TUserContext, TDbContext> : UserRoutineHandler<TUserContext>
        where TDbContext : IDisposable
    {
        readonly IEntityMetaServiceContainer entityMetaServiceContainer;
        readonly Func<TDbContext> createDbContext;
        readonly Func<(TDbContext, IAuditVisitor)> createDbContextForStorage;
        readonly IRepositoryContainer<TDbContext> repositoryGFactory;
        readonly IOrmContainer<TDbContext> ormGFactory;

        protected readonly TUserContext userContext;

        public StorageRoutineHandler(
            TUserContext userContext,

            IEntityMetaServiceContainer entityMetaServiceContainer,
            
            Func<TDbContext> createDbContext,
            Func<(TDbContext, IAuditVisitor)> createDbContextForStorage,
            
            IRepositoryContainer<TDbContext> repositoryGFactory,
            IOrmContainer<TDbContext> ormGFactory,

            IExceptionHandler exceptionHandler,
            IRoutineLogging routineLogging,
            RoutineClosure<TUserContext> closure,
            object input
            ) : base(
                exceptionHandler,
                routineLogging,
                closure,
                new IndependentRepositoryHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, createDbContext), 
                new IndependentOrmHandlerGFactory<TUserContext, TDbContext>(
                        repositoryGFactory, ormGFactory,
                        entityMetaServiceContainer, createDbContextForStorage), 
                input)
        {
            this.userContext = userContext;

            this.entityMetaServiceContainer = entityMetaServiceContainer;

            this.createDbContext    = createDbContext;
            this.createDbContextForStorage = createDbContextForStorage;
            this.repositoryGFactory        = repositoryGFactory;
            this.ormGFactory               = ormGFactory;
        }

        protected IndependentDbContextHandler<TUserContext, TDbContext> CreateDbContextHandler(RoutineClosure<TUserContext> closure)
        {
            var dbContextHandler = new IndependentDbContextHandler<TUserContext, TDbContext>(closure, createDbContext);
            return dbContextHandler;
        }

        private IndependentRepositoryHandler<TUserContext, TDbContext, TEntity> CreateRespositoryHandler<TEntity>(RoutineClosure<TUserContext> closure, bool noTracking = false) where TEntity : class
        {
            Func<TDbContext, bool, IRepository<TEntity>> createRepository = repositoryGFactory.ResolveCreateRepository<TEntity>();
            var repositoryHandler = new IndependentRepositoryHandler<TUserContext, TDbContext, TEntity>(closure, createDbContext, dbContext => createRepository(dbContext, noTracking));
            return repositoryHandler;
        }

        private IndependentOrmHandler<TUserContext, TDbContext, TEntity> CreateOrmHandler<TEntity>(RoutineClosure<TUserContext> closure, bool noTracking = false) where TEntity : class
        {
            var entityStorageMetaService = entityMetaServiceContainer.Resolve<TEntity>();
            IOrmEntitySchemaAdapter ormEntitySchemaAdapter  = entityStorageMetaService.GetOrmEntitySchemaAdapter();
            Func<Exception, StorageResult> analyzeException = entityStorageMetaService.Analyze;
            Func<TDbContext, bool, IRepository<TEntity>> createRepository = null;
            Func<TDbContext,
                 Func<Exception, StorageResult>,
                 IAuditVisitor,
                 IOrmStorage<TEntity>
                 > createOrmStorage = null;
            Func<TDbContext, IOrmEntitySchemaAdapter, IOrmEntitySchemaAdapter<TEntity>> createOrmMetaAdapter = null;
            var ormHandler = new IndependentOrmHandler<TUserContext, TDbContext, TEntity>(
                    closure, createDbContextForStorage, ormEntitySchemaAdapter, analyzeException, createRepository, noTracking, createOrmStorage, createOrmMetaAdapter
                );
            return ormHandler;
        }

        private Action<RoutineClosure<TUserContext>> ComposeDbContextHandled(Action<TDbContext, RoutineClosure<TUserContext>> action)
        {
            return closure =>
            {
                using (var dbContext = createDbContext())
                    action(dbContext, closure);
            };
        }

        private Func<RoutineClosure<TUserContext>, TOutput> ComposeDbContextFuncHandled<TOutput>(Func<TDbContext, RoutineClosure<TUserContext>, TOutput> func)
        {
            return closure =>
            {
                using (var dbContext = createDbContext())
                    return func(dbContext, closure);
            };
        }

        public Action<RoutineClosure<TUserContext>> ComposeDbContextHandled(Action<TDbContext> action)
        {
            return closure =>
            {
                using (var dbContext = createDbContext())
                    action(dbContext);
            };
        }

        public Func<RoutineClosure<TUserContext>, TOutput> ComposeDbContextHandled<TOutput>(Func<TDbContext, TOutput> func)
        {
            return closure =>
            {
                using (var dbContext = createDbContext())
                    return func(dbContext);
            };
        }
        #region  Compose Factory

        public Action<RoutineClosure<TUserContext>> ComposeOrmFactoryHandled(Action<ReliantOrmHandlerGFactory<TUserContext, TDbContext>, RoutineClosure<TUserContext>> action)
        {
            return closure =>
            {
                var (dbContext, auditVisitor) = createDbContextForStorage();
                using (dbContext)
                    action(new ReliantOrmHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, ormGFactory, entityMetaServiceContainer, auditVisitor, dbContext), closure);
            };
        }

        public Func<RoutineClosure<TUserContext>, TOutput> ComposeOrmFactoryHandled<TOutput>(Func<ReliantOrmHandlerGFactory<TUserContext, TDbContext>, RoutineClosure<TUserContext>, TOutput> func)
        {
            return closure =>
            {
                var (dbContext, auditVisitor) = createDbContextForStorage();
                using (dbContext)
                    return func(new ReliantOrmHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, ormGFactory, entityMetaServiceContainer, auditVisitor, dbContext), closure);
            };
        }

        public Action<RoutineClosure<TUserContext>> ComposeRepositoryFactoryHandled(Action<ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>, RoutineClosure<TUserContext>> action)
        {
            return closure =>
            {
                using (var dbContext = createDbContext())
                    action(new ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, dbContext), closure);
            };
        }

        public Func<RoutineClosure<TUserContext>, TOutput> ComposeRepositoryFactoryHandled<TOutput>(Func<ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>, RoutineClosure<TUserContext>, TOutput> func)
        {
            return closure =>
            {
                using (var dbContext = createDbContext())
                    return func(new ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, dbContext), closure);
            };
        }

        public Action<RoutineClosure<TUserContext>> ComposeOrmFactoryHandled(Action<ReliantOrmHandlerGFactory<TUserContext, TDbContext>> action)
        {
            return closure =>
            {
                var (dbContext, auditVisitor) = createDbContextForStorage();
                using (dbContext)
                    action(new ReliantOrmHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, ormGFactory, entityMetaServiceContainer, auditVisitor, dbContext));
            };
        }

        public Func<RoutineClosure<TUserContext>, TOutput> ComposeOrmFactoryHandled<TOutput>(Func<ReliantOrmHandlerGFactory<TUserContext, TDbContext>, TOutput> func)
        {
            return closure =>
            {
                var (dbContext, auditVisitor) = createDbContextForStorage();
                using (dbContext)
                    return func(new ReliantOrmHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, ormGFactory, entityMetaServiceContainer, auditVisitor, dbContext));
            };
        }

        public Action<RoutineClosure<TUserContext>> ComposeRepositoryFactoryHandled(Action<ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>> action)
        {
            return closure =>
            {
                using (var dbContext = createDbContext())
                    action(new ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, dbContext));
            };
        }

        public Func<RoutineClosure<TUserContext>, TOutput> ComposeRepositoryFactoryHandled<TOutput>(Func<ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>, TOutput> func)
        {
            return closure =>
            {
                using (var dbContext = createDbContext())
                    return func(new ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, dbContext));
            };
        }
        #endregion

        #region Handle with AdminkaDbContext
        public void HandleDbContext(Action<TDbContext> action) =>
            Handle(ComposeDbContextHandled(action));

        public TOutput HandleDbContext<TOutput>(Func<TDbContext, TOutput> func) =>
            Handle(ComposeDbContextHandled(func));

        public async Task<TOutput> HandleDbContextAsync<TOutput>(Func<TDbContext, TOutput> func) =>
            await HandleAsync(ComposeDbContextHandled(func));


        public void HandleDbContext(Action<TDbContext, RoutineClosure<TUserContext>> action) =>
            Handle(ComposeDbContextHandled(action));

        public TOutput HandleDbContext<TOutput>(Func<TDbContext, RoutineClosure<TUserContext>, TOutput> func) =>
            Handle(ComposeDbContextFuncHandled(func));

        public async Task<TOutput> HandleDbContextAsync<TOutput>(Func<TDbContext, RoutineClosure<TUserContext>, TOutput> func) =>
            await HandleAsync(ComposeDbContextFuncHandled(func));

        #endregion

        #region Handle with Handler's Factory
        public void HandleOrmFactory(Action<ReliantOrmHandlerGFactory<TUserContext, TDbContext>, RoutineClosure<TUserContext>> action) =>
            Handle(ComposeOrmFactoryHandled(action));

        public TOutput HandleOrmFactory<TOutput>(Func<ReliantOrmHandlerGFactory<TUserContext, TDbContext>, RoutineClosure<TUserContext>, TOutput> func) =>
            Handle(ComposeOrmFactoryHandled(func));

        public async Task<TOutput> HandleOrmFactoryAsync<TOutput>(Func<ReliantOrmHandlerGFactory<TUserContext, TDbContext>, RoutineClosure<TUserContext>, TOutput> func) =>
            await HandleAsync(ComposeOrmFactoryHandled(func));

        public void HandleRepositoryFactory(Action<ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>, RoutineClosure<TUserContext>> action) =>
            Handle(ComposeRepositoryFactoryHandled(action));

        public TOutput HandleRepositoryFactory<TOutput>(Func<ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>, RoutineClosure<TUserContext>, TOutput> func) =>
            Handle(ComposeRepositoryFactoryHandled(func));

        public async Task<TOutput> HandleRepositoryFactoryAsync<TOutput>(Func<ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>, RoutineClosure<TUserContext>, TOutput> func) =>
            await HandleAsync(ComposeRepositoryFactoryHandled(func));

        public void HandleOrmFactory(Action<ReliantOrmHandlerGFactory<TUserContext, TDbContext>> action) =>
            Handle(ComposeOrmFactoryHandled(action));

        public TOutput HandleOrmFactory<TOutput>(Func<ReliantOrmHandlerGFactory<TUserContext, TDbContext>, TOutput> func) =>
            Handle(ComposeOrmFactoryHandled(func));

        public async Task<TOutput> HandleOrmFactoryAsync<TOutput>(Func<ReliantOrmHandlerGFactory<TUserContext, TDbContext>, TOutput> func) =>
            await HandleAsync(ComposeOrmFactoryHandled(func));

        public void HandleRepositoryFactory(Action<ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>> action) =>
            Handle(ComposeRepositoryFactoryHandled(action));

        public TOutput HandleRepositoryFactory<TOutput>(Func<ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>, TOutput> func) =>
            Handle(ComposeRepositoryFactoryHandled(func));

        public async Task<TOutput> HandleRepositoryFactoryAsync<TOutput>(Func<ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>, TOutput> func) =>
            await HandleAsync(ComposeRepositoryFactoryHandled(func));
        #endregion
    }
}