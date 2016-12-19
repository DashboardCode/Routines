using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace Vse.Routines
{
    public static class MemberExpressionExtensions
    {
        public static string GetMemberName<T1, T2>(Expression<Func<T1, T2>> expression)
        {
            var memberExpression = (MemberExpression)expression.Body;
            return memberExpression.Member.Name;
        }
        private static object GetMemberValue(this MemberExpression memberExpression, object entity)
        {
            var popertyName = memberExpression.Member.Name;
            var type = entity.GetType();
            
            var propertyInfo = type.GetProperty(popertyName);
            Debug.Assert(propertyInfo!=null, "propertyInfo is null");
            Debug.Assert(propertyInfo.CanRead && propertyInfo.GetIndexParameters().Length == 0, "propertyInfo can't be read");
            var value = propertyInfo.GetValue(entity);
            return value;
        }

        private static Tuple<object, object> GetMemberValues(
            this MemberExpression memberExpression,
            object entity1,
            object entity2)
        {
            var popertyName = memberExpression.Member.Name;
            var type = entity1.GetType();

            var propertyInfo = type.GetProperty(popertyName);
            Debug.Assert(propertyInfo != null, "propertyInfo is null");
            Debug.Assert(propertyInfo.CanRead && propertyInfo.GetIndexParameters().Length == 0, "propertyInfo can't be read");
            var value1 = propertyInfo.GetValue(entity1);
            var value2 = propertyInfo.GetValue(entity2);
            return new Tuple<object, object>(value1, value2);
        }

        private static Type GetMemberType(this MemberExpression memberExpression)
        {
            var type = memberExpression.Type;
            var type3 = memberExpression.Member.DeclaringType;
            var type2 = memberExpression.Member.ReflectedType;
            return type;
        }
        private static void SetValue(this MemberExpression memberExpression, object entity, object propertyValue)
        {
            var type = entity.GetType();
            var popertyName = memberExpression.Member.Name;
            var propertyInfo = type.GetProperty(popertyName);
            Debug.Assert(propertyInfo.CanWrite && propertyInfo.GetIndexParameters().Length == 0);
            propertyInfo.SetValue(entity, propertyValue);
        }

        
        private static Tuple<object, object> CopyMemberValue(
            this MemberExpression memberExpression,
            object source,
            object destination)
        {
            object copiedValue = default(object);
            var sourceValue = memberExpression.GetMemberValue(source);
            if (sourceValue != null)
            {
                var type = sourceValue.GetType();
                if (!type.IsClass || sourceValue is string)
                {
                    copiedValue = sourceValue;
                    SetValue(memberExpression, destination, copiedValue);
                }
                else
                {
                    var destinationValue = memberExpression.GetMemberValue(destination);
                    if (destinationValue == null)
                    {
                        if (type.IsArray)
                        {
                            var sourceArray = (Array)sourceValue;
                            copiedValue = Activator.CreateInstance(type, new object[] { sourceArray.Length });
                        }
                        else
                        {
                            var constructor = type.GetConstructor(Type.EmptyTypes);
                            copiedValue = constructor.Invoke(null);
                        }
                    }
                    else
                    {
                        copiedValue = destinationValue;
                    }
                    SetValue(memberExpression, destination, copiedValue);
                }
            }
            return new Tuple<object, object>(sourceValue, copiedValue);
        }
        #region Path based
        public class PathesIncluding<TRootEntity> : IIncluding<TRootEntity> where TRootEntity : class
        {
            public readonly List<string[]> Pathes = new List<string[]>();
            private string[] sequence;
            public void Include<TEntity>(Expression<Func<TRootEntity, TEntity>> navigationExpression)
            {
                var name = GetMemberName(navigationExpression);
                sequence = Add(new string[] { }, name);
            }
            public void IncludeAll<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> navigationExpression)
            {
                Include(navigationExpression);
            }
            public void ThenInclude<TThenEntity, TEntity>(Expression<Func<TThenEntity, TEntity>> navigationExpression)
            {
                var name = GetMemberName(navigationExpression);
                sequence = Add(sequence.ToArray(), name);
            }
            public void ThenIncludeAll<TThenEntity, TEntity>(Expression<Func<TThenEntity, IEnumerable<TEntity>>> navigationExpression)
            {
                ThenInclude(navigationExpression);
            }
            private string[] Add(string[] parentPath, string member)
            {
                var newPath = parentPath.Concat(new[] { member }).ToArray();
                //var pathes = Pathes.Where(e => e.SequenceEqual(path)).ToList();
                //if (path.Count() == 0)
                //    Pathes.Add(newPath);
                //else
                //{

                //}

                var subpahtes = new List<string[]>();
                //var isExists = false;
                ////var pathesToRemove
                foreach (var p in Pathes)
                {
                    var isSub = true;
                    for (int i = 0; i <= parentPath.Length; i++)
                    {
                        if (i == parentPath.Length)
                        {
                            if (p.Length > i && p[i] == member) // there is full subpath
                            {
                                goto end;
                            }
                        }
                        else
                        {
                            if (parentPath.Length < i || p[i] != parentPath[i])
                            {
                                isSub = false;
                                break;
                            }
                        }
                    }
                    if (isSub)
                        subpahtes.Add(p);
                }
                if (subpahtes.Count > 0)
                {
                    var root = Pathes.Where(e => e.SequenceEqual(parentPath)).FirstOrDefault();
                    if (root!=null)
                        Pathes.Remove(root);
                }
                Pathes.Add(newPath);
            end:
                    return newPath;
            }
        }

        public static void Detach<T>(T entity, Include<T> include) where T : class
        {
            var including = new PathesIncluding<T>();
            var includable = new Includable<T>(including);
            include.Invoke(includable);
            var pathes = including.Pathes;
            DetachPaths(entity, pathes);
        }

        public static void DetachAll<T>(IEnumerable<T> entities, Include<T> include) where T : class
        {
            var including = new PathesIncluding<T>();
            var includable = new Includable<T>(including);
            include.Invoke(includable);
            var pathes = including.Pathes;
            foreach (var entity in entities) {
                if (entity!=null)
                    DetachPaths(entity, pathes);
            }
        }

        private static void DetachPaths(object entity, List<string[]> allowedPaths)
        {
            var type = entity.GetType();
            if (entity is IEnumerable)
            {
                foreach (var value in (IEnumerable)entity)
                {
                    if (value != null)
                    {
                        DetachPaths(value, allowedPaths);
                    }
                }
            }
            else
            {
                var properties = type.GetProperties();
                foreach (var propertyInfo in properties)
                {
                    if (propertyInfo.CanRead && propertyInfo.CanWrite && propertyInfo.GetIndexParameters().Length == 0)
                    {
                        if (!SystemTypes.Contains(propertyInfo.PropertyType))
                        {
                            string propertyName = propertyInfo.Name;
                            var value = propertyInfo.GetValue(entity, null);
                            if (value != null)
                            {
                                var selectedPaths = allowedPaths.Where(e => e[0] == propertyName).ToList();
                                if (selectedPaths.Count() > 0)
                                {
                                    var newPaths = new List<string[]>();
                                    foreach (var path in allowedPaths)
                                    {
                                        var root = path[0];
                                        if (root == propertyName)
                                        {
                                            if (path.Length > 1)
                                            {
                                                var newPath = new string[path.Length - 1];
                                                newPath = path.Skip(1).ToArray();
                                                newPaths.Add(newPath);
                                            }
                                        }
                                    }
                                    DetachPaths(value, newPaths);
                                }
                                else
                                {
                                    propertyInfo.SetValue(entity, null);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Nodes based
        public class MemberExpressionNode
        {
            public readonly string PropertyName;
            public readonly MemberExpression MemberExpression;
            public readonly List<MemberExpressionNode> Children = new List<MemberExpressionNode>();
            public MemberExpressionNode(MemberExpression memberExpression)
            {
                PropertyName = memberExpression.Member.Name;
                MemberExpression = memberExpression;
            }
        }
        public class NodesIncluding<TRootEntity> : IIncluding<TRootEntity> where TRootEntity : class
        {
            public readonly List<MemberExpressionNode> Root = new List<MemberExpressionNode>();
            public MemberExpressionNode CurrentNode;


            public void Include<TEntity>(Expression<Func<TRootEntity, TEntity>> navigationExpression)
            {
                var name = GetMemberName(navigationExpression);
                var node = Root.FirstOrDefault(e => e.PropertyName == name);
                if (node == null)
                {
                    node = new MemberExpressionNode((MemberExpression)(navigationExpression.Body));
                    Root.Add(node);
                }
                CurrentNode = node;
            }
            public void IncludeAll<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> navigationExpression)
            {
                Include(navigationExpression);
            }
            public void ThenInclude<TThenEntity, TEntity>(Expression<Func<TThenEntity, TEntity>> navigationExpression)
            {
                var name = GetMemberName(navigationExpression);
                var node = CurrentNode.Children.FirstOrDefault(e => e.PropertyName == name);
                if (node == null)
                    node = new MemberExpressionNode((MemberExpression)(navigationExpression.Body));
                CurrentNode.Children.Add(node);
                CurrentNode = node;
            }
            public void ThenIncludeAll<TThenEntity, TEntity>(Expression<Func<TThenEntity, IEnumerable<TEntity>>> navigationExpression)
            {
                ThenInclude(navigationExpression);
            }
        }

        public static void Cast<T1, T2>(T1 t1, T2 t2, Include<T1> userDto) //where T1 : class
        {
            throw new NotImplementedException();
        }

        public static void Cast<T1, T2>(T1 t1, T2 t2, Include<T2> userDto) //where T2 : class
        {
            throw new NotImplementedException();
        }


        public static T Clone<T>(T source, Include<T> include, IReadOnlyCollection<Type> systemTypes = null) where T : class
        {
            if (source == null)
                return null;
            if (systemTypes == null)
                systemTypes=SystemTypes;
            var constructor = source.GetType().GetConstructor(Type.EmptyTypes);
            var destination = (T)constructor.Invoke(null);
            Copy(source, destination, include, systemTypes);
            return destination;
        }

        public static TCol CloneAll<TCol, T>(TCol source, Include<T> include, IReadOnlyCollection<Type> systemTypes = null) 
            where T : class
            where TCol: class, IEnumerable<T>
        {
            if (source == null)
                return null;
            if (systemTypes == null)
                systemTypes = SystemTypes;
            var constructor = source.GetType().GetConstructor(Type.EmptyTypes);
            var destination = (TCol)constructor.Invoke(null);
            CopyAll(source, destination, include, systemTypes);
            return destination;
        }

        public static IEnumerable<Type> GetTypes<T>(Include<T> include) where T : class
        {
            var nodeIncluding = new NodesIncluding<T>();
            var includable = new Includable<T>(nodeIncluding);
            include.Invoke(includable);
            var nodes = nodeIncluding.Root;
            var types = new List<Type>();
            FlattenMemberExpressionNode(nodes, types);
            return types;
        }
        private static void FlattenMemberExpressionNode(IEnumerable<MemberExpressionNode> nodes, List<Type> types)
        {
            foreach (var node in nodes)
            {
                var type = node.MemberExpression.GetMemberType();
                if (!types.Any(t=>t.AssemblyQualifiedName == type.AssemblyQualifiedName ) )
                    types.Add(type);
                FlattenMemberExpressionNode(node.Children, types);
            }
        }

        public static void Copy<T>(T source, T destination, Include<T> include=null, IReadOnlyCollection<Type> systemTypes=null) where T : class
        {
            var nodes = new List<MemberExpressionNode>();
            if (include != null)
            {
                var nodeIncluding = new NodesIncluding<T>();
                var includable = new Includable<T>(nodeIncluding);
                include.Invoke(includable);
                nodes = nodeIncluding.Root;
            }
            CopyNodes(source, destination, nodes, systemTypes);
        }

        public static void CopyAll<TCol, T>(TCol source, TCol destination, Include<T> include = null, IReadOnlyCollection<Type> systemTypes = null) 
            where T : class
            where TCol : class, IEnumerable<T>
        {
            var nodes = new List<MemberExpressionNode>();
            if (include != null)
            {
                var nodeIncluding = new NodesIncluding<T>();
                var includable = new Includable<T>(nodeIncluding);
                include.Invoke(includable);
                nodes = nodeIncluding.Root;
            }
            CopyNodes(source, destination, nodes, systemTypes);
        }

        public static bool Equals<T>(T entity1, T entity2, Include<T> include=null) where T : class
        {
            var nodes = new List<MemberExpressionNode>();
            if (include != null)
            {
                var nodeIncluding = new NodesIncluding<T>();
                var includable = new Includable<T>(nodeIncluding);
                include.Invoke(includable);
                nodes = nodeIncluding.Root;
            }
            return EqualsNodes(entity1, entity2, nodes);
        }

        public static bool EqualsAll<TCol, T>(TCol entity1, TCol entity2, Include<T> include=null) 
            where T : class
            where TCol : class, IEnumerable<T>
        {
            var nodes = new List<MemberExpressionNode>();
            if (include != null)
            {
                var nodeIncluding = new NodesIncluding<T>();
                var includable = new Includable<T>(nodeIncluding);
                include.Invoke(includable);
                nodes = nodeIncluding.Root;
            }
            return EqualsNodes(entity1, entity2, nodes);
        }

        #region Collection's itearations

        private static void CopyList(
            IEnumerable sourceEnumerable,
            IList destinationList,
            Func<object,object> copyItem)
        {
            destinationList.Clear();
            foreach (var sourceItem in sourceEnumerable)
            {
                var destinationItem = copyItem(sourceItem);
                destinationList.Add(destinationItem);
            }
        }

        private static bool EqualsList(
            IList entity1List,
            IList entity2List,
            Func<object, object, bool> equals)
        {
            var @value = true;
            if (entity1List.Count != entity2List.Count)
            {
                @value = false;
            }
            else
            {
                for (int i = 0; i < entity2List.Count; i++)
                {
                    var entity2Item = entity2List[i];
                    var entity1Item = entity1List[i];
                    @value = equals(entity1Item, entity2Item);
                    if (@value == false)
                        break;
                }
            }
            return @value;
        }

        private static void CopySet(
            IEnumerable sourceEnumerable,
            object set, //object set,
            Func<object, object> copyItem)
        {

            var clear = set.GetType().GetMethod("Clear");
            var add = set.GetType().GetMethod("Add");
            clear.Invoke(set, null);
            //set.Clear();
            foreach (var sourceItem in sourceEnumerable)
            {
                var destinationItem = copyItem(sourceItem);
                add.Invoke(set, new[] { destinationItem });
                //  set.Add(destinationItem);
            }
        }

        private static bool EqualsSet(
            object set1,
            object set2, //object set,
            Func<object, object, bool> equals)
        {
            var setType = set1.GetType();
            var countProperty = setType.GetProperty("Count");
            var count1 = (int)countProperty.GetValue(set1, null);
            var count2 = (int)countProperty.GetValue(set2, null);

            var genType = setType.GenericTypeArguments.First();
            var array1 = Array.CreateInstance(genType, count1);
            var array2 = Array.CreateInstance(genType, count2);

            var сopyTo = setType.GetMethod("CopyTo", new[] { array1.GetType() });
            сopyTo.Invoke(set1, new[] { array1 });
            сopyTo.Invoke(set2, new[] { array2 });
            var @value = EqualsArray(array1, array2, equals);
            return @value;
        }

        private static void CopyArray(
            Array sourceArray,
            Array destinationArray,
            Func<object,object> copyItem)
        {
            if (destinationArray.Length != sourceArray.Length)
                throw new InvalidOperationException($"Destination array type of '{destinationArray.GetType().FullName}' is not null and its length differs from source array's length");
            for (int i = 0; i < sourceArray.Length; i++)
            {
                var sourceItem = sourceArray.GetValue(i);
                var destinationItem = copyItem(sourceItem);
                destinationArray.SetValue(destinationItem, i);
            }
        }

        private static bool EqualsArray(
            Array entity1Array,
            Array entity2Array,
            Func<object, object, bool> equals
            )
        {
            var @value = true;
            if (entity1Array.Length != entity2Array.Length)
            {
                @value = false;
            }
            else
            {
                for (int i = 0; i < entity2Array.Length; i++)
                {
                    var entity2Item = entity2Array.GetValue(i);
                    var entity1Item = entity1Array.GetValue(i);
                    @value = equals(entity1Item, entity2Item);
                    if (@value == false)
                        break;
                }
            }
            return @value;
        }
        #endregion

        private static object CloneItem(object sourceItem, List<MemberExpressionNode> nodes, IReadOnlyCollection<Type> systemTypes)
        {
            if (sourceItem == null || sourceItem is string || !sourceItem.GetType().IsClass)
            {
                return sourceItem;
            }
            else
            {
                var constructor = sourceItem.GetType().GetConstructor(Type.EmptyTypes);
                var destinationItem = constructor.Invoke(null);
                CopyNodes(sourceItem, destinationItem, nodes, systemTypes);
                return destinationItem;
            }
        }

        private static void CopyNodes(
            object source,
            object destination,
            List<MemberExpressionNode> nodes,
            IReadOnlyCollection<Type> systemTypes)
        {
            if (source is Array)
            {
                CopyArray((Array)source, (Array)destination,  (sourceItem)=> CloneItem(sourceItem, nodes, systemTypes));
            }
            else if (source is IList)
            {
                CopyList((IEnumerable)source, ((IList)destination), (sourceItem) => CloneItem(sourceItem, nodes, systemTypes));
            }
            else if (source is IEnumerable && source.GetType().GetInterfaces().Any(t =>
                    t.IsGenericType &&
                    t.GetGenericTypeDefinition() == typeof(ISet<>)))
            {
                CopySet((IEnumerable)source, (object)destination, (sourceItem) => CloneItem(sourceItem, nodes, systemTypes));
            }
            else
            {

                if (systemTypes != null)
                    CopySimpleTypesProperties(source, destination, systemTypes);
                foreach (var node in nodes)
                {
                    var value = node.MemberExpression.CopyMemberValue(source, destination);
                    if (value.Item1 != null)
                        CopyNodes(value.Item1, value.Item2, node.Children, systemTypes); // recursion
                }
            }
        }

        private static bool EqualsItem(object entity1Item, object entity2Item, List<MemberExpressionNode> nodes)
        {
            if (entity1Item == null && entity2Item == null)
                return true;
            else if ((entity1Item != null && entity2Item == null) || (entity1Item == null && entity2Item != null))
                return false;
            if (nodes.Count() > 0)
                return EqualsNodes(entity1Item, entity2Item, nodes);
            else if (entity1Item is IEnumerable && !(entity1Item is string))
                return EqualsNodes(entity1Item, entity2Item, nodes);
            else
                return entity1Item.Equals(entity2Item);
        }
        private static bool EqualsNodes(
            object entity1,
            object entity2,
            List<MemberExpressionNode> nodes)
        {
            bool @value = true;

            if (entity1 is Array && entity2 is Array)
            {
                @value = EqualsArray((Array)entity1, (Array)entity2, (e1, e2) => EqualsItem(e1, e2, nodes));
            }
            else if (entity1 is IList && entity2 is IList)
            {
                @value = EqualsList((IList)entity1, (IList)entity2, (e1, e2) => EqualsItem(e1, e2, nodes));
            }
            else if (entity1 is IEnumerable && entity1.GetType().GetInterfaces().Any(t =>
                    t.IsGenericType &&
                    t.GetGenericTypeDefinition() == typeof(ISet<>)))
            {
                @value = EqualsSet(entity1, entity2, (e1, e2) => EqualsItem(e1, e2, nodes));
            }
            else
            {
                foreach (var node in nodes)
                {
                    var copied = node.MemberExpression.GetMemberValues(entity1, entity2);
                    @value = EqualsItem(copied.Item1, copied.Item2, node.Children);
                    if (@value == false)
                        break;
                    //if (copied.Item1 != null)
                    //    @value = EqualsNodes(copied.Item1, copied.Item2, node.Children); // recursive
                    //if (@value == false)
                    //    break;
                }
            }
            return @value;
        }
        #endregion

        public static void CopySimpleTypesProperties(object source,
            object destination, IReadOnlyCollection<Type> systemTypes = null)
        {
            var properties = destination.GetType().GetProperties();
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.CanRead && propertyInfo.CanWrite && propertyInfo.GetIndexParameters().Length == 0)
                {
                    if (systemTypes.Contains(propertyInfo.PropertyType))
                    {
                        var value = propertyInfo.GetValue(source, null);
                        propertyInfo.SetValue(destination, value);
                    }
                }
            }
        }

        public static readonly IReadOnlyCollection<Type> SystemTypes = new List<Type>
        {
                typeof(bool),
                typeof(bool?),
                typeof(byte),
                typeof(byte?),
                typeof(char),
                typeof(char?),
                typeof(decimal),
                typeof(decimal?),
                typeof(double),
                typeof(double?),
                typeof(float),
                typeof(float?),
                typeof(int),
                typeof(int?),
                typeof(long),
                typeof(long?),
                typeof(sbyte),
                typeof(sbyte?),
                typeof(short),
                typeof(short?),
                typeof(uint),
                typeof(uint?),
                typeof(ulong),
                typeof(ulong?),
                typeof(ushort),
                typeof(ushort?),
                typeof(string),
                typeof(DateTime),
                typeof(DateTime?),
                typeof(DateTimeOffset),
                typeof(DateTimeOffset?),
                typeof(Guid),
                typeof(Guid?),
                typeof(TimeSpan),
                typeof(TimeSpan?)
            };
    }

    //public static class IncludeExtensions
    //{

    //}
}
