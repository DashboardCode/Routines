using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Vse.Routines.Storage
{
    public interface IRepository<TEntity> where TEntity : class
    {
        #region Queries
        IReadOnlyCollection<TEntity> ToList(Include<TEntity> include = null);
        IReadOnlyCollection<TEntity> ToList(Expression<Func<TEntity, bool>> predicate, Include<TEntity> include = null);
        TEntity Find(Expression<Func<TEntity, bool>> predicate, Include<TEntity> include = null);
        IQueryable<TEntity> GetQueryable(Include<TEntity> include = null);
        #endregion

        #region Navigations
        IRepository<TNewBaseEntity> Rebase<TNewBaseEntity>() where TNewBaseEntity : class;
        void Detach(TEntity entity, Include<TEntity> include = null);
        #endregion
    }
}
