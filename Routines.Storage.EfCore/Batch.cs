using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DashboardCode.Routines.Storage.EfCore
{
    public class Batch<TEntity> : IBatch<TEntity> where TEntity : class
    {
        private readonly DbContext context;
        private readonly IAuditVisitor auditVisitor;

        public Batch(DbContext context, IAuditVisitor auditVisitor)
        {
            this.context = context;
            this.auditVisitor = auditVisitor;
        }
        public void Add(TEntity entity)
        {
            auditVisitor.SetAuditProperties( entity);
            context.Set<TEntity>().Add(entity);
        }

        public void Modify(TEntity entity, Include<TEntity> include = null)
        {
            auditVisitor.SetAuditProperties(entity);
            EntityEntry<TEntity> entry = context.Entry(entity);
            // alternative: context.Entry(entity).Property("GroupAdName").IsModified = false;
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
            if (auditVisitor.HasAuditProperties(entity))
            {
                //EntityEntry<TEntity> entry = context.Entry(entity);
                //var propertyValues = entry.GetDatabaseValues();
                //var entityDbState = (TEntity)propertyValues.ToObject();
                //setAuditProperties(entityDbState);
            }
            context.Set<TEntity>().Remove(entity);
        }

        public void ModifyRelated<TRelationEntity>(
            TEntity entity,
            Expression<Func<TEntity, ICollection<TRelationEntity>>> getRelation,
            IEnumerable<TRelationEntity> newRelations,
            Func<TRelationEntity, TRelationEntity, bool> equalsById
            ) where TRelationEntity : class
        {
            auditVisitor.SetAuditProperties(entity); // TODO: test if ModifyWithRelated modifies entity ?
            Expression <Func<TEntity, IEnumerable<TRelationEntity>>> getRelationAsEnumerable = getRelation.ContravarianceToIEnumerable();
            EntityEntry<TEntity> entry = context.Entry(entity);
            //if (entry.State == EntityState.Detached)
            //    entry.State = EntityState.Unchanged;
            var col = entry.Collection(getRelationAsEnumerable);
            col.Load();
            var getRelationFunc = getRelation.Compile();
            var enumerable = getRelationFunc(entity);
            var oldRelations = enumerable;
            var tmp = new List<TRelationEntity>();
            foreach (var e in oldRelations)
                if (!newRelations.Any(e2 => equalsById(e, e2)))
                    tmp.Add(e);
            foreach (var e in tmp)
                oldRelations.Remove(e);

            foreach (var e in newRelations)
                if (!oldRelations.Any(e2 => equalsById(e, e2)))
                {
                    auditVisitor.SetAuditProperties(e);
                    oldRelations.Add(e);
                }
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