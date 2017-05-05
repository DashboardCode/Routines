using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Vse.Routines
{
    public static class NavigationExpressionExtensions
    {
        #region Path based

        public static void Detach<T>(T entity, Include<T> include) where T : class
        {
            var including = new PathesNavigationExpressionParser<T>();
            var includable = new Includable<T>(including);
            include.Invoke(includable);
            var pathes = including.Pathes;
            DetachPaths(entity, pathes);
        }

        public static void DetachAll<TCol, T>(IEnumerable<T> entities, Include<T> include) where TCol : IEnumerable<T>
        {
            var including = new PathesNavigationExpressionParser<T>();
            var includable = new Includable<T>(including);
            include.Invoke(includable);
            var pathes = including.Pathes;
            foreach (var entity in entities)
            {
                if (entity != null)
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
                var properties = type.GetTypeInfo().DeclaredProperties;
                foreach (var propertyInfo in properties)
                {
                    if (propertyInfo.CanRead && propertyInfo.CanWrite && propertyInfo.GetIndexParameters().Length == 0)
                    {
                        if (!SystemTypesExtensions.Contains(propertyInfo.PropertyType))
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

        public static T Clone<T>(T source, Include<T> include, IReadOnlyCollection<Type> systemTypes = null)
            where T : class
        {
            if (!(source is T))
                return default(T);
            if (systemTypes == default(IReadOnlyCollection<Type>))
                systemTypes = SystemTypesExtensions.SystemTypes;
            var constructor = source.GetType().GetTypeInfo().DeclaredConstructors.First(e => e.GetParameters().Count() == 0);
            //var destination = (T)Activator.CreateInstance(typeof(T));
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

        #region Algebra
        public static bool Contains<T>(this Include<T> include1, Include<T> include2)
        {
            var pathesIncluding1 = new PathesNavigationExpressionParser<T>();
            var includable1 = new Includable<T>(pathesIncluding1);
            include1.Invoke(includable1);
            var pathes1 = pathesIncluding1.Pathes;

            var pathesIncluding2 = new PathesNavigationExpressionParser<T>();
            var includable2 = new Includable<T>(pathesIncluding2);
            include2.Invoke(includable2);
            var pathes2 = pathesIncluding2.Pathes;

            var contains = true;
            foreach (var path2 in pathes2)
            {
                var isFound = false;
                foreach (var path1 in pathes1)
                {
                    if (path2.Length > path1.Length)
                        continue;
                    for (int i = 0; i < path2.Length; i++)
                    {
                        var member1 = path1[i];
                        var member2 = path2[i];
                        if (member1 == member2)
                        {
                            isFound = true;
                            break;
                        }
                    }
                }
                if (!isFound)
                {
                    contains = false;
                    break;
                }
            }
            return contains;
        }

        public static Include<T> Union<T>(this Include<T> include1, Include<T> include2)
        {
            Include<T> include = includable => {
                include1(includable);
                include2(includable);
            };
            return include;
        }
        #endregion

        public static IEnumerable<Type> GetTypes<T>(Include<T> include)
        {
            var nodeIncluding = new MemberNavigationExpressionParser<T>();
            var includable = new Includable<T>(nodeIncluding);
            include.Invoke(includable);
            var nodes = nodeIncluding.Root;
            var types = new List<Type>();
            nodes.FlattenMemberExpressionNode(types);
            return types;
        }

        public static void Copy<T>(T source, T destination, Include<T> include = null, IReadOnlyCollection<Type> systemTypes = null)
            where T : class
        {
            var nodes = new List<MemberExpressionNode>();
            if (include != null)
            {
                var nodeIncluding = new MemberNavigationExpressionParser<T>();
                var includable = new Includable<T>(nodeIncluding);
                include.Invoke(includable);
                nodes = nodeIncluding.Root;
            }
            MemberExpressionExtensions.CopyNodes(source, destination, nodes, systemTypes);
        }

        public static void CopyAll<TCol, T>(TCol source, TCol destination, Include<T> include = null, IReadOnlyCollection<Type> systemTypes = null)
            where TCol : IEnumerable<T>
        {
            var nodes = new List<MemberExpressionNode>();
            if (include != null)
            {
                var nodeIncluding = new MemberNavigationExpressionParser<T>();
                var includable = new Includable<T>(nodeIncluding);
                include.Invoke(includable);
                nodes = nodeIncluding.Root;
            }
            MemberExpressionExtensions.CopyNodes(source, destination, nodes, systemTypes);
        }

        public static bool Equals<T>(T entity1, T entity2, Include<T> include = null)
        {
            var nodes = new List<MemberExpressionNode>();
            if (include != null)
            {
                var nodeIncluding = new MemberNavigationExpressionParser<T>();
                var includable = new Includable<T>(nodeIncluding);
                include.Invoke(includable);
                nodes = nodeIncluding.Root;
            }
            return MemberExpressionExtensions.EqualsNodes(entity1, entity2, nodes);
        }

        public static bool EqualsAll<TCol, T>(TCol entity1, TCol entity2, Include<T> include = null)
            //where T : class
            where TCol : /*class,*/ IEnumerable<T>
        {
            var nodes = new List<MemberExpressionNode>();
            if (include != null)
            {
                var nodeIncluding = new MemberNavigationExpressionParser<T>();
                var includable = new Includable<T>(nodeIncluding);
                include.Invoke(includable);
                nodes = nodeIncluding.Root;
            }
            return MemberExpressionExtensions.EqualsNodes(entity1, entity2, nodes);
        }

    }
}
