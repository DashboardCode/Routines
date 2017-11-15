using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DashboardCode.Routines.Storage
{
    public interface IBatch<TEntity>
    {
        void Add(TEntity t);
        void Modify(TEntity t);
        void Remove(TEntity t);
        void UpdateRelations<TRelationEntity>(
                TEntity entity,
                Expression<Func<TEntity, ICollection<TRelationEntity>>> getRelationExpression,
                IEnumerable<TRelationEntity> newRelations,
                Func<TRelationEntity, TRelationEntity, bool> equalsById
        ) where TRelationEntity : class;
    }

    public interface IBatch
    {
        void Add<TEntity>(TEntity t) where TEntity : class;
        void Modify<TEntity>(TEntity t) where TEntity : class;
        void Remove<TEntity>(TEntity t) where TEntity : class;
    }

    public interface IAdoBatch
    {
        void RemoveAll<TEntity>() where TEntity : class;
    }
}
