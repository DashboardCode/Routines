using System;
using System.Linq.Expressions;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DashboardCode.Routines.Storage.EfCore
{
    public static class DbContextExtensions
    {
        public static void Detach<T>(this DbContext context, T entity, Include<T> include) where T : class
        {
            context.Entry(entity).State = EntityState.Detached;
            var dbSet = context.Set<T>();
            ObjectExtensions.Detach(entity, include);
        }

        public static void LoadCollection<TEntity, TRelationEntity>(
            this DbContext context,
            TEntity entity,
            Expression<Func<TEntity, IEnumerable<TRelationEntity>>> getTmmExpression
            ) where TEntity : class where TRelationEntity : class
        {
            EntityEntry<TEntity> entry = context.Entry(entity);
            var col = entry.Collection(getTmmExpression);
            col.Load();
        }
    }
}
