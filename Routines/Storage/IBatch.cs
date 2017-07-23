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
                Expression<Func<TEntity, IEnumerable<TRelationEntity>>> getRelationExpression,
                IEnumerable<TRelationEntity> newRelations,
                Func<TRelationEntity, TRelationEntity, bool> equalsById
        ) where TRelationEntity : class;
    }
}
