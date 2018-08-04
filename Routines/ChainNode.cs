using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace DashboardCode.Routines
{
    public class ChainNode
    {
        public readonly Dictionary<string, ChainPropertyNode> Children = new Dictionary<string, ChainPropertyNode>();
        public readonly Type Type;
        public ChainNode(Type type) => Type = type;
    }

    public class ChainPropertyNode : ChainNode
    {
        public readonly LambdaExpression Expression;
        public readonly string MemberName;
        public readonly bool IsEnumerable;
        public readonly ChainNode Parent;

        public ChainPropertyNode(Type type, LambdaExpression expression, string memberName, bool isEnumerable, ChainNode parent)
            : base(type)
        {
            Expression = expression;
            MemberName = memberName;
            IsEnumerable = isEnumerable;
            Parent = parent;
        }
    }

    public static class ChainNodeTree
    {
        private static readonly LinkedTree<ChainNode, ChainPropertyNode, string> meta = new LinkedTree<ChainNode, ChainPropertyNode, string>(
            n => n.Children.Values, 
            n => n.MemberName, 
            (n,k)   => n.Children.GetValueOrDefault(k),
            (n)     => new ChainNode(n.Type), 
            (n,p)   => n.CloneChainPropertyNode(p),
            (n)     => n.Parent,
            (n1,n2) => n1.Type==n2.Type
        );

        public static string FindLinkedRootXPath(ChainPropertyNode node) =>
             TreeExtensions.FindLinkedRootXPath(meta, node);

        public static ChainNode FindLinkedRootPath(ChainPropertyNode node) =>
             TreeExtensions.FindLinkedRootPath(meta, node);

        public static bool IsEqualTo(ChainNode node1, ChainNode node2) =>
            TreeExtensions.IsEqualTo(meta, node1, node2);

        public static bool IsSupersetOf(ChainNode node1, ChainNode node2) =>
             TreeExtensions.IsSupersetOf(meta, node1, node2);

        public static bool IsSubsetOf(ChainNode node1, ChainNode node2) =>
            TreeExtensions.IsSubsetOf(meta, node1, node2);

        public static ChainNode Merge(ChainNode node1, ChainNode node2) =>
            TreeExtensions.Merge(meta, node1, node2);

        public static IReadOnlyCollection<string[]> ListLeafKeyPaths(ChainNode node) =>
             TreeExtensions.ListLeafKeyPaths(meta, node);

        public static IReadOnlyCollection<ChainNode> ListLeafPaths(ChainNode node) =>
              TreeExtensions.ListLeafPaths(meta, node);

        public static IReadOnlyCollection<string> ListLeafXPaths(ChainNode node) =>
              TreeExtensions.ListLeafXPaths(meta, node);

        public static IReadOnlyCollection<string> ListXPaths(ChainNode node) =>
            TreeExtensions.ListXPaths(meta, node);
    }
}