using System;
using System.Threading.Tasks;
using DashboardCode.Routines.Storage;

namespace DashboardCode.Routines.Injected
{
    public class UserRoutineHandler<TUserContext> : RoutineHandler<RoutineClosure<TUserContext>>
    {
        readonly IOrmHandlerGFactory<TUserContext> ormHandlerGFactory;
        readonly IRepositoryHandlerGFactory<TUserContext> repositoryHandlerGFactory;

        public UserRoutineHandler(
            IBasicLogging basicLogging,
            Func<Exception, Exception> transformException,
            Func<Action<DateTime, string>, RoutineClosure<TUserContext>> createRoutineState,

            IRepositoryHandlerGFactory<TUserContext> repositoryHandlerFactory,
            IOrmHandlerGFactory<TUserContext> ormHandlerGFactory,
            
            object input
            ):base(
                new BasicRoutineTransients<RoutineClosure<TUserContext>>(
                    basicLogging,
                    transformException,
                    (verbose) => createRoutineState(verbose)
                    ), 
                input)
        {
            this.repositoryHandlerGFactory = repositoryHandlerFactory;
            this.ormHandlerGFactory = ormHandlerGFactory;
        }

        public async Task<TOutput> HandleStorageAsync<TOutput, TEntity>(
            Func<IRepository<TEntity>, TOutput> func
            ) where TEntity : class
        {
            return await HandleAsync(closure =>
            {
                var repositoryHandler = repositoryHandlerGFactory.Create<TEntity>(closure);
                return repositoryHandler.Handle(repository =>
                {
                    var output = func(repository);
                    return output;
                });
            });
        }

        public void HandleRepository<TEntity>(
            Action<IRepository<TEntity>> action
        ) where TEntity : class
        {
            Handle(closure =>
            {
                var repositoryHandler = repositoryHandlerGFactory.Create<TEntity>(closure);
                repositoryHandler.Handle(repository =>
                {
                    action(repository);
                });
            });
        }

        public TOutput HandleRepository<TOutput, TEntity>(
            Func<IRepository<TEntity>, TOutput> func
            ) where TEntity : class
        {
            return Handle(closure =>
            {
                var repositoryHandler = repositoryHandlerGFactory.Create<TEntity>(closure);
                return repositoryHandler.Handle(repository =>
                {
                    return func(repository);
                });
            });
        }

        public async Task<TOutput> HandleRepositoryAsync<TOutput, TEntity>(
            Func<IRepository<TEntity>, IOrmStorage<TEntity>, TOutput> func
            ) where TEntity : class
        {
            return await HandleAsync(closure =>
            {
                var repositoryHandler = ormHandlerGFactory.Create<TEntity>(closure);
                return repositoryHandler.Handle((repository, store) =>
                {
                    var output = func(repository, store);
                    return output;
                });
            });
        }

        public void HandleRepository<TEntity>(
            Action<IRepository<TEntity>, RoutineClosure<TUserContext>> action
        ) where TEntity : class
        {
            Handle(closure =>
            {
                var repositoryHandler = repositoryHandlerGFactory.Create<TEntity>(closure);
                repositoryHandler.Handle(repository =>
                {
                    action(repository, closure);
                });
            });
        }

        public TOutput HandleRepository<TOutput, TEntity>(
            Func<IRepository<TEntity>, RoutineClosure<TUserContext>, TOutput> func
            ) where TEntity : class
        {
            return Handle(closure =>
            {
                var repositoryHandler = repositoryHandlerGFactory.Create<TEntity>(closure);
                return repositoryHandler.Handle(repository =>
                {
                    return func(repository, closure);
                });
            });
        }

        public async Task<TOutput> HandleRepositoryAsync<TOutput, TEntity>(
            Func<IRepository<TEntity>, RoutineClosure<TUserContext>, TOutput> func
            ) where TEntity : class
        {
            return await HandleAsync(closure =>
            {
                var repositoryHandler = repositoryHandlerGFactory.Create<TEntity>(closure);
                return repositoryHandler.Handle(repository =>
                {
                    var output = func(repository, closure);
                    return output;
                });
            });
        }

        public void HandleStorage<TEntity>(
            Action<IRepository<TEntity>, IOrmStorage<TEntity>> action
        ) where TEntity : class
        {
            Handle(closure =>
            {
                var ormHandler = ormHandlerGFactory.Create<TEntity>(closure);
                ormHandler.Handle((repository, store) =>
                {
                    action(repository, store);
                });
            });
        }

        public TOutput HandleStorage<TOutput, TEntity>(
            Func<IRepository<TEntity>, IOrmStorage<TEntity>, TOutput> func
            ) where TEntity : class
        {
            return Handle(closure =>
            {
                var ormHandler = ormHandlerGFactory.Create<TEntity>(closure);
                return ormHandler.Handle((repository, store) =>
                {
                    return func(repository, store);
                });
            });
        }

        public async Task<TOutput> HandleStorageAsync<TOutput, TEntity>(
            Func<IRepository<TEntity>, IOrmStorage<TEntity>, TOutput> func
            ) where TEntity : class
        {
            return await HandleAsync(closure =>
            {
                var ormHandler = ormHandlerGFactory.Create<TEntity>(closure);
                return ormHandler.Handle((repository, store) =>
                {
                    var output = func(repository, store);
                    return output;
                });
            });
        }

        public void HandleStorage<TEntity>(
            Action<IRepository<TEntity>, IOrmStorage<TEntity>, RoutineClosure<TUserContext>> action
        ) where TEntity : class
        {
            Handle(closure =>
            {
                var ormHandler = ormHandlerGFactory.Create<TEntity>(closure);
                ormHandler.Handle((repository, store) =>
                {
                    action(repository, store, closure);
                });
            });
        }

        public TOutput HandleStorage<TOutput, TEntity>(
            Func<IRepository<TEntity>, IOrmStorage<TEntity>, RoutineClosure<TUserContext>, TOutput> func
            ) where TEntity : class
        {
            return Handle(closure =>
            {
                var ormHandler = ormHandlerGFactory.Create<TEntity>(closure);
                return ormHandler.Handle((repository, store) =>
                {
                    return func(repository, store, closure);
                });
            });
        }

        public async Task<TOutput> HandleStorageAsync<TOutput, TEntity>(
            Func<IRepository<TEntity>, IOrmStorage<TEntity>, RoutineClosure<TUserContext>, TOutput> func
            ) where TEntity : class
        {
            return await HandleAsync(closure =>
            {
                var ormHandler = ormHandlerGFactory.Create<TEntity>(closure);
                return ormHandler.Handle((repository, store) =>
                {
                    var output = func(repository, store, closure);
                    return output;
                });
            });
        }

        public async Task<TOutput> HandleTransactionAsync<TOutput, TEntity>(
            Func<Transacted<TEntity, TOutput>, RoutineClosure<TUserContext>, TOutput> func
            ) where TEntity : class
        {
            return await HandleAsync(closure =>
            {
                var ormHandler = ormHandlerGFactory.Create<TEntity>(closure);
                return ormHandler.Handle((repository, storage) =>
                {
                    return func(
                        f => f(repository, f2 => storage.HandleAnalyzableException(() => storage.HandleCommit(() => storage.HandleSave(batch => f2(batch))))),
                        closure);
                });
            });
        }
    }
}