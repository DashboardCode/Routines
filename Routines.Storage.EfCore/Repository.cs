using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DashboardCode.Routines.Storage.EfCore
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly DbContext context;
        private readonly bool noTracking;

        public Repository(DbContext context, bool noTracking)
        {
            this.context = context;
            this.noTracking = noTracking;
        }

        public IQueryable<TEntity> Query(Include<TEntity> include)
        {
            var dbSet = context.Set<TEntity>();
            IQueryable<TEntity> query;
            if (noTracking)
                query = dbSet.AsNoTracking();
            else
                query = dbSet.AsQueryable();
            query = query.Include(include);
            return query;
        }

        public IReadOnlyCollection<TEntity> List(Include<TEntity> include = null)
        {
            var queryable = Query(include);
            var list = queryable.ToList();
            return list;
        }

        public Task<List<TEntity>> ListAsync(Include<TEntity> include = null)
        {
            var queryable = Query(include);
            var list = queryable.ToListAsync();
            return list;
        }

        public IReadOnlyCollection<TEntity> List(Expression<Func<TEntity, bool>> predicate, Include<TEntity> include = null)
        {
            var queryable = Query(include);
            var list = queryable.Where(predicate).ToList();
            return list;
        }

        public TEntity Find(Expression<Func<TEntity, bool>> predicate, Include<TEntity> include = null)
        {
            var queryable = Query(include);
            var list = queryable.Where(predicate).SingleOrDefault();
            return list;
        }

        public IRepository<TNewBaseEntity> Clone<TNewBaseEntity>() where TNewBaseEntity : class =>
            new Repository<TNewBaseEntity>(this.context, noTracking);

        public void Detach(TEntity entity, Include<TEntity> include = null) =>
            context.Detach(entity, include);

        public void Detach(IEnumerable<TEntity> entities, Include<TEntity> include = null)
        {
            foreach (var entity in entities)
                context.Detach(entity, include);
        }
    }
}