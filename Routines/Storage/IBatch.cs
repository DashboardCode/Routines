using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DashboardCode.Routines.Storage
{
    public delegate void    Transacted<TEntity>(Action< IRepository<TEntity>, IOrmStorage<TEntity> > action) where TEntity: class;

    public delegate TOutput Transacted<TEntity, TOutput>(Func< IRepository<TEntity>, Func<Action<IBatch<TEntity>>, StorageError>, TOutput> func) where TEntity : class;

    public interface IBatch<TEntity>
    {
        void Add(TEntity t);
        void Remove(TEntity t);
        void Modify(TEntity t);
        void ModifyWithRelated<TRelationEntity>(
                TEntity entity,
                Expression<Func<TEntity, ICollection<TRelationEntity>>> getRelated,
                IEnumerable<TRelationEntity> releatedCollection,
                Func<TRelationEntity, TRelationEntity, bool> equalsById
        ) where TRelationEntity : class;
    }

    public interface IBatch
    {
        void Add<TEntity>(TEntity t)    where TEntity : class;
        void Modify<TEntity>(TEntity t) where TEntity : class;
        void Remove<TEntity>(TEntity t) where TEntity : class;
    }
}