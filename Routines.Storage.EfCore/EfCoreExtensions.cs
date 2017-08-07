using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;

namespace DashboardCode.Routines.Storage.EfCore
{
    public static class EfCoreExtensions
    {
        public static IQueryable<T> Include<T>(this IQueryable<T> query, Include<T> include) where T: class
        {
            var iState = new QueryableChainVisitor<T>(query);
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

        public static Include<T> AppendModelFields<T>(this Include<T> include, DbContext context) where T : class
        {
            var root = include.CreateChainNode();
            AppendModelFieldsRecursive(root, context.Model);
            var @value = root.ComposeInclude<T>();
            return @value;
        }

        private static void AppendModelFieldsRecursive(ChainNode node, IModel model)
        {
            foreach (var n in node.Children)
                AppendModelFieldsRecursive(n.Value, model);
            var entityTypes = model.GetEntityTypes().Where(t => t.ClrType == node.Type).ToList();
            var entityType = entityTypes.FirstOrDefault();
            var properties = entityType.GetProperties().ToList();
            foreach (var p in properties)
                node.AddChild(p.PropertyInfo);
        }

        public static Include<T> ExtractNavigationsAppendKeyLeafs<T>(this Include<T> include, DbContext context) where T : class
        {
            foreach (var entityType in context.Model.GetEntityTypes())
            {
                var properties = entityType.GetProperties().ToList();
                entityType.FindKey(properties);
                //entityType.FindAnnotation(properties);
                
            }
            return null;
        }

        public static Include<T> ExtractNavigations<T>(this Include<T> include, DbContext context) where T : class
        {
            foreach (var entityType in context.Model.GetEntityTypes())
            {
                var properties = entityType.GetProperties().ToList();
                entityType.FindKey(properties);
                //entityType.FindAnnotation(properties);

            }
            return null;
        }
    }
}
