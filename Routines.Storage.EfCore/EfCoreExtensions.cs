using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DashboardCode.Routines.Storage.EfCore
{
    public static class EfCoreExtensions
    {
        public static IQueryable<T> Include<T>(this IQueryable<T> query, Include<T> include) where T: class
        {
            var iState = new QueryableIncluding<T>(query);
            var includable = new Chain<T>(iState);
            include?.Invoke(includable);
            return iState.Queryable;
        }
        public static void Detach<T>(this DbContext context, T entity, Include<T> include) where T : class
        {
            context.Entry(entity).State = EntityState.Detached;
            var dbSet = context.Set<T>();
            ObjectExtensions.Detach(entity, include);
        }

        public static void AppendKeyLeafs<T>(this Include<T> include, DbContext context) where T : class
        {
            foreach (var entityType in context.Model.GetEntityTypes())
            {
                var properties = entityType.GetProperties().ToList();
                entityType.FindKey(properties);
                //entityType.FindAnnotation(properties);
                
            }
        }
    }
}
