using System;
using System.Collections;
using System.Collections.Generic;
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
            if (propertyInfo == null)
                throw new InvalidOperationException($"Unable to get property '{popertyName}' of class '{entity.GetType().FullName}' property info ");
            if (!propertyInfo.CanRead || propertyInfo.GetIndexParameters().Length != 0)
                throw new InvalidOperationException($"Unable to get '{propertyInfo.Name}' value");
            var value = propertyInfo.GetValue(entity);
            return value;
        }
        private static void SetValue(this MemberExpression memberExpression, object entity, object propertyValue)
        {
            var type = entity.GetType();
            var popertyName = memberExpression.Member.Name;
            var propertyInfo = type.GetProperty(popertyName);
            if (propertyInfo.CanWrite && propertyInfo.GetIndexParameters().Length == 0)
                propertyInfo.SetValue(entity, propertyValue);
            else
                throw new InvalidOperationException($"Unable to set '{propertyInfo.Name}' value");
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
            public void Include<TEntity>(Expression<Func<TRootEntity, TEntity>> navigationExpression)
            {
                var name = GetMemberName(navigationExpression);
                Add(new[] { name });
            }
            public void IncludeAll<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> navigationExpression)
            {
                Include(navigationExpression);
            }
            public void ThenInclude<TMidProperty, TEntity>(Expression<Func<TMidProperty, TEntity>> navigationExpression)
            {
                var name = GetMemberName(navigationExpression);
                var path = Pathes.Last();
                var newPath = path.Concat(new[] { name }).ToArray();
                Add(newPath);
            }
            public void ThenIncludeAll<TMidProperty, TEntity>(Expression<Func<TMidProperty, IEnumerable<TEntity>>> navigationExpression)
            {
                ThenInclude(navigationExpression);
            }
            private void Add(string[] path)
            {
                if (!Pathes.Any(e => e.SequenceEqual(path)))
                    Pathes.Add(path);
            }
        }

        public static void Detach<T>(T entity, Include<T> include) where T : class
        {
            var including = new PathesIncluding<T>();
            var includable = new Includable<T>(including);
            include.Invoke(includable);
            var pathes = including.Pathes;
            Detach(entity, pathes);
        }

        private static void Detach(object entity, List<string[]> allowedPaths)
        {
            var type = entity.GetType();
            if (entity is IEnumerable)
            {
                foreach (var value in (IEnumerable)entity)
                {
                    if (value != null)
                    {
                        Detach(value, allowedPaths);
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
                                var selectedPaths = allowedPaths.Where(e => e[0] == propertyName);
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
                                    Detach(value, newPaths);
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
            public void ThenInclude<TMidProperty, TEntity>(Expression<Func<TMidProperty, TEntity>> navigationExpression)
            {
                var name = GetMemberName(navigationExpression);
                var node = CurrentNode.Children.FirstOrDefault(e => e.PropertyName == name);
                if (node == null)
                    node = new MemberExpressionNode((MemberExpression)(navigationExpression.Body));
                CurrentNode.Children.Add(node);
                CurrentNode = node;
            }
            public void ThenIncludeAll<TMidProperty, TEntity>(Expression<Func<TMidProperty, IEnumerable<TEntity>>> navigationExpression)
            {
                ThenInclude(navigationExpression);
            }
        }

        public static T Clone<T>(T source, Include<T> include, IReadOnlyCollection<Type> systemTypes = null) where T : class, new()
        {
            var destination = new T();
            CopyTo(source, destination, include, systemTypes);
            return destination;
        }

        public static void CopyTo<T>(T source, T destination, Include<T> include, IReadOnlyCollection<Type> systemTypes=null) where T : class
        {
            var nodeIncluding = new NodesIncluding<T>();
            var includable = new Includable<T>(nodeIncluding);
            include.Invoke(includable);
            var nodes = nodeIncluding.Root;
            CopyTo(source, destination, nodes, systemTypes);
        }

        public static bool Equals<T>(T entity1, T entity2, Include<T> include) where T : class, new()
        {
            var nodeIncluding = new NodesIncluding<T>();
            var includable = new Includable<T>(nodeIncluding);
            include.Invoke(includable);
            var nodes = nodeIncluding.Root;
            return Equals(entity1, entity2, nodes);
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

        private static object CopyItem(object sourceItem, List<MemberExpressionNode> nodes, IReadOnlyCollection<Type> systemTypes)
        {
            if (sourceItem == null || sourceItem is string || !sourceItem.GetType().IsClass)
            {
                return sourceItem;
            }
            else
            {
                var constructor = sourceItem.GetType().GetConstructor(Type.EmptyTypes);
                var destinationItem = constructor.Invoke(null);
                CopyTo(sourceItem, destinationItem, nodes, systemTypes);
                return destinationItem;
            }
        }
       
        private static void CopyTo(
            object source,
            object destination,
            List<MemberExpressionNode> nodes,
            IReadOnlyCollection<Type> systemTypes)
        {
            if (source is Array)
            {
                CopyArray((Array)source, (Array)destination,  (sourceItem)=> CopyItem(sourceItem, nodes, systemTypes));
            }
            else if (source is IList)
            {
                CopyList((IEnumerable)source, ((IList)destination), (sourceItem) => CopyItem(sourceItem, nodes, systemTypes));
            }
            else if (source is IEnumerable && source.GetType().GetInterfaces().Any(t =>
                    t.IsGenericType &&
                    t.GetGenericTypeDefinition() == typeof(ISet<>)))
            {
                CopySet((IEnumerable)source, (object)destination, (sourceItem) => CopyItem(sourceItem, nodes, systemTypes));
            }
            else
            {

                if (systemTypes != null)
                    CopySimpleTypesProperties(source, destination, systemTypes);
                foreach (var node in nodes)
                {
                    var value = node.MemberExpression.CopyMemberValue(source, destination);
                    if (value.Item1 != null)
                        CopyTo(value.Item1, value.Item2, node.Children, systemTypes); // recursive
                }
            }
        }

        private static bool EqualsItem(object entity1Item, object entity2Item, List<MemberExpressionNode> nodes)
        {
            if (nodes.Count() == 0)
                return Equals(entity1Item, entity2Item);
            else
                return Equals(entity1Item, entity2Item, nodes);
        }
        private static bool Equals(
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
                    var copied = node.MemberExpression.CopyMemberValue(entity1, entity2);
                    if (copied.Item1 != null)
                        @value = Equals(copied.Item1, copied.Item2, node.Children); // recursive
                    if (@value == false)
                        break;
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
}
