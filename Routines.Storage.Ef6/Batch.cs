using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace DashboardCode.Routines.Storage.Ef6
{
    public class Batch<TEntity> : IBatch<TEntity> where TEntity : class
    {
        private readonly DbContext context;
        private readonly Action<object> setAuditProperties;

        public Batch(DbContext context, Action<object> setAuditProperties)
        {
            this.context = context;
            this.setAuditProperties = setAuditProperties;
        }

        public void Add(TEntity entity)
        {
            setAuditProperties(entity);
            context.Set<TEntity>().Add(entity);
        }

        public void Modify(TEntity entity, Include<TEntity> include = null)
        {
            setAuditProperties(entity);
            var entry = context.Entry(entity);
            if (include != null)
            {
                var propertyValues = entry.GetDatabaseValues();
                var entityDbState = (TEntity)propertyValues.ToObject();
                ObjectExtensions.Copy(entityDbState, entity, include);
            }
            entry.State = EntityState.Modified;
        }

        public void Remove(TEntity entity)
        {
            context.Set<TEntity>().Remove(entity);
        }

        public void LoadAndModifyRelated<TRelationEntity>(
            TEntity entity,
            Expression<Func<TEntity, ICollection<TRelationEntity>>> getRelated,
            IEnumerable<TRelationEntity> newRelations,
            Func<TRelationEntity, TRelationEntity, bool> equalsById
            ) where TRelationEntity : class
        {
            DbEntityEntry<TEntity> entry = context.Entry(entity);
            var name = getRelated.GetMemberName();
            var col = entry.Collection(name);
            col.Load();

            var getRelationFunc = getRelated.Compile();
            var oldRelations = getRelationFunc(entity);
            ModifyRelated(entity, oldRelations, newRelations, equalsById);
        }

        public void ModifyRelated<TRelationEntity>(TEntity entity, ICollection<TRelationEntity> oldRelations, IEnumerable<TRelationEntity> newRelations, Func<TRelationEntity, TRelationEntity, bool> equalsById) where TRelationEntity : class
        {
            setAuditProperties(entity); 
            EntityExtensions.UpdateCollection(
                oldRelations,
                newRelations,
                equalsById,
                setAuditProperties
                );
        }

        public void ModifyRelated<TRelationEntity>(TEntity entity, ICollection<TRelationEntity> oldRelations, IEnumerable<TRelationEntity> newRelations, Func<TRelationEntity, TRelationEntity, bool> equalsById, Func<TRelationEntity, TRelationEntity, bool> equalsByValue, Action<TRelationEntity, TRelationEntity> updateValue) where TRelationEntity : class
        {
            throw new NotImplementedException();
        }
    }

    public class Batch : IBatch
    {
        private readonly DbContext context;
        private readonly Action<object> setAuditProperties;

        public Batch(DbContext context, Action<object> setAuditProperties)
        {
            this.context = context;
            this.setAuditProperties = setAuditProperties;
        }

        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            setAuditProperties(entity);
            context.Set<TEntity>().Add(entity);
        }

        public void Modify<TEntity>(TEntity entity) where TEntity : class
        {
            setAuditProperties(entity);
            context.Entry(entity).State = EntityState.Modified;
        }

        public void Remove<TEntity>(TEntity entity) where TEntity : class
        {
            context.Set<TEntity>().Remove(entity);
        }
    }
}