using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;

namespace DashboardCode.Routines
{
    public static class IncludeExtensions
    {
        public static ChainNode GetChainNode<T>(this Include<T> include)
        {
            var visitor = new ChainVisitor<T>();
            var chain = new Chain<T>(visitor);
            include.Invoke(chain);
            var rootNode = visitor.Root;
            return rootNode;
        }

        private static LambdaExpression CreatePropertyLambda(Type declaringType, PropertyInfo propertyInfo)
        {
            ParameterExpression eParameterExpression = Expression.Parameter(declaringType, "e");
            var propertyCallExpression = Expression.Property(
                eParameterExpression,
                propertyInfo
                );
            var propertyLambda = Expression.Lambda(propertyCallExpression, new[] { eParameterExpression });
            return propertyLambda;
        }

        public static Include<T> AppendLeafs<T>(this Include<T> source) where T : class
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
                        node.Children.Add(propertyInfo.Name, new ChainPropertyNode(propertyInfo.PropertyType, expression, propertyInfo, propertyInfo.Name, false, node));
                    }
                }

                foreach (var n in node.Children.Values)
                    AppendLeafs(n);
            }
            AppendLeafs(root);
            var destination = root.ComposeInclude<T>();
            return destination;
        }

        public static IReadOnlyCollection<Type> ListLeafTypes<T>(this Include<T> include)
        {
            var node = include.GetChainNode();
            var @types = node.ListLeafTypes();
            return @types;
        }

        public static IReadOnlyCollection<string[]> ListLeafKeyPaths<T>(this Include<T> include)
        {
            var rootNode = include.GetChainNode();
            var @paths = ChainNodeTree.ListLeafKeyPaths(rootNode);
            return @paths;
        }

        public static IReadOnlyCollection<ChainNode> ListLeafPaths<T>(this Include<T> include, Type type)
        {
            var rootNode = include.GetChainNode();
            var @paths = ChainNodeTree.ListLeafPaths(rootNode);
            return @paths;
        }

        public static IReadOnlyCollection<string> ListLeafXPaths<T>(this Include<T> include)
        {
            if (include == null)
                return new List<string> { "/" };
            var rootNode = include.GetChainNode();
            var @paths = ChainNodeTree.ListLeafXPaths(rootNode);
            return @paths;
        }

        public static Include<T> Clone<T>(Include<T> include) where T : class
        {
            var rootNode = include.GetChainNode();
            var @cloned = rootNode.ComposeInclude<T>();
            return @cloned;
        }

        public static bool IsEqualTo<T>(this Include<T> include1, Include<T> include2)
        {
            var rootNode1 = include1.GetChainNode();
            var rootNode2 = include2.GetChainNode();

            bool @value = ChainNodeTree.IsEqualTo(rootNode1, rootNode2);
            return @value;
        }

        public static bool IsSupersetOf<T>(this Include<T> include1, Include<T> include2)
        {
            var rootNode1 = include1.GetChainNode();
            var rootNode2 = include2.GetChainNode();

            bool @value = ChainNodeTree.IsSupersetOf(rootNode1, rootNode2);
            return @value;
        }

        public static bool IsSubsetOf<T>(this Include<T> include1, Include<T> include2)
        {
            var rootNode1 = include1.GetChainNode();
            var rootNode2 = include2.GetChainNode();

            bool @value = ChainNodeTree.IsSubsetOf(rootNode1, rootNode2);
            return @value;
        }

        public static Include<T> Merge<T>(this Include<T> include1, Include<T> include2)
        {
            var rootNode1 = include1.GetChainNode();
            var rootNode2 = include2.GetChainNode();

            var union = ChainNodeTree.Merge(rootNode1, rootNode2);
            var @value = union.ComposeInclude<T>();
            return @value;
        }
    }
}

