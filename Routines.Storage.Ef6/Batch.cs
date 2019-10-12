﻿using System;
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
        private readonly IAuditVisitor auditVisitor;

        public Batch(DbContext context, IAuditVisitor auditVisitor)
        {
            this.context = context;
            this.auditVisitor = auditVisitor;
        }

        public void Add(TEntity entity)
        {
            auditVisitor.SetAuditProperties(entity);
            context.Set<TEntity>().Add(entity);
        }

        public void Modify(TEntity entity, Include<TEntity> include = null)
        {
            auditVisitor.SetAuditProperties(entity);
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
            Expression<Func<TEntity, ICollection<TRelationEntity>>> getTmmExpression,
            IEnumerable<TRelationEntity> newRelations,
            Func<TRelationEntity, TRelationEntity, bool> equalsById
            ) where TRelationEntity : class
        {
            DbEntityEntry<TEntity> entry = context.Entry(entity);
            var name = getTmmExpression.GetMemberName();
            var col = entry.Collection(name);
            col.Load();

            var getTmm = getTmmExpression.Compile();
            var oldRelations = getTmm(entity);
            ModifyRelated(entity, oldRelations, newRelations, equalsById);

            // Expression<Func<TEntity, IEnumerable<TRelationEntity>>> getRelationAsEnumerable = getTmmExpression.ContravarianceToIEnumerable();
            // DbContextExtensions.LoadCollection(context, entity, getRelationAsEnumerable);
            // var getTmm = getTmmExpression.Compile();
            // var oldRelations = getTmm(entity);
            // ModifyRelated(
            //    entity, oldRelations, newRelations, equalsById
            // );
        }


        public void ModifyRelated<TRelationEntity>(
            TEntity entity, ICollection<TRelationEntity> oldRelations, IEnumerable<TRelationEntity> newRelations, 
            Func<TRelationEntity, TRelationEntity, bool> equalsById) where TRelationEntity : class
        {
            auditVisitor.SetAuditProperties(entity); 
            EntityExtensions.UpdateCollection(
                oldRelations,
                newRelations,
                equalsById,
                e => auditVisitor.SetAuditProperties(e)
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
        private readonly IAuditVisitor auditVisitor;

        public Batch(DbContext context, IAuditVisitor auditVisitor)
        {
            this.context = context;
            this.auditVisitor = auditVisitor;
        }
        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            auditVisitor.SetAuditProperties(entity);
            context.Set<TEntity>().Add(entity);
        }

        public void Modify<TEntity>(TEntity entity) where TEntity : class
        {
            auditVisitor.SetAuditProperties(entity);
            context.Entry(entity).State = EntityState.Modified;
        }

        public void Remove<TEntity>(TEntity entity) where TEntity : class
        {
            context.Set<TEntity>().Remove(entity);
        }
    }
}