using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DashboardCode.Routines.Storage
{
    public delegate void    Transacted<TEntity>(Action< IRepository<TEntity>, IOrmStorage<TEntity> > action) where TEntity: class;

    public delegate TOutput Transacted<TEntity, TOutput>(Func< IRepository<TEntity>, Func<Action<IBatch<TEntity>>, StorageResult>, TOutput> func) where TEntity : class;

    public delegate TOutput TransactedAsync<TEntity, TOutput>(Func<IRepository<TEntity>, Func<Func<IBatch<TEntity>,Task>, Task<StorageResult>>, TOutput> func) where TEntity : class;

    public delegate void TransactedAsync<TEntity>(Action<IRepository<TEntity>, Func<Func<IBatch<TEntity>, Task>, Task<StorageResult>>> action) where TEntity : class;

    public interface IBatch<TEntity>
    {
        void Add(TEntity t);
        void Remove(TEntity t);
        void Modify(TEntity t, Include<TEntity> include=null);
        void LoadAndModifyRelated<TRelationEntity>(
                TEntity entity,
                Expression<Func<TEntity, ICollection<TRelationEntity>>> getRelated,
                IEnumerable<TRelationEntity> releatedCollection,
                Func<TRelationEntity, TRelationEntity, bool> equalsById
            ) where TRelationEntity : class;
        void ModifyRelated<TRelationEntity>(
            TEntity entity,
            ICollection<TRelationEntity> oldRelations,
            IEnumerable<TRelationEntity> newRelations,
            Func<TRelationEntity, TRelationEntity, bool> equalsById
            ) where TRelationEntity : class;

        void ModifyRelated<TRelationEntity>(
            TEntity entity,
            ICollection<TRelationEntity> oldRelations,
            IEnumerable<TRelationEntity> newRelations,
            Func<TRelationEntity, TRelationEntity, bool> equalsById,
            Func<TRelationEntity, TRelationEntity, bool> equalsByValue,
            Action<TRelationEntity, TRelationEntity> updateValue
            ) where TRelationEntity : class;
        
     }

    public interface IBatch
    {
        void Add<TEntity>(TEntity t)    where TEntity : class;
        void Modify<TEntity>(TEntity t) where TEntity : class;
        void Remove<TEntity>(TEntity t) where TEntity : class;
    }
}