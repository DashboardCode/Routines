using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DashboardCode.Routines
{
    public class ChainNode
    {
        public readonly Dictionary<string, ChainMemberNode> Children = new Dictionary<string, ChainMemberNode>();
        public readonly Type Type;
        public ChainNode(Type type) => Type = type;
    }

    public class ChainMemberNode : ChainNode
    {
        public readonly LambdaExpression Expression;
        public readonly string MemberName;
        public readonly bool IsEnumerable;
        public readonly ChainNode Parent;

        public ChainMemberNode(Type type, LambdaExpression expression, string memberName, bool isEnumerable, ChainNode parent)
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
        private static readonly LinkedTree<ChainNode, ChainMemberNode, string> meta = new LinkedTree<ChainNode, ChainMemberNode, string>(
            n => n.Children.Values, 
            n => n.MemberName, 
            (n,k)   => n.Children.GetValueOrDefault(k),
            (n)     => new ChainNode(n.Type), 
            (n,p)   => n.CloneChainMemberNode(p),
            (n)     => n.Parent,
            (n1,n2) => n1.Type==n2.Type
        );

        public static bool FindNode(ChainNode root, string[] path, out ChainMemberNode node) =>
             TreeExtensions.FindNode(meta, root, path, out node);

        public static string FindLinkedRootXPath(ChainMemberNode node) =>
             TreeExtensions.FindLinkedRootXPath(meta, node);

        public static string[] FindLinkedRootKeyPath(ChainNode node) =>
             TreeExtensions.FindLinkedRootKeyPath(meta, node);

        public static ChainNode FindLinkedRootPath(ChainMemberNode node) =>
             TreeExtensions.FindLinkedRootPath(meta, node);

        public static bool IsEqualTo(ChainNode node1, ChainNode node2) =>
            TreeExtensions.IsEqualTo(meta, node1, node2);

        public static bool IsSuperTreeOf(ChainNode node1, ChainNode node2) =>
             TreeExtensions.IsSuperTreeOf(meta, node1, node2);

        public static bool IsSubTreeOf(ChainNode node1, ChainNode node2) =>
            TreeExtensions.IsSubTreeOf(meta, node1, node2);

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