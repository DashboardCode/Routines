using System;
using System.Threading.Tasks;
using DashboardCode.Routines.Storage;

namespace DashboardCode.Routines.Injected
{
    public class UserRoutineHandler<TUserContext> : RoutineHandler<Routine<TUserContext>>
    {
        readonly IRepositoryHandlerFactory<TUserContext> repositoryHandlerFactory;
        public UserRoutineHandler(
            IBasicLogging basicLogging,
            Func<Exception, Exception> transformException,
            Func<Action<DateTime, string>, Routine<TUserContext>> createRoutineState,
            IRepositoryHandlerFactory<TUserContext> repositoryHandlerFactory,
            object input
            ):base(
                new BasicRoutineTransients<Routine<TUserContext>>(
                    basicLogging,
                    transformException,
                    (verbose) => createRoutineState(verbose)
                    ), 
                input)
        {
            this.repositoryHandlerFactory = repositoryHandlerFactory;
        }
        public async Task<TOutput> HandleStorageAsync<TOutput, TEntity>(
            Func<IRepository<TEntity>, TOutput> func
            ) where TEntity : class
        {
            return await HandleAsync(state =>
            {
                var repositoryHandler = repositoryHandlerFactory.CreateRepositoryHandler<TEntity>(state);
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
            Handle(state =>
            {
                var repositoryHandler = repositoryHandlerFactory.CreateRepositoryHandler<TEntity>(state);
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
            return Handle(state =>
            {
                var repositoryHandler = repositoryHandlerFactory.CreateRepositoryHandler<TEntity>(state);
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
            return await HandleAsync(state =>
            {
                var repositoryHandler = repositoryHandlerFactory.CreateRepositoryHandler<TEntity>(state);
                return repositoryHandler.Handle((repository, store) =>
                {
                    var output = func(repository, store);
                    return output;
                });
            });
        }
        public void HandleRepository<TEntity>(
            Action<IRepository<TEntity>, Routine<TUserContext>> action
        ) where TEntity : class
        {
            Handle(state =>
            {
                var repositoryHandler = repositoryHandlerFactory.CreateRepositoryHandler<TEntity>(state);
                repositoryHandler.Handle(repository =>
                {
                    action(repository, state);
                });
            });
        }

        public TOutput HandleRepository<TOutput, TEntity>(
            Func<IRepository<TEntity>, Routine<TUserContext>, TOutput> func
            ) where TEntity : class
        {
            return Handle(state =>
            {
                var repositoryHandler = repositoryHandlerFactory.CreateRepositoryHandler<TEntity>(state);
                return repositoryHandler.Handle(repository =>
                {
                    return func(repository, state);
                });
            });
        }

        public async Task<TOutput> HandleRepositoryAsync<TOutput, TEntity>(
            Func<IRepository<TEntity>, Routine<TUserContext>, TOutput> func
            ) where TEntity : class
        {
            return await HandleAsync(state =>
            {
                var repositoryHandler = repositoryHandlerFactory.CreateRepositoryHandler<TEntity>(state);
                return repositoryHandler.Handle(repository =>
                {
                    var output = func(repository, state);
                    return output;
                });
            });
        }

        public void HandleStorage<TEntity>(
            Action<IRepository<TEntity>, IOrmStorage<TEntity>> action
        ) where TEntity : class
        {
            Handle(state =>
            {
                var repositoryHandler = repositoryHandlerFactory.CreateRepositoryHandler<TEntity>(state);
                repositoryHandler.Handle((repository, store) =>
                {
                    action(repository, store);
                });
            });
        }

        public TOutput HandleStorage<TOutput, TEntity>(
            Func<IRepository<TEntity>, IOrmStorage<TEntity>, TOutput> func
            ) where TEntity : class
        {
            return Handle(state =>
            {
                var repositoryHandler = repositoryHandlerFactory.CreateRepositoryHandler<TEntity>(state);
                return repositoryHandler.Handle((repository, store) =>
                {
                    return func(repository, store);
                });
            });
        }

        public async Task<TOutput> HandleStorageAsync<TOutput, TEntity>(
            Func<IRepository<TEntity>, IOrmStorage<TEntity>, TOutput> func
            ) where TEntity : class
        {
            return await HandleAsync(state =>
            {
                var repositoryHandler = repositoryHandlerFactory.CreateRepositoryHandler<TEntity>(state);
                return repositoryHandler.Handle((repository, store) =>
                {
                    var output = func(repository, store);
                    return output;
                });
            });
        }
        public void HandleStorage<TEntity>(
            Action<IRepository<TEntity>, IOrmStorage<TEntity>, Routine<TUserContext>> action
        ) where TEntity : class
        {
            Handle(state =>
            {
                var repositoryHandler = repositoryHandlerFactory.CreateRepositoryHandler<TEntity>(state);
                repositoryHandler.Handle((repository, store) =>
                {
                    action(repository, store, state);
                });
            });
        }

        public TOutput HandleStorage<TOutput, TEntity>(
            Func<IRepository<TEntity>, IOrmStorage<TEntity>, Routine<TUserContext>, TOutput> func
            ) where TEntity : class
        {
            return Handle(state =>
            {
                var repositoryHandler = repositoryHandlerFactory.CreateRepositoryHandler<TEntity>(state);
                return repositoryHandler.Handle((repository, store) =>
                {
                    return func(repository, store, state);
                });
            });
        }

        public async Task<TOutput> HandleStorageAsync<TOutput, TEntity>(
            Func<IRepository<TEntity>, IOrmStorage<TEntity>, Routine<TUserContext>, TOutput> func
            ) where TEntity : class
        {
            return await HandleAsync(state =>
            {
                var repositoryHandler = repositoryHandlerFactory.CreateRepositoryHandler<TEntity>(state);
                return repositoryHandler.Handle((repository, store) =>
                {
                    var output = func(repository, store, state);
                    return output;
                });
            });
        }

        public async Task<TOutput> HandleTransactionAsync<TOutput, TEntity>(
            Func<Transacted<TEntity, TOutput>, Routine<TUserContext>, TOutput> func
            ) where TEntity : class
        {
            return await HandleAsync(state =>
            {
                var repositoryHandler = repositoryHandlerFactory.CreateRepositoryHandler<TEntity>(state);
                return repositoryHandler.Handle((repository, storage) =>
                {
                    return func(
                        f => f(repository, f2 => storage.HandleException(() => storage.HandleCommit(() => storage.HandleSave(batch => f2(batch))))),
                        state);
                });
            });
        }
    }
}