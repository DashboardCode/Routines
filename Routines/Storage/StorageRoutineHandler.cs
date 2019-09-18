using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines.Storage
{
    public class StorageRoutineHandler<TUserContext> : IHandler<RoutineClosure<TUserContext>>
    {
        readonly IOrmHandlerGFactory<TUserContext> ormHandlerGFactory;
        readonly IRepositoryHandlerGFactory<TUserContext> repositoryHandlerGFactory;
        readonly IHandler<RoutineClosure<TUserContext>> routineHandler;

        public StorageRoutineHandler(
            IRepositoryHandlerGFactory<TUserContext> repositoryHandlerGFactory,
            IOrmHandlerGFactory<TUserContext> ormHandlerGFactory,
            IHandler<RoutineClosure<TUserContext>> routineHandler)
        {
            this.repositoryHandlerGFactory = repositoryHandlerGFactory;
            this.ormHandlerGFactory = ormHandlerGFactory;
            this.routineHandler = routineHandler;
        }

        public void Handle(Action<RoutineClosure<TUserContext>> action) =>
            routineHandler.Handle(action);

        public TOutput Handle<TOutput>(Func<RoutineClosure<TUserContext>, TOutput> func) =>
            routineHandler.Handle(func);

        public Task<TOutput> HandleAsync<TOutput>(Func<RoutineClosure<TUserContext>, Task<TOutput>> func) =>
            routineHandler.Handle(func);

        public Task HandleAsync(Func<RoutineClosure<TUserContext>, Task> func) =>
            routineHandler.Handle(func);


        public void HandleRepository<TEntity>(
            Action<IRepository<TEntity>> action
        ) where TEntity : class
        {
            routineHandler.Handle(closure =>
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
            return routineHandler.Handle(closure =>
            {
                var repositoryHandler = repositoryHandlerGFactory.Create<TEntity>(closure);
                return repositoryHandler.Handle(repository =>
                {
                    return func(repository);
                });
            });
        }

 
        public Task<TOutput> HandleRepositoryAsync<TOutput, TEntity>(
            Func<IRepository<TEntity>,  Task<TOutput>> func
            ) where TEntity : class
        {
            return routineHandler.HandleAsync(closure =>
            {
                var repositoryHandler = ormHandlerGFactory.Create<TEntity>(closure);
                return repositoryHandler.HandleAsync((repository, store) =>
                {
                    var output = func(repository);
                    return output;
                });
            });
        }

        public Task HandleRepositoryAsync<TEntity>(
            Func<IRepository<TEntity>,  Task> func
            ) where TEntity : class
        {
            return routineHandler.HandleAsync(closure =>
            {
                var repositoryHandler = ormHandlerGFactory.Create<TEntity>(closure);
                return repositoryHandler.HandleAsync((repository, store) =>
                {
                    var output = func(repository);
                    return output;
                });
            });
        }
        public void HandleRepository<TEntity>(
            Action<IRepository<TEntity>, RoutineClosure<TUserContext>> action
        ) where TEntity : class
        {
            routineHandler.Handle(closure =>
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
            return routineHandler.Handle(closure =>
            {
                var repositoryHandler = repositoryHandlerGFactory.Create<TEntity>(closure);
                return repositoryHandler.Handle(repository =>
                {
                    return func(repository, closure);
                });
            });
        }

        public Task<TOutput> HandleRepositoryAsync<TOutput, TEntity>(
            Func<IRepository<TEntity>, RoutineClosure<TUserContext>, Task<TOutput>> func
            ) where TEntity : class
        {
            return routineHandler.HandleAsync(closure =>
            {
                var repositoryHandler = repositoryHandlerGFactory.Create<TEntity>(closure);
                return repositoryHandler.HandleAsync(repository =>
                {
                    var output = func(repository, closure);
                    return output;
                });
            });
        }

        public Task HandleRepositoryAsync<TEntity>(
            Func<IRepository<TEntity>, RoutineClosure<TUserContext>, Task> func
            ) where TEntity : class
        {
            return routineHandler.HandleAsync(closure =>
            {
                var repositoryHandler = repositoryHandlerGFactory.Create<TEntity>(closure);
                return repositoryHandler.HandleAsync(repository =>
                {
                    var output = func(repository, closure);
                    return output;
                });
            });
        }


        // -------------------------------------------------
        public void HandleStorage<TEntity>(
            Action<IRepository<TEntity>, IOrmStorage<TEntity>> action
        ) where TEntity : class
        {
            routineHandler.Handle(closure =>
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
            return routineHandler.Handle(closure =>
            {
                var ormHandler = ormHandlerGFactory.Create<TEntity>(closure);
                return ormHandler.Handle((repository, store) =>
                {
                    return func(repository, store);
                });
            });
        }

        public Task<TOutput> HandleStorageAsync<TOutput, TEntity>(
            Func<IRepository<TEntity>, IOrmStorage<TEntity>, Task<TOutput>> func
            ) where TEntity : class
        {
            return routineHandler.HandleAsync(closure =>
            {
                var ormHandler = ormHandlerGFactory.Create<TEntity>(closure);
                return ormHandler.HandleAsync((repository, store) =>
                {
                    var output = func(repository, store);
                    return output;
                });
            });
        }



        public Task HandleStorageAsync<TEntity>(
            Func<IRepository<TEntity>, IOrmStorage<TEntity>, Task> func
            ) where TEntity : class
        {
            return routineHandler.HandleAsync(closure =>
            {
                var ormHandler = ormHandlerGFactory.Create<TEntity>(closure);
                return ormHandler.HandleAsync((repository, store) =>
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
            routineHandler.Handle(closure =>
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
            return routineHandler.Handle(closure =>
            {
                var ormHandler = ormHandlerGFactory.Create<TEntity>(closure);
                return ormHandler.Handle((repository, store) =>
                {
                    return func(repository, store, closure);
                });
            });
        }

        public Task<TOutput> HandleStorageAsync<TOutput, TEntity>(
            Func<IRepository<TEntity>, IOrmStorage<TEntity>, RoutineClosure<TUserContext>, Task<TOutput>> func
            ) where TEntity : class
        {
            return routineHandler.HandleAsync(closure =>
            {
                var ormHandler = ormHandlerGFactory.Create<TEntity>(closure);
                return ormHandler.HandleAsync((repository, store) =>
                {
                    var output = func(repository, store, closure);
                    return output;
                });
            });
        }

        public Task HandleStorageAsync<TEntity>(
            Func<IRepository<TEntity>, IOrmStorage<TEntity>, RoutineClosure<TUserContext>, Task> func
            ) where TEntity : class
        {
            return routineHandler.HandleAsync(closure =>
            {
                var ormHandler = ormHandlerGFactory.Create<TEntity>(closure);
                return ormHandler.HandleAsync((repository, store) =>
                {
                    var output = func(repository, store, closure);
                    return output;
                });
            });
        }


        public TOutput HandleTransaction<TOutput, TEntity>(
            Func<Transacted<TEntity, TOutput>, RoutineClosure<TUserContext>, TOutput> func
        ) where TEntity : class
        {
            return routineHandler.Handle(closure =>
            {
                var ormHandler = ormHandlerGFactory.Create<TEntity>(closure);
                return ormHandler.Handle((repository, storage) =>
                    func(
                        f => f(repository, f2 => storage.HandleAnalyzableException(() => storage.HandleCommit(() => storage.HandleSave(batch => f2(batch))))),
                        closure)
                );
            });
        }

        public Task<TOutput> HandleTransactionAsync<TOutput, TEntity>(
            Func<TransactedAsync<TEntity, TOutput>, RoutineClosure<TUserContext>, Task<TOutput>> func
            ) where TEntity : class
        {
            return routineHandler.HandleAsync(closure =>
            {
                var ormHandler = ormHandlerGFactory.Create<TEntity>(closure);
                return ormHandler.HandleAsync((repository, storage) =>
                {
                    return func(
                        transacted => transacted(
                            repository,
                            f2 => storage.HandleAnalyzableExceptionAsync(
                                () => storage.HandleCommitAsync(
                                    () => storage.HandleSaveAsync(batch => f2(batch))))
                        ),
                        closure);
                });
            });
        }

        public Task HandleTransactionAsync<TEntity>(
           Func<TransactedAsync<TEntity>, RoutineClosure<TUserContext>, Task> func
           ) where TEntity : class
        {
            return routineHandler.HandleAsync(closure =>
            {
                var ormHandler = ormHandlerGFactory.Create<TEntity>(closure);
                return ormHandler.HandleAsync((repository, storage) =>
                {
                    return func(
                        transacted => transacted(
                            repository,
                            f2 => storage.HandleAnalyzableExceptionAsync(() => storage.HandleCommitAsync(() => storage.HandleSaveAsync(batch => f2(batch))))
                        ),
                        closure);
                });
            });
        }
    }



    public class StorageHandler<TUserContext> : IHandler<RoutineClosure<TUserContext>>
    {
        readonly IOrmHandlerGFactory<TUserContext> ormHandlerGFactory;
        readonly IRepositoryHandlerGFactory<TUserContext> repositoryHandlerGFactory;
        readonly RoutineClosure<TUserContext> closure;

        public StorageHandler(
            IRepositoryHandlerGFactory<TUserContext> repositoryHandlerGFactory,
            IOrmHandlerGFactory<TUserContext> ormHandlerGFactory,
            RoutineClosure<TUserContext> closure)
        {
            this.repositoryHandlerGFactory = repositoryHandlerGFactory;
            this.ormHandlerGFactory = ormHandlerGFactory;
            this.closure = closure;
        }

        public void Handle(Action<RoutineClosure<TUserContext>> action) =>
            action(closure);

        public TOutput Handle<TOutput>(Func<RoutineClosure<TUserContext>, TOutput> func) =>
            func(closure);

        public Task<TOutput> HandleAsync<TOutput>(Func<RoutineClosure<TUserContext>, Task<TOutput>> func) =>
            func(closure);

        public Task HandleAsync(Func<RoutineClosure<TUserContext>, Task> func) =>
            func(closure);


        public void HandleRepository<TEntity>(
            Action<IRepository<TEntity>> action
        ) where TEntity : class
        {
            var repositoryHandler = repositoryHandlerGFactory.Create<TEntity>(closure);
            repositoryHandler.Handle(repository =>
            {
                action(repository);
            });
        }

        public TOutput HandleRepository<TOutput, TEntity>(
            Func<IRepository<TEntity>, TOutput> func
            ) where TEntity : class
        {
            var repositoryHandler = repositoryHandlerGFactory.Create<TEntity>(closure);
            return repositoryHandler.Handle(repository =>
            {
                return func(repository);
            });
        }

        public Task<TOutput> HandleRepositoryAsync<TOutput, TEntity>(
            Func<IRepository<TEntity>, Task<TOutput>> func
            ) where TEntity : class
        {
            var repositoryHandler = ormHandlerGFactory.Create<TEntity>(closure);
            return repositoryHandler.HandleAsync((repository, store) =>
            {
                var output = func(repository);
                return output;
            });
        }

        public Task HandleRepositoryAsync<TEntity>(
            Func<IRepository<TEntity>, Task> func
            ) where TEntity : class
        {
            var repositoryHandler = ormHandlerGFactory.Create<TEntity>(closure);
            return repositoryHandler.HandleAsync((repository, store) =>
            {
                var output = func(repository);
                return output;
            });
        }


        public void HandleRepository<TEntity>(
            Action<IRepository<TEntity>, RoutineClosure<TUserContext>> action
        ) where TEntity : class
        {
            var repositoryHandler = repositoryHandlerGFactory.Create<TEntity>(closure);
            repositoryHandler.Handle(repository =>
            {
                action(repository, closure);
            });
        }

        public TOutput HandleRepository<TOutput, TEntity>(
            Func<IRepository<TEntity>, RoutineClosure<TUserContext>, TOutput> func
            ) where TEntity : class
        {
            var repositoryHandler = repositoryHandlerGFactory.Create<TEntity>(closure);
            return repositoryHandler.Handle(repository =>
            {
                return func(repository, closure);
            });
        }

        public Task<TOutput> HandleRepositoryAsync<TOutput, TEntity>(
            Func<IRepository<TEntity>, RoutineClosure<TUserContext>, Task<TOutput>> func
            ) where TEntity : class
        {
            var repositoryHandler = repositoryHandlerGFactory.Create<TEntity>(closure);
            return repositoryHandler.HandleAsync(repository =>
            {
                var output = func(repository, closure);
                return output;
            });
        }

        public Task HandleRepositoryAsync<TEntity>(
            Func<IRepository<TEntity>, RoutineClosure<TUserContext>, Task> func
            ) where TEntity : class
        {
            var repositoryHandler = repositoryHandlerGFactory.Create<TEntity>(closure);
            return repositoryHandler.HandleAsync(repository =>
            {
                var output = func(repository, closure);
                return output;
            });
        }


        public void HandleStorage<TEntity>(
            Action<IRepository<TEntity>, IOrmStorage<TEntity>> action
        ) where TEntity : class
        {
            var ormHandler = ormHandlerGFactory.Create<TEntity>(closure);
            ormHandler.Handle((repository, store) =>
            {
                action(repository, store);
            });
        }

        public TOutput HandleStorage<TOutput, TEntity>(
            Func<IRepository<TEntity>, IOrmStorage<TEntity>, TOutput> func
            ) where TEntity : class
        {
            var ormHandler = ormHandlerGFactory.Create<TEntity>(closure);
            return ormHandler.Handle((repository, store) =>
            {
                return func(repository, store);
            });
        }

        public Task<TOutput> HandleStorageAsync<TOutput, TEntity>(
            Func<IRepository<TEntity>, IOrmStorage<TEntity>, Task<TOutput>> func
            ) where TEntity : class
        {
            var ormHandler = ormHandlerGFactory.Create<TEntity>(closure);
            return ormHandler.HandleAsync((repository, store) =>
            {
                var output = func(repository, store);
                return output;
            });
        }

        

        public Task HandleStorageAsync<TEntity>(
            Func<IRepository<TEntity>, IOrmStorage<TEntity>, Task> func
            ) where TEntity : class
        {
                var ormHandler = ormHandlerGFactory.Create<TEntity>(closure);
                return ormHandler.HandleAsync((repository, store) =>
                {
                    var output = func(repository, store);
                    return output;
                });
        }



        public void HandleStorage<TEntity>(
            Action<IRepository<TEntity>, IOrmStorage<TEntity>, RoutineClosure<TUserContext>> action
        ) where TEntity : class
        {
                var ormHandler = ormHandlerGFactory.Create<TEntity>(closure);
                ormHandler.Handle((repository, store) =>
                {
                    action(repository, store, closure);
                });
        }

        public TOutput HandleStorage<TOutput, TEntity>(
            Func<IRepository<TEntity>, IOrmStorage<TEntity>, RoutineClosure<TUserContext>, TOutput> func
            ) where TEntity : class
        {
                var ormHandler = ormHandlerGFactory.Create<TEntity>(closure);
                return ormHandler.Handle((repository, store) =>
                {
                    return func(repository, store, closure);
                });
        }

        public Task<TOutput> HandleStorageAsync<TOutput, TEntity>(
            Func<IRepository<TEntity>, IOrmStorage<TEntity>, RoutineClosure<TUserContext>, Task<TOutput>> func
            ) where TEntity : class
        {
                var ormHandler = ormHandlerGFactory.Create<TEntity>(closure);
                return ormHandler.HandleAsync((repository, store) =>
                {
                    var output = func(repository, store, closure);
                    return output;
                });
        }

        public Task HandleStorageAsync<TEntity>(
            Func<IRepository<TEntity>, IOrmStorage<TEntity>, RoutineClosure<TUserContext>, Task> func
            ) where TEntity : class
        {
                var ormHandler = ormHandlerGFactory.Create<TEntity>(closure);
                return ormHandler.HandleAsync((repository, store) =>
                {
                    var output = func(repository, store, closure);
                    return output;
                });
        }


        public TOutput HandleTransaction<TOutput, TEntity>(
            Func<Transacted<TEntity, TOutput>, RoutineClosure<TUserContext>, TOutput> func
        ) where TEntity : class
        {
                var ormHandler = ormHandlerGFactory.Create<TEntity>(closure);
                return ormHandler.Handle((repository, storage) =>
                    func(
                        f => f(repository, f2 => storage.HandleAnalyzableException(() => storage.HandleCommit(() => storage.HandleSave(batch => f2(batch))))),
                        closure)
                );
        }

        public Task<TOutput> HandleTransactionAsync<TOutput, TEntity>(
            Func<TransactedAsync<TEntity, TOutput>, RoutineClosure<TUserContext>, Task<TOutput>> func
            ) where TEntity : class
        {
                var ormHandler = ormHandlerGFactory.Create<TEntity>(closure);
                return ormHandler.HandleAsync((repository, storage) =>
                {
                    return func(
                        transacted => transacted(
                            repository,
                            f2 => storage.HandleAnalyzableExceptionAsync(() => storage.HandleCommitAsync(() => storage.HandleSaveAsync(batch => f2(batch))))
                        ),
                        closure);
                });
        }

        public Task HandleTransactionAsync<TEntity>(
           Func<TransactedAsync<TEntity>, RoutineClosure<TUserContext>, Task> func
           ) where TEntity : class
        {
                var ormHandler = ormHandlerGFactory.Create<TEntity>(closure);
                return ormHandler.HandleAsync((repository, storage) =>
                {
                    return func(
                        transacted => transacted(
                            repository,
                            f2 => storage.HandleAnalyzableExceptionAsync(() => storage.HandleCommitAsync(() => storage.HandleSaveAsync(batch => f2(batch))))
                        ),
                        closure);
                });
        }
    }
}