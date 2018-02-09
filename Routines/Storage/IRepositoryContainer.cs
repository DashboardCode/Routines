using System;

namespace DashboardCode.Routines.Storage
{
    public interface IRepositoryContainer<TDbContext>
    {
        Func<TDbContext, bool, IRepository<TEntity>> ResolveCreateRepository<TEntity>() where TEntity : class;
    }
}