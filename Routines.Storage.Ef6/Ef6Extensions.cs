using System;
using System.Linq;
using System.Reflection;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Mapping;

namespace DashboardCode.Routines.Storage.Ef6
{
    public static class Ef6Extensions
    {
        public static IQueryable<T> Include<T>(this IQueryable<T> query, Include<T> include) where T: class
        {
            var iState = new QueryableChainVisitor<T>(query, QueryableExtensions.Include);
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
            static void appendModelFieldsRecursive(ChainNode node, ObjectContext model)
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
            static void appendModelFieldsRecursive(ChainNode node, ObjectContext model)
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
                        var propertyInfo = navigationPropertyInfos.FirstOrDefault(p => p.Name == child.MemberName);
                        if (propertyInfo != null)
                        {
                            var childDestination = child.CloneChainMemberNode(destination);
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
        // var objectContext = ((IObjectContextAdapter)context).ObjectContext;
        // ObjectSet<T> objectSet = objectContext.CreateObjectSet<T>();
        // EntitySet entitySet1 = objectSet.EntitySet;
        // 2. private (need reflection to objectContext.GetEntitySetForType)
        // var objectContext = ((IObjectContextAdapter)context).ObjectContext;
        // EntitySet entitySet2 = objectContext.GetEntitySetForType(entityCLRType: typeof(TEntity), exceptionParameterName: "TEntity");
        // 3. private (need reflection to objectContext.Perspective)
        // var objectContext = ((IObjectContextAdapter)context).ObjectContext;
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

        // 
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
                    var propertyInfo = navigationPropertyInfos.FirstOrDefault(e => e.Name == child.MemberName);
                    if (propertyInfo != null)
                    {
                        var childDestination = child.CloneChainMemberNode(destination);
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

        public static string GetTableName<TEntity>(this DbContext context) where TEntity: class
        {
            // 1. get mappings collection
            var metadataWorkspace = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;

            // 2. EF6 contains mappings in CS-Space (conceptual-storage) collection, so find "conceptual name" and search for it
            var type = typeof(TEntity);

            // assume that this is a conceptual name (in Code-first this should work)
            var entityName = type.Name;

            // NOTE: it could be more correct way to get the candadate to entityName
            // var objectItemCollection = ((ObjectItemCollection)metadataWorkspace.GetItemCollection(DataSpace.OSpace));
            // var entityTypesO = metadataWorkspace.GetItems<EntityType>(DataSpace.OSpace);
            // var entityOName = entityTypesO.Single(e => objectItemCollection.GetClrType(e) == type).Name;

            // NOTE: this can be used to test that mapping exists in CSpace (Conceptual Space) - entityName name is "entity name"
            //var entityTypesC = metadataWorkspace.GetItems(DataSpace.CSpace).Where(x => x.BuiltInTypeKind == BuiltInTypeKind.EntityType).Cast<EntityType>();
            //var entityName = entityTypesC.Single(x => x.Name == entityOName).Name;

            EntitySet entitySet = null;
            var entityContainerMappings = metadataWorkspace.GetItems<EntityContainerMapping>(DataSpace.CSSpace);
            foreach (var entityContainerMapping in entityContainerMappings)
                if (entityContainerMapping.StoreEntityContainer.TryGetEntitySetByName(entityName, true, out entitySet))
                    break;
            if (entitySet == null)
            {
                throw new Exception($"Table name not found in the EF6 model: type {0} it is not an entity or there is something that we do not know about the model browsing");
                // TODO: try this in case of problems
                //var entitySetMappings = entityContainerMappings.Single().EntitySetMappings.ToList();
                //var mapping = entitySetMappings.SingleOrDefault(x => x.EntitySet.Name == entityName);
                //if (mapping != null)
                //{
                //    entitySet = mapping.EntityTypeMappings.Single().Fragments.Single().StoreEntitySet;
                //}
                //else
                //{
                //    mapping = entitySetMappings.SingleOrDefault(x => x.EntityTypeMappings.Where(y => y.EntityType != null).Any(y => y.EntityType.Name == entityName));

                //    if (mapping != null)
                //    {
                //        entitySet = mapping.EntityTypeMappings.Where(x => x.EntityType != null).Single(x => x.EntityType.Name == entityName).Fragments.Single().StoreEntitySet;
                //    }
                //    else
                //    {
                //        var entitySetMapping = entitySetMappings.Single(x => x.EntityTypeMappings.Any(y => y.IsOfEntityTypes.Any(z => z.Name == entityName)));
                //        entitySet = entitySetMapping.EntityTypeMappings.First(x => x.IsOfEntityTypes.Any(y => y.Name == entityName)).Fragments.Single().StoreEntitySet;
                //    }
                //}
            }
            return (string.IsNullOrEmpty(entitySet.Schema)) ? entitySet.Table : (entitySet.Schema + "." + entitySet.Table);
            // TODO: try this in case of problems
            //var table = entitySet.MetadataProperties["Table"].Value as string;
            //var schema = entitySet.MetadataProperties["Schema"].Value as string;
            //return (string.IsNullOrEmpty(schema)) ? table : (schema + "." + table);
        }
    }
}