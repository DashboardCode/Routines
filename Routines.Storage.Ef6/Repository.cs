using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace DashboardCode.Routines.Storage.Ef6
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

        public IQueryable<TEntity> MakeQueryable(Include<TEntity> include)
        {
            var dbSet = context.Set<TEntity>();
            IQueryable<TEntity> query;
            if (asNoTracking)
                query = dbSet.AsNoTracking();
            else
                query = dbSet.AsQueryable();
            if (include!=null)
                query = query.Include(include);
            return query;
        }

        public IReadOnlyCollection<TEntity> List(Include<TEntity> include = null)
        {
            var queryable = MakeQueryable(include);
            var list = queryable.ToList();
            return list;
        }

        public Task<List<TEntity>> ListAsync(Include<TEntity> include = null)
        {
            var queryable = MakeQueryable(include);
            var list = queryable.ToListAsync();
            return list;
        }

        public IReadOnlyCollection<TEntity> List(Expression<Func<TEntity, bool>> predicate, Include<TEntity> include = null)
        {
            var queryable = MakeQueryable(include);
            var list = queryable.Where(predicate).ToList();
            return list;
        }

        public async Task<IReadOnlyCollection<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate, Include<TEntity> include = null)
        {
            var queryable = MakeQueryable(include);
            var list = await queryable.Where(predicate).ToListAsync();
            return list;
        }


        public TEntity Find(Expression<Func<TEntity, bool>> predicate, Include<TEntity> include = null)
        {
            var queryable = MakeQueryable(include);
            var list = queryable.Where(predicate).SingleOrDefault();
            return list;
        }

        public async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, Include<TEntity> include = null)
        {
            var queryable = MakeQueryable(include);
            var list = await queryable.Where(predicate).SingleOrDefaultAsync();
            return list;
        }


        public IRepository<TNewBaseEntity> Clone<TNewBaseEntity>() where TNewBaseEntity : class =>
            new Repository<TNewBaseEntity>(this.context, asNoTracking);

        public void Detach(TEntity entity, Include<TEntity> include = null) =>
            context.Detach(entity, include);

        public void Detach(IEnumerable<TEntity> entities, Include<TEntity> include = null)
        {
            foreach (var entity in entities)
                context.Detach(entity, include);
        }

        public IQueryable<TEntity> Query(Include<TEntity> include = null)
        {
            var queryable = MakeQueryable(include);
            return queryable;
        }

        public void LoadCollection<TRelationEntity>(TEntity entity, Expression<Func<TEntity, IEnumerable<TRelationEntity>>> getTmmExpression) where TRelationEntity : class
        {
            DbEntityEntry<TEntity> entry = context.Entry(entity);
            var name = getTmmExpression.GetMemberName();
            var col = entry.Collection(name);
            col.Load();
        }
    }
}