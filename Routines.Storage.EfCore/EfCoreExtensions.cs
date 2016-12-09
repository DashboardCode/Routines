using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Vse.Routines.Storage.EfCore
{
    public static class EfCoreExtensions
    {
        public static IQueryable<T> Include<T>(this IQueryable<T> query, Include<T> include) where T: class
        {
            var iState = new QueryableIncluding<T>(query);
            var includable = new Includable<T>(iState);
            include?.Invoke(includable);
            return iState.Queryable;
        }
        public static void Detach<T>(this DbContext context, T entity, Include<T> include) where T : class
        {
            context.Entry(entity).State = EntityState.Detached;
            var dbSet = context.Set<T>();
            MemberExpressionExtensions.Detach(entity, include);
        }
    }
}
