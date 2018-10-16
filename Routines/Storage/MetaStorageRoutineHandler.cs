using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines.Storage
{
    public class MetaStorageRoutineHandler<TUserContext, TDbContext> : StorageRoutineHandler<TUserContext>
        where TDbContext : IDisposable
    {
        readonly IEntityMetaServiceContainer entityMetaServiceContainer;
        readonly Func<TDbContext> createDbContext;
        readonly Func<(TDbContext, IAuditVisitor)> createDbContextForStorage;
        readonly IRepositoryContainer<TDbContext> repositoryGFactory;
        readonly IOrmContainer<TDbContext> ormGFactory;

        public MetaStorageRoutineHandler(
            IEntityMetaServiceContainer entityMetaServiceContainer,
            
            Func<TDbContext> createDbContext,
            Func<(TDbContext, IAuditVisitor)> createDbContextForStorage,
            
            IRepositoryContainer<TDbContext> repositoryGFactory,
            IOrmContainer<TDbContext> ormGFactory,

            IHandler<RoutineClosure<TUserContext>> routineHandler
            ) : base(
                new IndependentRepositoryHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, createDbContext), 
                new IndependentOrmHandlerGFactory<TUserContext, TDbContext>(
                        repositoryGFactory, ormGFactory,
                        entityMetaServiceContainer, createDbContextForStorage),
                routineHandler)
        {
            this.entityMetaServiceContainer = entityMetaServiceContainer;
            this.createDbContext            = createDbContext;
            this.createDbContextForStorage  = createDbContextForStorage;
            this.repositoryGFactory         = repositoryGFactory;
            this.ormGFactory                = ormGFactory;
        }

        public RoutineDisposeHandler<TDbContext, TUserContext> CreateDbContextHandler(RoutineClosure<TUserContext> closure)
        {
            var dbContextHandler = new RoutineDisposeHandler<TDbContext, TUserContext>(createDbContext, closure);
            return dbContextHandler;
        }

        #region Handle with AdminkaDbContext
        public void HandleDbContext(Action<TDbContext> action) =>
            Handle(closure => {
                using (var dbContext = createDbContext())
                    action(dbContext);
            });

        public TOutput HandleDbContext<TOutput>(Func<TDbContext, TOutput> func) =>
            Handle(closure =>
            {
                using (var dbContext = createDbContext())
                    return func(dbContext);
            });

        public Task<TOutput> HandleDbContextAsync<TOutput>(Func<TDbContext, Task<TOutput>> func) =>
            HandleAsync(async closure =>
            {
                using (var dbContext = createDbContext())
                    return await func(dbContext);
            });

        public Task HandleDbContextAsync(Func<TDbContext, Task> func) =>
            HandleAsync(async closure =>
            {
                using (var dbContext = createDbContext())
                    await func(dbContext);
            });


        public void HandleDbContext(Action<TDbContext, RoutineClosure<TUserContext>> action) =>
            Handle( closure => {
                using (var dbContext = createDbContext())
                    action(dbContext, closure);
            });

        public TOutput HandleDbContext<TOutput>(Func<TDbContext, RoutineClosure<TUserContext>, TOutput> func) =>
            Handle(closure =>{
                using (var dbContext = createDbContext())
                    return func(dbContext, closure);
            });

        public Task<TOutput> HandleDbContextAsync<TOutput>(Func<TDbContext, RoutineClosure<TUserContext>, Task<TOutput>> func) =>
            HandleAsync(async closure =>
            {
                using (var dbContext = createDbContext())
                    return await func(dbContext, closure);
            });

        public Task HandleDbContextAsync(Func<TDbContext, RoutineClosure<TUserContext>, Task> func) =>
            HandleAsync(async closure =>
            {
                using (var dbContext = createDbContext())
                    await func(dbContext, closure);
            });
        #endregion

        #region Handle with Handler's Factory
        public void HandleOrmFactory(Action<ReliantOrmHandlerGFactory<TUserContext, TDbContext>, RoutineClosure<TUserContext>> action) =>
            Handle(closure =>
            {
                var (dbContext, auditVisitor) = createDbContextForStorage();
                using (dbContext)
                    action(new ReliantOrmHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, ormGFactory, entityMetaServiceContainer, auditVisitor, dbContext), closure);
            });

        public TOutput HandleOrmFactory<TOutput>(Func<ReliantOrmHandlerGFactory<TUserContext, TDbContext>, RoutineClosure<TUserContext>, TOutput> func) =>
            Handle(closure =>
            {
                var (dbContext, auditVisitor) = createDbContextForStorage();
                using (dbContext)
                    return func(new ReliantOrmHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, ormGFactory, entityMetaServiceContainer, auditVisitor, dbContext), closure);
            });

        public Task<TOutput> HandleOrmFactoryAsync<TOutput>(Func<ReliantOrmHandlerGFactory<TUserContext, TDbContext>, RoutineClosure<TUserContext>, Task<TOutput>> func) =>
            HandleAsync(async closure =>
            {
                var (dbContext, auditVisitor) = createDbContextForStorage();
                using (dbContext)
                    return await func(new ReliantOrmHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, ormGFactory, entityMetaServiceContainer, auditVisitor, dbContext), closure);
            });

        public Task HandleOrmFactoryAsync(Func<ReliantOrmHandlerGFactory<TUserContext, TDbContext>, RoutineClosure<TUserContext>, Task> func) =>
            HandleAsync(async closure =>
            {
                var (dbContext, auditVisitor) = createDbContextForStorage();
                using (dbContext)
                    await func(new ReliantOrmHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, ormGFactory, entityMetaServiceContainer, auditVisitor, dbContext), closure);
            });

        public void HandleRepositoryFactory(Action<ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>, RoutineClosure<TUserContext>> action) =>
            Handle(closure =>
            {
                using (var dbContext = createDbContext())
                    action(new ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, dbContext), closure);
            });

        public TOutput HandleRepositoryFactory<TOutput>(Func<ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>, RoutineClosure<TUserContext>, TOutput> func) =>
            Handle(closure =>
            {
                using (var dbContext = createDbContext())
                    return func(new ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, dbContext), closure);
            });

        public Task<TOutput> HandleRepositoryFactoryAsync<TOutput>(Func<ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>, RoutineClosure<TUserContext>, Task<TOutput>> func) =>
            HandleAsync(async closure =>
            {
                using (var dbContext = createDbContext())
                    return await func(new ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, dbContext), closure);
            });

        public Task HandleRepositoryFactoryAsync(Func<ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>, RoutineClosure<TUserContext>, Task> func) =>
            HandleAsync(async closure =>
            {
                using (var dbContext = createDbContext())
                    await func(new ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, dbContext), closure);
            });

        public void HandleOrmFactory(Action<ReliantOrmHandlerGFactory<TUserContext, TDbContext>> action) =>
            Handle(closure =>
            {
                var (dbContext, auditVisitor) = createDbContextForStorage();
                using (dbContext)
                    action(new ReliantOrmHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, ormGFactory, entityMetaServiceContainer, auditVisitor, dbContext));
            });

        public TOutput HandleOrmFactory<TOutput>(Func<ReliantOrmHandlerGFactory<TUserContext, TDbContext>, TOutput> func) =>
            Handle(closure =>
            {
                var (dbContext, auditVisitor) = createDbContextForStorage();
                using (dbContext)
                    return func(new ReliantOrmHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, ormGFactory, entityMetaServiceContainer, auditVisitor, dbContext));
            });

        public Task<TOutput> HandleOrmFactoryAsync<TOutput>(Func<ReliantOrmHandlerGFactory<TUserContext, TDbContext>, Task<TOutput>> func) =>
            HandleAsync(async closure =>
            {
                var (dbContext, auditVisitor) = createDbContextForStorage();
                using (dbContext)
                    return await func(new ReliantOrmHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, ormGFactory, entityMetaServiceContainer, auditVisitor, dbContext));
            });

        public Task HandleOrmFactoryAsync(Func<ReliantOrmHandlerGFactory<TUserContext, TDbContext>, Task> func) =>
            HandleAsync(async closure =>
            {
                var (dbContext, auditVisitor) = createDbContextForStorage();
                using (dbContext)
                    await func(new ReliantOrmHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, ormGFactory, entityMetaServiceContainer, auditVisitor, dbContext));
            });


        public void HandleRepositoryFactory(Action<ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>> action) =>
            Handle(closure =>
            {
                using (var dbContext = createDbContext())
                    action(new ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, dbContext));
            });

        public TOutput HandleRepositoryFactory<TOutput>(Func<ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>, TOutput> func) =>
            Handle(closure =>
            {
                using (var dbContext = createDbContext())
                    return func(new ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, dbContext));
            });

        public Task HandleRepositoryFactoryAsync<TOutput>(Func<ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>, Task> func) =>
            HandleAsync(async closure =>
            {
                using (var dbContext = createDbContext())
                    await func(new ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, dbContext));
            });
        #endregion
    }

    public class MetaStorageHandler<TUserContext, TDbContext> : StorageHandler<TUserContext>
        where TDbContext : IDisposable
    {
        readonly IEntityMetaServiceContainer entityMetaServiceContainer;
        readonly Func<TDbContext> createDbContext;
        readonly Func<(TDbContext, IAuditVisitor)> createDbContextForStorage;
        readonly IRepositoryContainer<TDbContext> repositoryGFactory;
        readonly IOrmContainer<TDbContext> ormGFactory;
        readonly RoutineClosure<TUserContext> closure;

        public MetaStorageHandler(
            IEntityMetaServiceContainer entityMetaServiceContainer,

            Func<TDbContext> createDbContext,
            Func<(TDbContext, IAuditVisitor)> createDbContextForStorage,

            IRepositoryContainer<TDbContext> repositoryGFactory,
            IOrmContainer<TDbContext> ormGFactory,

            RoutineClosure<TUserContext> closure
            ) : base(
                new IndependentRepositoryHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, createDbContext),
                new IndependentOrmHandlerGFactory<TUserContext, TDbContext>(
                        repositoryGFactory, ormGFactory,
                        entityMetaServiceContainer, createDbContextForStorage),
                closure)
        {
            this.entityMetaServiceContainer = entityMetaServiceContainer;
            this.createDbContext = createDbContext;
            this.createDbContextForStorage = createDbContextForStorage;
            this.repositoryGFactory = repositoryGFactory;
            this.ormGFactory = ormGFactory;
            this.closure = closure;
        }
        
        #region Handle with AdminkaDbContext
        public void HandleDbContext(Action<TDbContext> action) =>
            Handle(closure =>
            {
                using (var dbContext = createDbContext())
                    action(dbContext);
            });

        public TOutput HandleDbContext<TOutput>(Func<TDbContext, TOutput> func) =>
            Handle(closure =>
            {
                using (var dbContext = createDbContext())
                    return func(dbContext);
            });

        public Task<TOutput> HandleDbContextAsync<TOutput>(Func<TDbContext, Task<TOutput>> func) =>
            HandleAsync(async closure =>
            {
                using (var dbContext = createDbContext())
                    return await func(dbContext);
            });

        public Task HandleDbContextAsync(Func<TDbContext, Task> action) =>
            HandleAsync(async closure =>
            {
                using (var dbContext = createDbContext())
                    await action(dbContext);
            });


        public void HandleDbContext(Action<TDbContext, RoutineClosure<TUserContext>> action) =>
            Handle(closure =>
            {
                using (var dbContext = createDbContext())
                    action(dbContext, closure);
            });

        public TOutput HandleDbContext<TOutput>(Func<TDbContext, RoutineClosure<TUserContext>, TOutput> func) =>
            Handle(closure =>
            {
                using (var dbContext = createDbContext())
                    return func(dbContext, closure);
            });

        public Task<TOutput> HandleDbContextAsync<TOutput>(Func<TDbContext, RoutineClosure<TUserContext>, Task<TOutput>> func) =>
            HandleAsync(async closure =>
            {
                using (var dbContext = createDbContext())
                    return await func(dbContext, closure);
            });

        public Task HandleDbContextAsync(Func<TDbContext, RoutineClosure<TUserContext>, Task> action) =>
            HandleAsync(async closure =>
            {
                using (var dbContext = createDbContext())
                    await action(dbContext, closure);
            });
        #endregion

        #region Handle with Handler's Factory
        public void HandleOrmFactory(Action<ReliantOrmHandlerGFactory<TUserContext, TDbContext>, RoutineClosure<TUserContext>> action) =>
            Handle(closure =>
            {
                var (dbContext, auditVisitor) = createDbContextForStorage();
                using (dbContext)
                    action(new ReliantOrmHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, ormGFactory, entityMetaServiceContainer, auditVisitor, dbContext), closure);
            });

        public TOutput HandleOrmFactory<TOutput>(Func<ReliantOrmHandlerGFactory<TUserContext, TDbContext>, RoutineClosure<TUserContext>, TOutput> func) =>
            Handle(closure =>
            {
                var (dbContext, auditVisitor) = createDbContextForStorage();
                using (dbContext)
                    return func(new ReliantOrmHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, ormGFactory, entityMetaServiceContainer, auditVisitor, dbContext), closure);
            });

        public Task<TOutput> HandleOrmFactoryAsync<TOutput>(Func<ReliantOrmHandlerGFactory<TUserContext, TDbContext>, RoutineClosure<TUserContext>, Task<TOutput>> func) =>
            HandleAsync(async closure =>
            {
                var (dbContext, auditVisitor) = createDbContextForStorage();
                using (dbContext)
                    return await func(new ReliantOrmHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, ormGFactory, entityMetaServiceContainer, auditVisitor, dbContext), closure);
            });

        public Task HandleOrmFactoryAsync(Func<ReliantOrmHandlerGFactory<TUserContext, TDbContext>, RoutineClosure<TUserContext>, Task> action) =>
            HandleAsync(async closure =>
            {
                var (dbContext, auditVisitor) = createDbContextForStorage();
                using (dbContext)
                    await action(new ReliantOrmHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, ormGFactory, entityMetaServiceContainer, auditVisitor, dbContext), closure);
            });

        public void HandleRepositoryFactory(Action<ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>, RoutineClosure<TUserContext>> action) =>
            Handle(closure =>
            {
                using (var dbContext = createDbContext())
                    action(new ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, dbContext), closure);
            });

        public TOutput HandleRepositoryFactory<TOutput>(Func<ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>, RoutineClosure<TUserContext>, TOutput> func) =>
            Handle(closure =>
            {
                using (var dbContext = createDbContext())
                    return func(new ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, dbContext), closure);
            });

        public Task<TOutput> HandleRepositoryFactoryAsync<TOutput>(Func<ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>, RoutineClosure<TUserContext>, Task<TOutput>> func) =>
            HandleAsync(async closure =>
            {
                using (var dbContext = createDbContext())
                    return await func(new ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, dbContext), closure);
            });

        public Task HandleRepositoryFactoryAsync(Func<ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>, RoutineClosure<TUserContext>, Task> action) =>
            HandleAsync(async closure =>
            {
                using (var dbContext = createDbContext())
                    await action(new ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, dbContext), closure);
            });

        public void HandleOrmFactory(Action<ReliantOrmHandlerGFactory<TUserContext, TDbContext>> action) =>
            Handle(closure =>
            {
                var (dbContext, auditVisitor) = createDbContextForStorage();
                using (dbContext)
                    action(new ReliantOrmHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, ormGFactory, entityMetaServiceContainer, auditVisitor, dbContext));
            });

        public TOutput HandleOrmFactory<TOutput>(Func<ReliantOrmHandlerGFactory<TUserContext, TDbContext>, TOutput> func) =>
            Handle(closure =>
            {
                var (dbContext, auditVisitor) = createDbContextForStorage();
                using (dbContext)
                    return func(new ReliantOrmHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, ormGFactory, entityMetaServiceContainer, auditVisitor, dbContext));
            });

        public Task<TOutput> HandleOrmFactoryAsync<TOutput>(Func<ReliantOrmHandlerGFactory<TUserContext, TDbContext>, Task<TOutput>> func) =>
            HandleAsync(async closure =>
            {
                var (dbContext, auditVisitor) = createDbContextForStorage();
                using (dbContext)
                    return await func(new ReliantOrmHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, ormGFactory, entityMetaServiceContainer, auditVisitor, dbContext));
            });

        public Task HandleOrmFactoryAsync(Func<ReliantOrmHandlerGFactory<TUserContext, TDbContext>, Task> action) =>
            HandleAsync(async closure =>
            {
                var (dbContext, auditVisitor) = createDbContextForStorage();
                using (dbContext)
                    await action(new ReliantOrmHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, ormGFactory, entityMetaServiceContainer, auditVisitor, dbContext));
            });

        public void HandleRepositoryFactory(Action<ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>> action) =>
            Handle(closure =>
            {
                using (var dbContext = createDbContext())
                    action(new ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, dbContext));
            });

        public TOutput HandleRepositoryFactory<TOutput>(Func<ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>, TOutput> func) =>
            Handle(closure =>
            {
                using (var dbContext = createDbContext())
                    return func(new ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, dbContext));
            });

        public Task HandleRepositoryFactoryAsync<TOutput>(Func<ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>, Task> func) =>
            HandleAsync(async closure =>
            {
                using (var dbContext = createDbContext())
                    await func(new ReliantRepositoryHandlerGFactory<TUserContext, TDbContext>(repositoryGFactory, dbContext));
            });
        #endregion
    }
}