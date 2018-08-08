using System;
using System.Collections.Generic;
using System.Reflection;

namespace DashboardCode.Routines
{
    public static class IncludeExtensions
    {
        public static ChainNode CreateChainNode<T>(this Include<T> include)
        {
            if (include == null)
                return new ChainNode(typeof(T));
            var visitor = new ChainVisitor<T>();
            var chain = new Chain<T>(visitor);
            include.Invoke(chain);
            var rootNode = visitor.Root;
            return rootNode;
        }

        public static Include<T> AppendLeafs<T>(this Include<T> source, Func<ChainNode, IEnumerable<MemberInfo>> leafRule = null)
        {
            var parser = new ChainVisitor<T>();
            var train = new Chain<T>(parser);
            source.Invoke(train);
            var root = parser.Root;
            root.AppendLeafs(leafRule);
            var destination = root.ComposeInclude<T>();
            return destination;
        }

        #region Lists
        public static IReadOnlyCollection<Type> ListLeafTypes<T>(this Include<T> include)
        {
            var node = include.CreateChainNode();
            var @types = node.ListLeafTypes();
            return @types;
        }

        public static IReadOnlyCollection<string[]> ListLeafKeyPaths<T>(this Include<T> include)
        {
            var rootNode = include.CreateChainNode();
            var @paths = ChainNodeTree.ListLeafKeyPaths(rootNode);
            return @paths;
        }

        public static IReadOnlyCollection<ChainNode> ListLeafPaths<T>(this Include<T> include, Type type)
        {
            var rootNode = include.CreateChainNode();
            var @paths = ChainNodeTree.ListLeafPaths(rootNode);
            return @paths;
        }

        public static IReadOnlyCollection<string> ListLeafXPaths<T>(this Include<T> include)
        {
            if (include == null)
                return new List<string> { "/" };
            var rootNode = include.CreateChainNode();
            var @paths = ChainNodeTree.ListLeafXPaths(rootNode);
            return @paths;
        }

        public static IReadOnlyCollection<string> ListXPaths<T>(this Include<T> include)
        {
            if (include == null)
                return new List<string> { "/" };
            var rootNode = include.CreateChainNode();
            var @paths = ChainNodeTree.ListXPaths(rootNode);
            return @paths;
        }
        #endregion

        public static Include<T> Clone<T>(Include<T> include) where T : class
        {
            var rootNode = include.CreateChainNode();
            var @cloned = rootNode.ComposeInclude<T>();
            return @cloned;
        }

        public static bool IsEqualTo<T>(this Include<T> include1, Include<T> include2)
        {
            var rootNode1 = include1.CreateChainNode();
            var rootNode2 = include2.CreateChainNode();

            bool @value = ChainNodeTree.IsEqualTo(rootNode1, rootNode2);
            return @value;
        }

        public static bool IsSupersetOf<T>(this Include<T> include1, Include<T> include2)
        {
            var rootNode1 = include1.CreateChainNode();
            var rootNode2 = include2.CreateChainNode();

            bool @value = ChainNodeTree.IsSupersetOf(rootNode1, rootNode2);
            return @value;
        }

        public static bool IsSubsetOf<T>(this Include<T> include1, Include<T> include2)
        {
            var rootNode1 = include1.CreateChainNode();
            var rootNode2 = include2.CreateChainNode();

            bool @value = ChainNodeTree.IsSubsetOf(rootNode1, rootNode2);
            return @value;
        }

        public static Include<T> Merge<T>(this Include<T> include1, Include<T> include2)
        {
            var rootNode1 = include1.CreateChainNode();
            var rootNode2 = include2.CreateChainNode();

            var union = ChainNodeTree.Merge(rootNode1, rootNode2);
            var @value = union.ComposeInclude<T>();
            return @value;
        }
    }
}