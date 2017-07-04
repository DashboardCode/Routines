using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using System.Linq.Expressions;

namespace Vse.Routines
{
    public static class IncludeExtensions
    {
        public static Include<T> CloneInclude<T>(Include<T> source) where T : class
        {
            var visitor = new ChainVisitor<T>();
            var chain = new Chain<T>(visitor);
            source.Invoke(chain);
            var root = visitor.Root;

            var parents = new ChainPropertyNode[0];
            var entityType = root.Type;
            Type rootChainType = typeof(Chain<>).MakeGenericType(entityType);
            ParameterExpression tParameterExpression = Expression.Parameter(rootChainType, "t");
            int number = AddLevel(root, parents, 0, tParameterExpression, out Expression outExpression);
            var lambdaExpression = Expression.Lambda<Include<T>>(outExpression, new[] { tParameterExpression });
            var destination = lambdaExpression.Compile();
            return destination;
        }

        public static Include<T> CreateInclude<T>(ChainNode root) where T : class
        {
            var parents = new ChainPropertyNode[0];
            var entityType = root.Type;
            Type rootChainType = typeof(Chain<>).MakeGenericType(entityType);
            ParameterExpression tParameterExpression = Expression.Parameter(rootChainType, "t");
            int number = AddLevel(root, parents, 0, tParameterExpression, out Expression outExpression);
            var lambdaExpression = Expression.Lambda<Include<T>>(outExpression, new[] { tParameterExpression });
            var destination = lambdaExpression.Compile();
            return destination;
        }

        public static Include<T> AppendLeafs<T>(Include<T> source) where T : class
        {
            var parser = new ChainVisitor<T>();
            var train = new Chain<T>(parser);
            source.Invoke(train);
            var root = parser.Root;

            void AppendLeafs(ChainNode node)
            {
                var containsLeafs = node.Children.Values.Any(c => c.Children.Count == 0);
                if (!containsLeafs)
                {
                    //TODO: compare performance
                    //var childProperties = MemberExpressionExtensions.GetSimpleProperties(propertyType, SystemTypesExtensions.SystemTypes);

                    var childProperties = MemberExpressionExtensions.GetPrimitiveOrSimpleProperties(
                        node.Type,
                        SystemTypesExtensions.DefaultSimpleTextTypes,
                        SystemTypesExtensions.DefaultSimpleSymbolTypes);
                    foreach (var propertyInfo in childProperties)
                    {
                        var expression = CreatePropertyLambda(propertyInfo.DeclaringType, propertyInfo);
                        node.Children.Add(propertyInfo.Name, new ChainPropertyNode(propertyInfo.PropertyType, expression, propertyInfo,/* node,*/ propertyInfo.Name, false));
                    }
                }

                foreach (var n in node.Children.Values)
                    AppendLeafs(n);
            }
            AppendLeafs(root);
            var destination = CreateInclude<T>(root);
            return destination;
        }


        private static int AddLevel(ChainNode root, ChainPropertyNode[] parents, int number, Expression inExpression,  out Expression outExpression)
        {
            var @value = number;
            var node = parents.Length==0?root:parents.Last();
            outExpression = null;
            foreach (var childPair in node.Children)
            {
                var propertyNode = childPair.Value;

                var modifiedParents = new ChainPropertyNode[parents.Length + 1];
                parents.CopyTo(modifiedParents, 0);
                modifiedParents[parents.Length] = propertyNode;

                if (propertyNode.Children.Count != 0)
                {
                    @value = AddLevel(root, modifiedParents, @value, inExpression, out outExpression);
                    inExpression = outExpression;
                }
                else
                {
                    @value = AddProperty(root, modifiedParents, @value, inExpression, out outExpression);
                    inExpression = outExpression;
                }
            }
            return @value;
        }

        private static int AddProperty(ChainNode root, ChainPropertyNode[] parents, int number, Expression inExpression, out Expression outExpression)
        {
            bool isRoot = true;
            outExpression = null;
            if (parents.Length == 0)
                throw new Exception("impossible situation");
            var head = root;
            foreach (var p in parents)
            {
                MethodInfo includeMethodInfo=null;
                var isEnumerable = p.IsEnumerable;
                if (isRoot)
                {
                    if (isEnumerable)
                    {
                        Type rootChainType = typeof(Chain<>).MakeGenericType(root.Type);
                        MethodInfo includeGenericMethodInfo = rootChainType.GetTypeInfo().GetDeclaredMethod(nameof(Chain<object>.IncludeAll));
                        includeMethodInfo = includeGenericMethodInfo.MakeGenericMethod(p.Type);
                    }
                    else
                    {
                        Type rootChainType = typeof(Chain<>).MakeGenericType(root.Type);
                        MethodInfo includeGenericMethodInfo = rootChainType.GetTypeInfo().GetDeclaredMethod(nameof(Chain<object>.Include));
                        includeMethodInfo = includeGenericMethodInfo.MakeGenericMethod(p.Type);
                    }
                    isRoot = false;
                }
                else
                {
                    if (isEnumerable)
                    {
                        Type rootChainType = typeof(ThenChain<,>).MakeGenericType(root.Type, head.Type);
                        MethodInfo includeGenericMethodInfo = rootChainType.GetTypeInfo().GetDeclaredMethod(nameof(ThenChain<object, object>.ThenIncludeAll));
                        includeMethodInfo = includeGenericMethodInfo.MakeGenericMethod(p.Type);
                    }
                    else
                    {
                        Type rootChainType = typeof(ThenChain<,>).MakeGenericType(root.Type, head.Type);
                        MethodInfo includeGenericMethodInfo = rootChainType.GetTypeInfo().GetDeclaredMethod(nameof(ThenChain<object, object>.ThenInclude));
                        includeMethodInfo = includeGenericMethodInfo.MakeGenericMethod(p.Type);
                    }
                }
                var propertyLambda = p.Expression;

                var methodCallExpression = Expression.Call(inExpression, includeMethodInfo, new[] { propertyLambda });
                inExpression = methodCallExpression;

                isRoot = false;
                head = p;
            }
            outExpression = inExpression;
            return number+1;
        }



        public static LambdaExpression CreatePropertyLambda(Type declaringType, PropertyInfo propertyInfo)
        {
            ParameterExpression eParameterExpression = Expression.Parameter(declaringType, "e");
            var propertyCallExpression = Expression.Property(
                eParameterExpression,
                propertyInfo
                );
            var propertyLambda = Expression.Lambda(propertyCallExpression, new[] { eParameterExpression });
            return propertyLambda;
        }

        public static bool IncludeEquals<T>(Include<T> include1, Include<T> include2)
        {
            var state = new PathesChainingState<T>();
            var chain1 = new Chain<T>(state);
            include1.Invoke(chain1);
            var pathes1 = state.Pathes;

            var parser2 = new PathesChainingState<T>();
            var chain2 = new Chain<T>(parser2);
            include2.Invoke(chain2);
            var pathes2 = parser2.Pathes;

            bool @value = true;
            if (pathes1.Count!=pathes2.Count)
            {
                @value = false;
            }
            else
            {
                foreach(var p1 in pathes1)
                {
                    var found = false;
                    foreach (var p2 in pathes2)
                    {
                        if (p1.SequenceEqual(p2))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        @value = false;
                        break;
                    }
                }
            }
            
            return @value;
        }


        #region Path based

        public static void Detach<T>(T entity, Include<T> include) where T : class
        {
            var parser = new PathesChainingState<T>();
            var chain = new Chain<T>(parser);
            include.Invoke(chain);
            var pathes = parser.Pathes;
            DetachPaths(entity, pathes);
        }

        public static void DetachAll<TCol, T>(IEnumerable<T> entities, Include<T> include) where TCol : IEnumerable<T>
        {
            var including = new PathesChainingState<T>();
            var includable = new Chain<T>(including);
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
            var pathesIncluding1 = new PathesChainingState<T>();
            var includable1 = new Chain<T>(pathesIncluding1);
            include1.Invoke(includable1);
            var pathes1 = pathesIncluding1.Pathes;

            var pathesIncluding2 = new PathesChainingState<T>();
            var includable2 = new Chain<T>(pathesIncluding2);
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

        #region ChainNode IsSupersetOf, IsSubsetOf
        private static ChainPropertyNode FindChild(this IEnumerable<ChainPropertyNode> lists, ChainPropertyNode node)
        {
            foreach (var n in lists)
                if (n.PropertyName == node.PropertyName)
                    return n;
            return null;
        }


        private static bool Contains(this ChainNode node1, ChainNode node2)
        {
            var found = true;
            var children1 = node1.Children.Values;
            var children2 = node2.Children.Values;
            if (children1.Count == 0 && children2.Count == 0)
                return found;
            foreach (var n in children2)
            {
                var f = FindChild(node1.Children.Values, n);
                if (f != null)
                {
                    found = Contains(n, f);
                }
                else
                {
                    found = false;
                }
                if (found == false)
                    break;
            }
            return found;
        }

        public static bool IsSupersetOf(this ChainNode node1, ChainNode node2)
        {
            var @value = Contains(node1, node2);
            return @value;
        }

        public static bool IsSubsetOf(this ChainNode node1, ChainNode node2)
        {
            var @value = IsSupersetOf(node2, node1);
            return @value;
        }

        //public static ChainNode Union(ChainNode source, ChainNode target)
        //{
        //    foreach (var n in source.Children.Values)
        //    {
        //        // see if there is a match in target
        //        var f = FindChild(source.Children.Values, n); // match paths
        //        if (f == null)
        //        { // no match was found so add n to the target
        //            target.Children.Add(n.PropertyName, n.Clone(target));
        //        }
        //        else
        //        {
        //            // a match was found so add the children of match 
        //            Union(n, f);
        //        }
        //    }
        //    return target;
        //}
        #endregion

        //public static Include<T> Union<T>(this Include<T> include1, Include<T> include2)
        //{
        //    var visitor1 = new ChainVisitor<T>();
        //    var chain1 = new Chain<T>(visitor1);
        //    include1.Invoke(chain1);
        //    var root1 = visitor1.Root;

        //    var visitor2 = new ChainVisitor<T>();
        //    var chain2 = new Chain<T>(visitor2);
        //    include2.Invoke(chain2);
        //    var root2 = visitor1.Root;

        //    foreach (var c in root2.Children)
        //    {
        //        if (c.)
        //        {

        //        }
        //    }
        //}

        public static Include<T> UnionState<T>(this Include<T> include1, Include<T> include2)
        {
            Include<T> include = chain => {
                include1(chain);
                include2(chain);
            };
            return include;
        }
        #endregion

        public static IEnumerable<Type> GetTypes<T>(Include<T> include)
        {
            var nodeIncluding = new MemberExpressionChainParser<T>();
            var includable = new Chain<T>(nodeIncluding);
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
                var nodeIncluding = new MemberExpressionChainParser<T>();
                var includable = new Chain<T>(nodeIncluding);
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
                var nodeIncluding = new MemberExpressionChainParser<T>();
                var includable = new Chain<T>(nodeIncluding);
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
                var nodeIncluding = new MemberExpressionChainParser<T>();
                var includable = new Chain<T>(nodeIncluding);
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
                var nodeIncluding = new MemberExpressionChainParser<T>();
                var includable = new Chain<T>(nodeIncluding);
                include.Invoke(includable);
                nodes = nodeIncluding.Root;
            }
            return MemberExpressionExtensions.EqualsNodes(entity1, entity2, nodes);
        }
    }
}

