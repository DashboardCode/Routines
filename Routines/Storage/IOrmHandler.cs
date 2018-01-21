using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines.Storage
{
    /// <summary>
    /// This interface allows do not reference huge EFCore's packages batch 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IOrmHandler<TEntity> where TEntity : class
    {
        //void          Handle               (Action<IRepository<TEntity>> action);
        //TOutput       Handle<TOutput>      (Func<IRepository<TEntity>, TOutput> func);
        //Task<TOutput> HandleAsync<TOutput> (Func<IRepository<TEntity>, TOutput> func);

        void          Handle(Action<IRepository<TEntity>, IOrmStorage<TEntity>> action);
        TOutput       Handle<TOutput>      (Func<IRepository<TEntity>, IOrmStorage<TEntity>, TOutput> func);
        Task<TOutput> HandleAsync<TOutput> (Func<IRepository<TEntity>, IOrmStorage<TEntity>, TOutput> func);

        void Handle(Action<IRepository<TEntity>, IOrmStorage<TEntity>, IOrmEntitySchemaAdapter<TEntity>> action);
        TOutput Handle<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, IOrmEntitySchemaAdapter<TEntity>, TOutput> func);
        Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, IOrmStorage<TEntity>, IOrmEntitySchemaAdapter<TEntity>, TOutput> func);
    }
}
