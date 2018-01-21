using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

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

        public static Include<T> AppendModelFields<T>(this Include<T> include, IModel model) where T : class
        {
            void appendModelFieldsRecursive(ChainNode node)
            {
                foreach (var n in node.Children)
                    appendModelFieldsRecursive(n.Value);
                var entityType = model.FindEntityType(node.Type);
                if (entityType != null)
                {
                    var properties = entityType.GetProperties();
                    foreach (var p in properties)
                        if (!node.Children.ContainsKey(p.Name))
                            node.AddChild(p.PropertyInfo);
                }
            }
            var root = include.CreateChainNode();
            appendModelFieldsRecursive(root);
            var @value = root.ComposeInclude<T>();
            return @value;
        }

        public static Include<T> AppendModelFieldsIfEmpty<T>(this Include<T> include, IModel model) where T : class
        {
            void appendModelFieldsRecursive(ChainNode node)
            {
                foreach (var n in node.Children)
                    appendModelFieldsRecursive(n.Value);

                var entityType = model.FindEntityType(node.Type);
                if (entityType != null)
                {
                    var properties = entityType.GetProperties();
                    if(!node.HasLeafs())
                        foreach (var p in properties)
                            node.AddChild(p.PropertyInfo);
                }
            }
            var root = include.CreateChainNode();
            appendModelFieldsRecursive(root);
            var @value = root.ComposeInclude<T>();
            return @value;
        }

        public static Include<T> ExtractNavigationsAppendKeyLeafs<T>(this Include<T> include, IModel model) where T : class
        {
            void extractNavigationsAppendKeyLeafsRecursive(ChainNode source, ChainNode destination)
            {
                var entityType = model.FindEntityType(source.Type);
                if (entityType != null)
                {
                    var navigationPropertyInfos = entityType.GetNavigations().Select(e => e.PropertyInfo).ToList();
                    foreach (var n in source.Children)
                    {
                        var child = n.Value;
                        var propertyInfo = navigationPropertyInfos.FirstOrDefault(e => e.Name == child.PropertyName);
                        if (propertyInfo != null)
                        {
                            var childDestination = child.CloneChainPropertyNode(destination);
                            extractNavigationsAppendKeyLeafsRecursive(child, childDestination);
                        }
                    }

                    var key = entityType.FindPrimaryKey();
                    var properties = key.Properties;
                    foreach (var p in properties)
                        destination.AddChild(p.PropertyInfo);
                }
            }
            var root = include.CreateChainNode();
            var destinaion = new ChainNode(root.Type);
            extractNavigationsAppendKeyLeafsRecursive(root, destinaion);
            var @value = destinaion.ComposeInclude<T>();
            return @value;
        }

        public static Include<T> ExtractNavigations<T>(this Include<T> include, IModel model) where T : class
        {
            void extractNavigationsAppendKeyLeafsRecursive(ChainNode source, ChainNode destination)
            {
                var entityType = model.FindEntityType(source.Type);
                if (entityType != null)
                {
                    var navigationPropertyInfos = entityType.GetNavigations().Select(e => e.PropertyInfo).ToList();
                    foreach (var n in source.Children)
                    {
                        var child = n.Value;
                        var propertyInfo = navigationPropertyInfos.FirstOrDefault(e => e.Name == child.PropertyName);
                        if (propertyInfo != null)
                        {
                            var childDestination = child.CloneChainPropertyNode(destination);
                            //childDestination.AddChild(propertyInfo);
                            extractNavigationsAppendKeyLeafsRecursive(child, childDestination);
                        }
                    }
                }
            }
            var root = include.CreateChainNode();
            var destinaion = new ChainNode(root.Type);
            extractNavigationsAppendKeyLeafsRecursive(root, destinaion);
            var @value = destinaion.ComposeInclude<T>();
            return @value;
        }
    }
}