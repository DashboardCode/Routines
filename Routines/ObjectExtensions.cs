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
            var chainNode = include.GetChainNode();
            var paths = ChainNodeTree.ListLeafKeyPaths(chainNode);
            DetachRecursive(entity, paths);
        }

        public static void DetachAll<TCol, T>(IEnumerable<T> entities, Include<T> include) where TCol : IEnumerable<T>
        {
            var chainNode = include.GetChainNode();
            var paths = ChainNodeTree.ListLeafKeyPaths(chainNode);
            foreach (var entity in entities)
            {
                if (entity != null)
                    DetachRecursive(entity, paths);
            }
        }

        public static void Detach2<T>(T entity, Include<T> include) where T : class
        {
            var chainNode = include.GetChainNode();
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
                        if ( !SystemTypesExtensions.Contains(propertyInfo.PropertyType) )
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
            IEnumerable<ChainPropertyNode> nodes = new List<ChainPropertyNode>();
            if (include != null)
                nodes = include.GetChainNode().Children.Values;
            return ChainNodeExtensions.EqualsNodes(entity1, entity2, nodes);
        }

        public static bool EqualsAll<TCol, T>(TCol entity1, TCol entity2, Include<T> include = null)
            where TCol: IEnumerable<T>
        {
            IEnumerable<ChainPropertyNode> nodes = new List<ChainPropertyNode>();
            if (include != null)
                nodes = include.GetChainNode().Children.Values;
            return ChainNodeExtensions.EqualsNodes(entity1, entity2, nodes);
        }

        public static void Copy<T>(T source, T destination, Include<T> include = null, IReadOnlyCollection<Type> systemTypes = null)
            where T : class
        {
            IEnumerable<ChainPropertyNode> nodes = new List<ChainPropertyNode>();
            if (include != null)
                nodes = include.GetChainNode().Children.Values;
            ChainNodeExtensions.CopyNodes(source, destination, nodes, systemTypes);
        }

        public static void CopyAll<TCol, T>(TCol source, TCol destination, Include<T> include = null, IReadOnlyCollection<Type> systemTypes = null)
            where TCol : IEnumerable<T>
        {
            IEnumerable<ChainPropertyNode> nodes = new List<ChainPropertyNode>();
            if (include != null)
                nodes = include.GetChainNode().Children.Values;
            ChainNodeExtensions.CopyNodes(source, destination, nodes, systemTypes);
        }

        public static T Clone<T>(T source, Include<T> include, IReadOnlyCollection<Type> systemTypes = null)
            where T : class
        {
            if (!(source is T))
                return default(T);
            if (systemTypes == default(IReadOnlyCollection<Type>))
                systemTypes = SystemTypesExtensions.SystemTypes;
            var constructor = source.GetType().GetTypeInfo().DeclaredConstructors.First(e => e.GetParameters().Count() == 0);
            var destination = (T)constructor.Invoke(null);
            Copy(source, destination, include, systemTypes);
            return destination;
        }

        public static TCol CloneAll<TCol, T>(TCol source, Include<T> include, IReadOnlyCollection<Type> systemTypes = null)
            where TCol : class, IEnumerable<T>
        {
            if (source == null)
                return null;
            if (systemTypes == default(IReadOnlyCollection<Type>))
                systemTypes = SystemTypesExtensions.SystemTypes;
            var constructor = source.GetType().GetTypeInfo().DeclaredConstructors.First(e => e.GetParameters().Count() == 0);
            var destination = (TCol)constructor.Invoke(null);
            CopyAll(source, destination, include, systemTypes);
            return destination;
        }
    }
}
