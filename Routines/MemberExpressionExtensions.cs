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
            var type = entity.GetType();
            var popertyName = memberExpression.Member.Name;
            var propertyInfo = type.GetProperty(popertyName);
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

        #region Path based
        public class  PathesIncluding<TRootEntity> : IIncluding<TRootEntity> where TRootEntity : class
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
       
        public static void Detach<T>(T entity, Include<T> include) where T:class
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
            public readonly List<MemberExpressionNode> Nodes = new List<MemberExpressionNode>();
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
                var node = CurrentNode.Nodes.FirstOrDefault(e => e.PropertyName == name);
                if (node == null)
                    node = new MemberExpressionNode((MemberExpression)(navigationExpression.Body));
                CurrentNode.Nodes.Add(node);
                CurrentNode = node;
            }
            public void ThenIncludeAll<TMidProperty, TEntity>(Expression<Func<TMidProperty, IEnumerable<TEntity>>> navigationExpression)
            {
                ThenInclude(navigationExpression);
            }
        }
        public static T Clone<T>(T source, Include<T> include) where T : class, new()
        {
            var destination = new T();
            CopyTo(source, destination, include);
            return destination;
        }
        public static void CopyTo<T>(T source, T destination, Include<T> include) where T : class
        {
            var nodeIncluding = new NodesIncluding<T>();
            var includable = new Includable<T>(nodeIncluding);
            include.Invoke(includable);
            var node = nodeIncluding.Root;
            CopyTo(source, destination, node);
        }
        private static Tuple<object, object> CopyTo(
            object source,
            object destination,
            MemberExpression memberExpression)
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

        private static void CopyTo(
            object source,
            object destination,
            List<MemberExpressionNode> nodes)
        {
            foreach (var node in nodes)
            {
                var values = CopyTo(source, destination, node.MemberExpression);
                var sourceValue = values.Item1;
                var destinationValue = values.Item2;
                if (sourceValue != null)
                {
                    if (destinationValue is Array)
                    {
                        var destinationArray = (Array)destinationValue;
                        var sourceArray = (Array)sourceValue;
                        if (destinationArray.Length != sourceArray.Length)
                            throw new InvalidOperationException($"Destination array '{node.PropertyName}' is not null and its length differs from source array's length");
                        for (int i = 0; i < sourceArray.Length; i++)
                        {
                            object destinationItem = default(object);
                            var sourceItem = sourceArray.GetValue(i);
                            if (sourceItem != null)
                            {
                                if (sourceItem is string || !sourceItem.GetType().IsClass)
                                {
                                    destinationItem = sourceItem;
                                }
                                else
                                {
                                    var constructor = sourceItem.GetType().GetConstructor(Type.EmptyTypes);
                                    destinationItem = constructor.Invoke(null);
                                    CopyTo(sourceItem, destinationItem, node.Nodes);
                                }
                            }
                            destinationArray.SetValue(destinationItem, i);
                        }
                    }
                    else if (destinationValue is IList)
                    {
                        var destinationList = (IList)destinationValue;
                        destinationList.Clear();
                        foreach (var sourceItem in (IEnumerable)sourceValue)
                        {
                            object destinationItem = default(object);
                            if (sourceItem != null)
                            {
                                if (sourceItem is string || !sourceItem.GetType().IsClass)
                                {
                                    destinationItem = sourceItem;
                                }
                                else
                                {
                                    var constructor = sourceItem.GetType().GetConstructor(Type.EmptyTypes);
                                    destinationItem = constructor.Invoke(null);
                                    CopyTo(sourceItem, destinationItem, node.Nodes);
                                }
                            }
                            destinationList.Add(destinationItem);
                        }
                    }
                    else
                    {
                        CopyTo(sourceValue, destinationValue, node.Nodes);
                    }
                }
            }
        }

        public static bool Equals<T>(T entity1, T entity2, Include<T> include) where T : class, new()
        {
            var nodeIncluding = new NodesIncluding<T>();
            var includable = new Includable<T>(nodeIncluding);
            include.Invoke(includable);
            var node = nodeIncluding.Root;
            return Equals(entity1, entity2, node);
        }
        private static bool Equals(
            object entity1,
            object entity2,
            List<MemberExpressionNode> nodes)
        {
            bool @value = true;
            foreach (var node in nodes)
            {
                var entity1Value = node.MemberExpression.GetMemberValue(entity1);
                var entity2Value = node.MemberExpression.GetMemberValue(entity2);
                if (entity1Value == null && entity2Value == null)
                {
                    continue;
                }
                else if ((entity1Value == null && entity2Value != null) || (entity2Value == null && entity1Value != null))
                {
                    @value = false;
                    break;
                }
                else
                {
                    if (entity1Value is Array && entity2Value is Array)
                    {
                        var entity1Array = (Array)entity1Value;
                        var entity2Array = (Array)entity2Value;
                        if (entity1Array.Length != entity2Array.Length)
                        {
                            @value = false;
                            break;
                        }
                        for (int i = 0; i < entity2Array.Length; i++)
                        {
                            var entity2Item = entity2Array.GetValue(i);
                            var entity1Item = entity1Array.GetValue(i);
                            if (node.Nodes.Count() == 0)
                                @value = Equals(entity1Item, entity2Item);
                            else
                                @value = Equals(entity1Item, entity2Item, node.Nodes);
                            if (@value == false)
                                break;
                        }
                        if (@value == false)
                            break;
                    }
                    else if (entity1Value is IList && entity2Value is IList)
                    {
                        var entity1List = (IList)entity1Value;
                        var entity2List = (IList)entity2Value;
                        if (entity1List.Count != entity2List.Count)
                        {
                            @value = false;
                            break;
                        }
                        for (int i = 0; i < entity2List.Count; i++)
                        {
                            var entity2Item = entity2List[i];
                            var entity1Item = entity1List[i];

                            if (node.Nodes.Count() == 0)
                                @value = Equals(entity1Item, entity2Item);
                            else
                                @value = Equals(entity1Item, entity2Item, node.Nodes);
                            if (@value == false)
                                break;
                        }
                        if (@value == false)
                            break;
                    }
                    else
                    {

                        if (node.Nodes.Count() == 0)
                            @value = Equals(entity1Value, entity2Value);
                        else
                            @value = Equals(entity1Value, entity2Value, node.Nodes);
                        if (@value == false)
                            break;
                    }
                }
                if (@value == false)
                    break;
            }
            return @value;
        }
        #endregion

        public static readonly List<Type> SystemTypes = new List<Type>
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
