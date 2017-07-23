using System;
using System.Threading.Tasks;
using DashboardCode.Routines.Storage;

namespace DashboardCode.Routines.Injected
{
    public class UserRoutine<TUserContext> : Routine<RoutineState<TUserContext>>
    {
        readonly IRepositoryHandlerFactory<TUserContext> repositoryHandlerFactory;
        public UserRoutine(
            IBasicLogging basicLogging,
            Func<Exception, Exception> transformException,
            Func<Action<DateTime, string>, RoutineState<TUserContext>> createRoutineState,
            IRepositoryHandlerFactory<TUserContext> repositoryHandlerFactory,
            object input
            ):base(
                new BasicRoutineTransients<RoutineState<TUserContext>>(
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
            Func<IRepository<TEntity>, IStorage<TEntity>, TOutput> func
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
            Action<IRepository<TEntity>, RoutineState<TUserContext>> action
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
            Func<IRepository<TEntity>, RoutineState<TUserContext>, TOutput> func
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
            Func<IRepository<TEntity>, RoutineState<TUserContext>, TOutput> func
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
            Action<IRepository<TEntity>, IStorage<TEntity>> action
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
            Func<IRepository<TEntity>, IStorage<TEntity>, TOutput> func
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
            Func<IRepository<TEntity>, IStorage<TEntity>, TOutput> func
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
            Action<IRepository<TEntity>, IStorage<TEntity>, RoutineState<TUserContext>> action
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
            Func<IRepository<TEntity>, IStorage<TEntity>, RoutineState<TUserContext>, TOutput> func
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
            Func<IRepository<TEntity>, IStorage<TEntity>, RoutineState<TUserContext>, TOutput> func
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

    }
}
