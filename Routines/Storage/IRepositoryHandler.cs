using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines.Storage
{
    public interface IRepositoryHandler<TEntity> where TEntity : class
    {
        void Handle(Action<IRepository<TEntity>> action);
        TOutput Handle<TOutput>(Func<IRepository<TEntity>, TOutput> func);
        Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, TOutput> func);
    }
}