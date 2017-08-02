﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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

        public void UpdateRelations<TRelationEntity>(
            TEntity entity,
            Expression<Func<TEntity, IEnumerable<TRelationEntity>>> getRelation,
            IEnumerable<TRelationEntity> newRelations,
            Func<TRelationEntity, TRelationEntity, bool> equalsById
            ) where TRelationEntity : class
        {
            EntityEntry<TEntity> entry = context.Entry(entity);
            var col = entry.Collection(getRelation);
            col.Load();
            var getRelationFunc = getRelation.Compile();
            var enumerable = getRelationFunc(entity);
            var oldRelations = (ICollection<TRelationEntity>)enumerable;
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
}
