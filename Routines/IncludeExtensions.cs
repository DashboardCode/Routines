using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using System.Linq.Expressions;

namespace Vse.Routines
{
    public class TestChildlX
    {
        public float PropertyFloat { get; set; }
        public byte[] PropertBytes { get; set; }
    }

    public class TestModelX
    {
        public TestChildlX TestChildlX { get; set; }
        public int PropertyInt { get; set; }
        public string PropertyText { get; set; }
    }
    public static class IncludeExtensions
    {
        public static void Kuku<T>(Expression<Include<T>> exp)
        {
            //__methodref();
        }

        public static void Kuku2()
        {
            Kuku<TestModelX>(t => t
                .Include(e => e.PropertyInt)
                .Include(e => e.PropertyText)
                .Include(e => e.TestChildlX).ThenInclude(e => e.PropertyFloat)
                .Include(e => e.TestChildlX).ThenInclude(e => e.PropertyFloat));
        }


        public static Include<T> AppendLeafs<T>(Include<T> source) where T : class
        {
            var parser = new SerializerChainParser<T>();
            var train = new Chain<T>(parser);
            source.Invoke(train);
            var parents = new SerializerNode[1];
            var node = parser.Root;
            parents[0]=node;
            var entityType = node.Type;
            
            //LambdaExpression propertyLambda = null;

            Type rootChainType = typeof(Chain<>).MakeGenericType(entityType);
            //MethodInfo includeGenericMethodInfo = rootChainType.GetTypeInfo().GetDeclaredMethod(nameof(Chain<object>.Include));
            //MethodInfo includeMethodInfo = includeGenericMethodInfo.MakeGenericMethod(rootChainType);

            ParameterExpression tParameterExpression = Expression.Parameter(rootChainType, "t");

            int number = AddLevel(parents, 0,  tParameterExpression,  out Expression outMethodCallExpression);

            var lambdaExpression = Expression.Lambda<Include<T>>(outMethodCallExpression, new[] {tParameterExpression });
            var destination = lambdaExpression.Compile();

            return destination;
        }

        private static int AddLevel(SerializerNode[] parents, int number, Expression includeParameterExpression,  out Expression outMethodCallExpression)
        {
            var @value = number;
            var node = parents.Last();
            outMethodCallExpression = null;
            foreach (var childPair in node.Children)
            {
                //var propertyName = childPair.Key;
                var propertyNode = childPair.Value;

                var modifiedParents = new SerializerNode[parents.Length + 1];
                parents.CopyTo(modifiedParents, 0);
                modifiedParents[parents.Length] = propertyNode;

                if (propertyNode.Children.Count != 0)
                {
                    @value = AddLevel(modifiedParents, @value, includeParameterExpression, out outMethodCallExpression);
                    includeParameterExpression = outMethodCallExpression;
                }
                else
                {
                    @value = AddProperty(/*propertyNode.Type, propertyName,*/ modifiedParents, @value, includeParameterExpression, out outMethodCallExpression);
                    includeParameterExpression = outMethodCallExpression;
                }
            }
            return @value;
        }

        private static int AddProperty(/*Type nodeType, string propertyName,*/ SerializerNode[] parents, int number, Expression includeParameterExpression, /* Expression methodCallExpression, */out Expression outMethodCallExpression)
        {
            bool isRoot = true;
            outMethodCallExpression = null;
            if (parents.Length <= 1)
                throw new Exception("impossible situation");
            var head = parents[0];
            var root = head;
            var tail = parents.Skip(1).ToArray();
            foreach (var p in tail)
            {
                MethodInfo includeMethodInfo=null;
                var isEnumerable = false;
                if (isRoot)
                {
                    if (isEnumerable)
                    {
                    }
                    else
                    {
                        if (number == 0)
                        {
                            Type rootChainType = typeof(Chain<>).MakeGenericType(root.Type);
                            MethodInfo includeGenericMethodInfo = rootChainType.GetTypeInfo().GetDeclaredMethod(nameof(Chain<object>.Include));
                            includeMethodInfo = includeGenericMethodInfo.MakeGenericMethod(p.Type);
                        }
                        else
                        {
                            Type rootChainType = typeof(/*Then*/Chain</*,*/>).MakeGenericType(root.Type/*, head.Type*/);
                            MethodInfo includeGenericMethodInfo = rootChainType.GetTypeInfo().GetDeclaredMethod(nameof(/*Then*/Chain<object/*,object*/>.Include));
                            includeMethodInfo = includeGenericMethodInfo.MakeGenericMethod(p.Type);
                        }
                    }
                    isRoot = false;
                }
                else
                {
                    if (isEnumerable)
                    {
                    }
                    else
                    {
                        Type rootChainType = typeof(ThenChain<,>).MakeGenericType(root.Type, head.Type);
                        MethodInfo includeGenericMethodInfo = rootChainType.GetTypeInfo().GetDeclaredMethod(nameof(ThenChain<object, object>.ThenInclude));
                        includeMethodInfo = includeGenericMethodInfo.MakeGenericMethod(p.Type);
                    }
                }

                ParameterExpression eParameterExpression = Expression.Parameter(head.Type, "e");
                PropertyInfo propertyMethodInfo = head.Type.GetTypeInfo().GetDeclaredProperty(((SerializerPropertyNode)p).PropertyName);

                var propertyCallExpression = Expression.Property(
                    eParameterExpression,
                    propertyMethodInfo
                    );

                var propertyLambda = Expression.Lambda(propertyCallExpression, new[] { eParameterExpression });
                var methodCallExpression = Expression.Call(includeParameterExpression, includeMethodInfo, new[] { propertyLambda });
                includeParameterExpression = methodCallExpression;

                isRoot = false;
                head = p;
            }
            outMethodCallExpression = includeParameterExpression;
            return number+1;
        }

        public static bool IncludeEquals<T>(Include<T> include1, Include<T> include2)
        {
            var state = new ChainingPathesState<T>();
            var chain1 = new Chain<T>(state);
            include1.Invoke(chain1);
            var pathes1 = state.Pathes;

            var parser2 = new ChainingPathesState<T>();
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

        //public 
        #region Path based

        public static void Detach<T>(T entity, Include<T> include) where T : class
        {
            var parser = new ChainingPathesState<T>();
            var chain = new Chain<T>(parser);
            include.Invoke(chain);
            var pathes = parser.Pathes;
            DetachPaths(entity, pathes);
        }

        public static void DetachAll<TCol, T>(IEnumerable<T> entities, Include<T> include) where TCol : IEnumerable<T>
        {
            var including = new ChainingPathesState<T>();
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
            var pathesIncluding1 = new ChainingPathesState<T>();
            var includable1 = new Chain<T>(pathesIncluding1);
            include1.Invoke(includable1);
            var pathes1 = pathesIncluding1.Pathes;

            var pathesIncluding2 = new ChainingPathesState<T>();
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

