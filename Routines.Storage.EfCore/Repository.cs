using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Vse.Routines.Storage.EfCore
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly DbContext context;
        private readonly bool asNoTracking;

        public Repository(DbContext context, bool asNoTracking)
        {
            this.context = context;
            this.asNoTracking = asNoTracking;
        }
        public IQueryable<TEntity> GetQueryable(Include<TEntity> addIncludes)
        {
            var dbSet = context.Set<TEntity>();
            IQueryable<TEntity> query;
            if (asNoTracking)
                query = dbSet.AsNoTracking();
            else
                query = dbSet.AsQueryable();
            query = query.Include(addIncludes);
            return query;
        }

        public IReadOnlyCollection<TEntity> ToList(Include<TEntity> include = null)
        {
            var queryable = GetQueryable(include);
            var list = queryable.ToList();
            return list;
        }

        public IReadOnlyCollection<TEntity> ToList(Expression<Func<TEntity, bool>> predicate, Include<TEntity> include = null)
        {
            var queryable = GetQueryable(include);
            var list = queryable.Where(predicate).ToList();
            return list;
        }

        public TEntity Find(Expression<Func<TEntity, bool>> predicate, Include<TEntity> include = null)
        {
            var queryable = GetQueryable(include);
            var list = queryable.Where(predicate).SingleOrDefault();
            return list;
        }

        public IRepository<TNewBaseEntity> Rebase<TNewBaseEntity>() where TNewBaseEntity : class
        {
            return new Repository<TNewBaseEntity>(this.context, asNoTracking);
        }

        public void Detach(TEntity entity, Include<TEntity> include = null)
        {
            context.Detach(entity, include);
        }

        public void Detach(IEnumerable<TEntity> entities, Include<TEntity> include = null)
        {
            foreach (var entity in entities)
            {
                context.Detach(entity, include);
            }
        }
    }

    public class Storage<TEntity> : IStorage<TEntity> where TEntity : class
    {
        private readonly DbContext context;
        private readonly Func<Exception, List<FieldError>> analyzeException;
        private readonly Action<object> setAudit;

        public Storage(
            DbContext context, 
            Func<Exception, List<FieldError>> analyzeException,
            Action<object> setAudit)
        {
            this.context = context;
            this.analyzeException = analyzeException;
            this.setAudit = setAudit;
        }

        public StorageError Handle(Action<IBatch<TEntity>> action)
        {
            var batch = new Batch<TEntity>(context, setAudit);
            try
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    action(batch);
                    context.SaveChanges();
                    transaction.Commit();
                }
            }
            catch (Exception exception)
            {
                var list = analyzeException(exception);
                if (list.Count > 0)
                {
                    return new StorageError(exception, list);
                }
                throw;
            }
            return null;
        }
    }
    public class Batch<TEntity> : IBatch<TEntity> where TEntity : class
    {
        private readonly DbContext context;
        private readonly Action<object> setAudit;

        public Batch(DbContext context,  Action<object> setAudit)
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
