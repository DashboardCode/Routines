using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines.Storage
{
    /// <summary>
    /// This interface allows do not reference huge EFCore's packages batch 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepositoryHandler<TEntity> where TEntity : class
    {
        void Handle(Action<IRepository<TEntity>> action);
        TOutput Handle<TOutput>(Func<IRepository<TEntity>, TOutput> func);
        Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, TOutput> func);

        TOutput Handle<TOutput>(Func<IRepository<TEntity>, IStorage<TEntity>, TOutput> func);
        void Handle(Action<IRepository<TEntity>, IStorage<TEntity>> action);
        Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, IStorage<TEntity>, TOutput> func);
    }
}
