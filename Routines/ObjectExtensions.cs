using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DashboardCode.Routines
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Set to null all properties except included
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="include"></param>
        public static void Detach<T>(T entity, Include<T> include) where T : class
        {
            var chainNode = include.CreateChainNode();
            var paths = ChainNodeTree.ListLeafKeyPaths(chainNode);
            DetachRecursive(entity, paths);
        }

        public static void DetachAll<TCol, T>(IEnumerable<T> entities, Include<T> include) where TCol : IEnumerable<T>
        {
            var chainNode = include.CreateChainNode();
            var paths = ChainNodeTree.ListLeafKeyPaths(chainNode);
            foreach (var entity in entities)
            {
                if (entity != null)
                    DetachRecursive(entity, paths);
            }
        }

        public static void Detach2<T>(T entity, Include<T> include) where T : class
        {
            var chainNode = include.CreateChainNode();
            var paths = ChainNodeTree.ListLeafKeyPaths(chainNode);
            DetachRecursive2(entity, paths);
        }

        private static void DetachRecursive(object entity, IReadOnlyCollection<string[]> allowedPaths)
        {
            IReadOnlyCollection<string[]> ModifyPaths(string propertyName, IReadOnlyCollection<string[]> source)
            {
                var destination = new List<string[]>();
                foreach (var path in allowedPaths)
                {
                    var root = path[0];
                    if (root == propertyName)
                    {
                        if (path.Length > 1)
                        {
                            var newPath = new string[path.Length - 1];
                            newPath = path.Skip(1).ToArray();
                            destination.Add(newPath);
                        }
                    }
                }
                return destination;
            }

            var type = entity.GetType();
            if (entity is IEnumerable enumerableEntity)
            {
                foreach (var value in enumerableEntity)
                {
                    if (value != null)
                    {
                        DetachRecursive(value, allowedPaths);
                    }
                }
            }
            else
            {
                var properties = type.GetTypeInfo().DeclaredProperties;
                foreach (var propertyInfo in properties)
                {
                    if (propertyInfo.CanRead && propertyInfo.CanWrite && propertyInfo.GetIndexParameters().Length == 0)
                    {
                        //if ( !SystemTypesExtensions.Contains(propertyInfo.PropertyType) )
                        //{
                            string propertyName = propertyInfo.Name;
                            var value = propertyInfo.GetValue(entity, null);
                            if (value != null)
                            {
                                var selectedPaths = allowedPaths.Where(e => e[0] == propertyName).ToList();
                                if (selectedPaths.Count() > 0)
                                {
                                    var newPaths = ModifyPaths(propertyName, allowedPaths); 
                                    DetachRecursive(value, newPaths);
                                }
                                else
                                {
                                    propertyInfo.SetValue(entity, null);
                                }
                            }
                        //}
                    }
                }
            }
        }

        private static void DetachRecursive2(object entity, IReadOnlyCollection<string[]> allowedPaths)
        {
            List<string[]> ModifyPaths(string propertyName, IReadOnlyCollection<string[]> source)
            {
                var destination = new List<string[]>();
                foreach (var path in allowedPaths)
                {
                    var root = path[0];
                    if (root == propertyName)
                    {
                        if (path.Length > 1)
                        {
                            var newPath = new string[path.Length - 1];
                            newPath = path.Skip(1).ToArray();
                            destination.Add(newPath);
                        }
                    }
                }
                return destination;
            }

            var type = entity.GetType();
            if (entity is IEnumerable enumerableEntity)
            {
                foreach (var value in enumerableEntity)
                {
                    if (value != null)
                    {
                        DetachRecursive2(value, allowedPaths);
                    }
                }
            }
            else
            {
                var properties = type.GetTypeInfo().DeclaredProperties;
                foreach (var propertyInfo in properties)
                {
                    if (propertyInfo.CanRead && propertyInfo.CanWrite && propertyInfo.GetIndexParameters().Length == 0)
                    {
                        if ( !TypeExtensions.IsSystemType(propertyInfo.PropertyType) )
                        {
                            string propertyName = propertyInfo.Name;
                            var value = propertyInfo.GetValue(entity, null);
                            if (value != null)
                            {
                                var selectedPaths = allowedPaths.Where(e => e[0] == propertyName).ToList();
                                if (selectedPaths.Count() > 0)
                                {
                                    var newPaths = ModifyPaths(propertyName, allowedPaths);
                                    DetachRecursive2(value, newPaths);
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

        public static bool Equals<T>(T entity1, T entity2, Include<T> include = null)
        {
            IEnumerable<ChainMemberNode> nodes = new List<ChainMemberNode>();
            if (include != null)
                nodes = include.CreateChainNode().Children.Values;
            return ChainNodeExtensions.EqualsNodes(entity1, entity2, nodes);
        }

        public static bool EqualsAll<TCol, T>(TCol entity1, TCol entity2, Include<T> include = null)
            where TCol: IEnumerable<T>
        {
            IEnumerable<ChainMemberNode> nodes = new List<ChainMemberNode>();
            if (include != null)
                nodes = include.CreateChainNode().Children.Values;
            return ChainNodeExtensions.EqualsNodes(entity1, entity2, nodes);
        }

        public static void Copy<T>(T source, T destination, Include<T> include = null, Func<ChainNode, IEnumerable<MemberInfo>> leafRule = null)
            where T : class
        {
            var node = include.CreateChainNode();
            var nodes = node.Children.Values;
            ChainNodeExtensions.CopyNodes(source, destination, node, nodes, leafRule);
        }

        public static void CopyAll<TCol, T>(TCol source, TCol destination, Include<T> include = null, Func<ChainNode, IEnumerable<MemberInfo>> leafRule = null)
            where TCol : IEnumerable<T>
        {
            var node = include.CreateChainNode();
            var nodes = node.Children.Values;
            ChainNodeExtensions.CopyNodes(source, destination, node, nodes, leafRule);
        }

        public static T Clone<T>(T source, Include<T> include, Func<ChainNode, IEnumerable<MemberInfo>> leafRule = null)
            where T : class
        {
            if (!(source is T))
                return default(T);
            if (leafRule == null)
                leafRule = LeafRuleManager.Default;
            var constructor = source.GetType().GetTypeInfo().DeclaredConstructors.First(e => e.GetParameters().Count() == 0);
            var destination = (T)constructor.Invoke(null);
            Copy(source, destination, include, leafRule);
            return destination;
        }

        public static TCol CloneAll<TCol, T>(TCol source, Include<T> include,
            Func<ChainNode, IEnumerable<MemberInfo>> leafRule = null)
            where TCol : class, IEnumerable<T>
        {
            if (source == null)
                return null;
            if (leafRule == null)
                leafRule = LeafRuleManager.Default;
            var typeInfo = source.GetType().GetTypeInfo();
            var constructorInfo = typeInfo.DeclaredConstructors.FirstOrDefault(e => e.GetParameters().Count() == 0);
            if (constructorInfo == null)
                 throw new NotImplementedException($"Can't clone collection '${typeInfo.Name}' because it doesn't have default constructor. Use CopyAll instead passing precreated collection as copy destination.");
            var destination = (TCol)constructorInfo.Invoke(null);
            CopyAll(source, destination, include, leafRule);
            return destination;
        }
    }
}