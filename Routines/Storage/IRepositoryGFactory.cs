using System;

namespace DashboardCode.Routines.Storage
{
    public interface IRepositoryGFactory<TDbContext>
    {
        Func<TDbContext, bool, IRepository<TEntity>> ComposeCreateRepository<TEntity>() where TEntity : class;
    }
}