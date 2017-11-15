﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DashboardCode.Routines.Storage.EfCore
{
    public class Batch<TEntity> : IBatch<TEntity> where TEntity : class
    {
        private readonly DbContext context;
        private readonly Action<object> setAudit;

        public Batch(DbContext context, Action<object> setAudit)
        {
            this.context = context;
            this.setAudit = setAudit;
        }
        public void Add(TEntity entity)
        {
            setAudit(entity);
            context.Set<TEntity>().Add(entity);
        }

        public void Modify(TEntity entity)
        {
            setAudit(entity);
            context.Entry(entity).State = EntityState.Modified;
        }

        public void Remove(TEntity entity)
        {
            context.Set<TEntity>().Remove(entity);
        }

        public void ModifyWithRelated<TRelationEntity>(
            TEntity entity,
            Expression<Func<TEntity, ICollection<TRelationEntity>>> getRelation,
            IEnumerable<TRelationEntity> newRelations,
            Func<TRelationEntity, TRelationEntity, bool> equalsById
            ) where TRelationEntity : class
        {
            Expression <Func<TEntity, IEnumerable<TRelationEntity>>> getRelationAsEnumerable = getRelation.ContravarianceToIEnumerable();

            EntityEntry<TEntity> entry = context.Entry(entity);
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
                    setAudit(e);
                    oldRelations.Add(e);
                }
        }
    }


    public class Batch : IBatch
    {
        private readonly DbContext context;
        private readonly Action<object> setAudit;

        public Batch(DbContext context, Action<object> setAudit)
        {
            this.context = context;
            this.setAudit = setAudit;
        }
        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            setAudit(entity);
            context.Set<TEntity>().Add(entity);
        }

        public void Modify<TEntity>(TEntity entity) where TEntity : class
        {
            setAudit(entity);
            context.Entry(entity).State = EntityState.Modified;
        }

        public void Remove<TEntity>(TEntity entity) where TEntity : class
        {
            context.Set<TEntity>().Remove(entity);
        }
    }

    public class AdoBatch : IAdoBatch
    {
        private readonly DbContext context;
        private readonly Action<object> setAudit;

        public AdoBatch(DbContext context, Action<object> setAudit)
        {
            this.context = context;
            this.setAudit = setAudit;
        }

        public void RemoveAll<TEntity>() where TEntity : class
        {
            //TODO realization
            //context.ExecuteSqlCommand("DELETE FROM tst.ParentRecordHierarchyRecordMap");
        }
    }
}
