using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines.Storage
{
    public class ReliantOrmHandler<TEntity> : IOrmHandler<TEntity>
    where TEntity : class
    {
        readonly IOrmStorage<TEntity> ormStorage;
        readonly IRepository<TEntity> repository;
        readonly IOrmEntitySchemaAdapter<TEntity> ormEntitySchemaAdapter;

        public ReliantOrmHandler(
            IRepository<TEntity> repository,
            IOrmStorage<TEntity> ormStorage,
            IOrmEntitySchemaAdapter<TEntity> ormEntitySchemaAdapter
            )
        {
            this.repository = repository;
            this.ormStorage = ormStorage;
            this.ormEntitySchemaAdapter = ormEntitySchemaAdapter;
        }

        public void Handle(Action<IRepository<TEntity>, IOrmStorage<TEntity>> action)
        {
            action(repository, ormStorage);
        }

        public TOutput Handle<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, TOutput> func)
        {
            return func(repository, ormStorage);
        }

        public Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, Task<TOutput>> func)
        {
            return func(repository, ormStorage);
        }

        public void Handle(Action<IRepository<TEntity>, IOrmStorage<TEntity>, IOrmEntitySchemaAdapter<TEntity>> action)
        {
            action(repository, ormStorage, ormEntitySchemaAdapter);
        }

        public TOutput Handle<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, IOrmEntitySchemaAdapter<TEntity>, TOutput> func)
        {
            return func(repository, ormStorage, ormEntitySchemaAdapter
                );
        }

        public Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, IOrmEntitySchemaAdapter<TEntity>, Task<TOutput>> func)
        {
            return func(repository, ormStorage, ormEntitySchemaAdapter);
        }

        public Task HandleAsync(Func<IRepository<TEntity>, IOrmStorage<TEntity>, Task> func)
        {
            return func(repository, ormStorage);
        }

        public Task HandleAsync(Func<IRepository<TEntity>, IOrmStorage<TEntity>, IOrmEntitySchemaAdapter<TEntity>, Task> func)
        {
            return func(repository, ormStorage, ormEntitySchemaAdapter);
        }
    }
}