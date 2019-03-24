using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DashboardCode.Routines.Storage
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IReadOnlyCollection<TEntity> List(Include<TEntity> include = null);
        Task<List<TEntity>> ListAsync(Include<TEntity> include = null);

        IReadOnlyCollection<TEntity> List(Expression<Func<TEntity, bool>> predicate, Include<TEntity> include = null);
        Task<IReadOnlyCollection<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate, Include<TEntity> include = null);

        TEntity Find(Expression<Func<TEntity, bool>> predicate, Include<TEntity> include = null);
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, Include<TEntity> include = null);


        IQueryable<TEntity> Query(Include<TEntity> include = null);

        #region Prototype support
        IRepository<TNewBaseEntity> Clone<TNewBaseEntity>() where TNewBaseEntity : class;
        #endregion

        #region ORM "abstraction leaks" - sometimes you need to manipulate entities without pushing changes to db
        void LoadCollection<TRelationEntity>(TEntity entity, Expression<Func<TEntity, IEnumerable<TRelationEntity>>> getTmmExpression) where TRelationEntity : class;


        void Detach(TEntity entity, Include<TEntity> include = null);
        void Detach(IEnumerable<TEntity> entity, Include<TEntity> include = null);
        #endregion
    }
}