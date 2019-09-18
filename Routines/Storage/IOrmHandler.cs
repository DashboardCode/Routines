using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines.Storage
{
    public interface IOrmHandler<TEntity> where TEntity : class
    {
        void Handle(Action<IRepository<TEntity>> action);
        TOutput Handle<TOutput>(Func<IRepository<TEntity>, TOutput> func);
        Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, Task<TOutput>> func);
        Task HandleAsync(Func<IRepository<TEntity>, Task> func);

        void Handle(Action<IRepository<TEntity>, IOrmStorage<TEntity>> action);
        TOutput Handle<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, TOutput> func);
        Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, Task<TOutput>> func);
        Task HandleAsync(Func<IRepository<TEntity>, IOrmStorage<TEntity>, Task> func);

        void Handle(Action<IRepository<TEntity>, IOrmStorage<TEntity>, IOrmEntitySchemaAdapter<TEntity>> action);
        TOutput Handle<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, IOrmEntitySchemaAdapter<TEntity>, TOutput> func);
        Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, IOrmEntitySchemaAdapter<TEntity>, Task<TOutput>> func);
        Task HandleAsync(Func<IRepository<TEntity>, IOrmStorage<TEntity>, IOrmEntitySchemaAdapter<TEntity>, Task> func);
    }
}
