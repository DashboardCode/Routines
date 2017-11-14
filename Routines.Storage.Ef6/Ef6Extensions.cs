using System.Linq;
using System.Data.Entity;
using System;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Metadata.Edm;
using System.Reflection;

namespace DashboardCode.Routines.Storage.Ef6
{
    public static class Ef6Extensions
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

        public static Include<T> AppendModelProperties<T>(this Include<T> include, DbContext context) where T : class
        {
            void appendModelFieldsRecursive(ChainNode node, ObjectContext model)
            {
                foreach (var n in node.Children)
                    appendModelFieldsRecursive(n.Value, model);

                var entitySet = (EntitySet)(methodInfo.Invoke(model, new object[] { node.Type, "TEntity" }));
                EntityType entityType = entitySet.ElementType;
                if (entityType != null)
                {
                    var properties = entityType.Properties;
                    foreach (var p in properties)
                        if (!node.Children.ContainsKey(p.Name))
                            node.AddChild(node.Type.GetProperty(p.Name));
                }
            }
            var root = include.CreateChainNode();
            var objectContext = ((IObjectContextAdapter)context).ObjectContext;
            appendModelFieldsRecursive(root, objectContext);
            var @value = root.ComposeInclude<T>();
            return @value;
        }

        public static Include<T> AppendModelPropertiesIfEmpty<T>(this Include<T> include, DbContext context) where T : class
        {
            void appendModelFieldsRecursive(ChainNode node, ObjectContext model)
            {
                foreach (var n in node.Children)
                    appendModelFieldsRecursive(n.Value, model);

                var entitySet = (EntitySet)(methodInfo.Invoke(model, new object[] { node.Type, "TEntity" }));
                EntityType entityType = entitySet.ElementType;
                if (entityType != null)
                {
                    var properties = entityType.Properties;
                    if (!node.HasLeafs())
                        foreach (var p in properties)
                            node.AddChild(node.Type.GetProperty(p.Name));
                }
            }
            var root = include.CreateChainNode();
            var objectContext = ((IObjectContextAdapter)context).ObjectContext;
            appendModelFieldsRecursive(root, objectContext);
            var @value = root.ComposeInclude<T>();
            return @value;
        }

        public static Include<T> ExtractNavigationsAppendKeyProperties<T>(this Include<T> include, DbContext context) where T : class
        {
            void extractNavigationsAppendKeyLeafsRecursive(ChainNode node, ChainNode destination, ObjectContext model)
            {
                var entitySet = (EntitySet)(methodInfo.Invoke(model, new object[] { node.Type, "TEntity" }));
                EntityType entityType = entitySet.ElementType;
                if (entityType != null)
                {
                    var navigationPropertyInfos = entityType.NavigationProperties.Select(p => node.Type.GetProperty(p.Name)).ToList();
                    foreach (var n in node.Children)
                    {
                        var child = n.Value;
                        var propertyInfo = navigationPropertyInfos.FirstOrDefault(p => p.Name == child.PropertyName);
                        if (propertyInfo != null)
                        {
                            var childDestination = child.CloneChainPropertyNode(destination);
                            extractNavigationsAppendKeyLeafsRecursive(child, childDestination, model);
                        }
                    }

                    var keyProperties = entityType.KeyProperties;
                    foreach (var p in keyProperties)
                        destination.AddChild(node.Type.GetProperty(p.Name));
                }
            }
            var root = include.CreateChainNode();
            var destinaion = new ChainNode(root.Type);
            var objectContext = ((IObjectContextAdapter)context).ObjectContext;
            extractNavigationsAppendKeyLeafsRecursive(root, destinaion, objectContext);
            var @value = destinaion.ComposeInclude<T>();
            return @value;
        }

        #region Ways to get EntitySet 
        // 1. provided by API
        // ObjectSet<T> objectSet = objectContext.CreateObjectSet<T>();
        // EntitySet entitySet1 = objectSet.EntitySet;
        // 2. private (need reflection to objectContext.GetEntitySetForType)
        // EntitySet entitySet2 = objectContext.GetEntitySetForType(entityCLRType: typeof(TEntity), exceptionParameterName: "TEntity");
        // 3. private (need reflection to objectContext.Perspective)
        // EntitySet entitySet2 = null;
        // EntityContainer defaultContainer = objectContext.Perspective.GetDefaultContainer();
        // objectContext.MetadataWorkspace.ImplicitLoadAssemblyForType(entityCLRType, Assembly.GetCallingAssembly());
        // if (!objectContext.Perspective.TryGetTypeByName((!entityCLRType.IsNested) ? entityCLRType.FullName :
        //     entityCLRType.FullName.Replace('+', '.'), false, out TypeUsage typeUsage))
        //     throw new Exception("no mapping");
        // EdmType edmType = typeUsage.EdmType;
        // foreach (EntitySetBase current in defaultContainer.BaseEntitySets)
        //     if (current.BuiltInTypeKind == BuiltInTypeKind.EntitySet && current.ElementType == edmType)
        //         entitySet2 = (entitySet2 == null)? (EntitySet)current : throw new Exception("many mapping");
        static readonly MethodInfo methodInfo = typeof(DbContext).GetMethod("GetEntitySetForType",new[] {typeof(Type), typeof(string)});
        #endregion

        public static Include<T> ExtractNavigations<T>(this Include<T> include, DbContext context) where T : class
        {
            void extractNavigationsAppendKeyLeafsRecursive(ChainNode node, ChainNode destination, ObjectContext model)
            {
                var entitySet = (EntitySet)(methodInfo.Invoke(model, new object[] { node.Type, "TEntity" }) );
                EntityType entityType = entitySet.ElementType;
                var navigationPropertyInfos = entityType.NavigationProperties.Select(e => node.Type.GetProperty(e.Name)).ToList();
                foreach (var n in node.Children)
                {
                    var child = n.Value;
                    var propertyInfo = navigationPropertyInfos.FirstOrDefault(e => e.Name == child.PropertyName);
                    if (propertyInfo != null)
                    {
                        var childDestination = child.CloneChainPropertyNode(destination);
                        extractNavigationsAppendKeyLeafsRecursive(child, childDestination, model);
                    }
                }
            }
            var root = include.CreateChainNode();
            var destinaion = new ChainNode(root.Type);

            var objectContext = ((IObjectContextAdapter)context).ObjectContext;
            extractNavigationsAppendKeyLeafsRecursive(root, destinaion, objectContext);
            var @value = destinaion.ComposeInclude<T>();
            return @value;
        }
    }
}