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
            if (include == null)
            {
                entry.State = EntityState.Modified;
            }
            else
            {
                var p = entry.GetDatabaseValues();
            }
        }

        public void Remove(TEntity entity)
        {
            context.Set<TEntity>().Remove(entity);
        }

        public void ModifyRelated<TRelationEntity>(
            TEntity entity,
            Expression<Func<TEntity, ICollection<TRelationEntity>>> getRelated,
            IEnumerable<TRelationEntity> newRelated,
            Func<TRelationEntity, TRelationEntity, bool> equalsById
            ) where TRelationEntity : class
        {
            setAuditProperties(entity); // TODO: test does it change
            DbEntityEntry<TEntity> entry = context.Entry(entity);
            var name = getRelated.GetMemberName();
            var col = entry.Collection(name);
            col.Load();

            var getRelationFunc = getRelated.Compile();
            var oldRelations = getRelationFunc(entity);
            var tmp = new List<TRelationEntity>();
            foreach (var e in oldRelations)
                if (!newRelated.Any(e2 => equalsById(e, e2)))
                    tmp.Add(e);
            foreach (var e in tmp)
                oldRelations.Remove(e);

            foreach (var e in newRelated)
                if (!oldRelations.Any(e2 => equalsById(e, e2)))
                {
                    setAuditProperties(e);
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