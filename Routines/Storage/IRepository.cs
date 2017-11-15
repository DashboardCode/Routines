using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DashboardCode.Routines.Storage
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IReadOnlyCollection<TEntity> List(Include<TEntity> include = null);
        IReadOnlyCollection<TEntity> List(Expression<Func<TEntity, bool>> predicate, Include<TEntity> include = null);
        TEntity Find(Expression<Func<TEntity, bool>> predicate, Include<TEntity> include = null);
        IQueryable<TEntity> MakeQueryable(Include<TEntity> include = null);

        #region Prototyping support
        IRepository<TNewBaseEntity> Sprout<TNewBaseEntity>() where TNewBaseEntity : class;
        #endregion

        #region ORM "abstraction leaks"
        void Detach(TEntity entity, Include<TEntity> include = null);
        void Detach(IEnumerable<TEntity> entity, Include<TEntity> include = null);
        #endregion

        #region Navigations meta
        Include<TEntity> AppendModelFields(Include<TEntity> include);

        Include<TEntity> AppendModelFieldsIfEmpty(Include<TEntity> include);

        Include<TEntity> ExtractNavigations(Include<TEntity> include);

        Include<TEntity> ExtractNavigationsAppendKeyLeafs(Include<TEntity> include);
        #endregion
    }
}
