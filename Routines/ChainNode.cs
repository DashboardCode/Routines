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

        public ChainNode(Type type)
        {
            Type = type;
        }

    }

    public class ChainPropertyNode : ChainNode
    {
        public readonly LambdaExpression Expression;
        public readonly PropertyInfo PropertyInfo;
        public readonly string PropertyName;
        public readonly bool IsEnumerable;
        public readonly ChainNode Parent;

        public ChainPropertyNode(Type type, LambdaExpression expression, PropertyInfo propertyInfo, string propertyName, bool isEnumerable, ChainNode parent)
            : base(type)
        {
            Expression = expression;
            PropertyInfo = propertyInfo;
            PropertyName = propertyName;
            IsEnumerable = isEnumerable;
            Parent = parent;
        }
    }

    public static class ChainNodeTree
    {
        public static readonly LinkedTree<ChainNode, ChainPropertyNode, string> chainNodeTreeMeta = new LinkedTree<ChainNode, ChainPropertyNode, string>(
            n => n.Children.Values, 
            n => n.PropertyName, 
            (n,k)   => { var child = default(ChainPropertyNode); n.Children.TryGetValue(k, out child); return child; }, 
            (n)     => new ChainNode(n.Type), 
            (n,p)   => {
                var child = new ChainPropertyNode(n.Type, n.Expression, n.PropertyInfo, n.PropertyName, n.IsEnumerable, p);
                p.Children.Add(n.PropertyName, child);
                return child;
                },
            (n)     => n.Parent,
            (n1,n2) => n1.Type==n2.Type
        );

        public static List<string[]> ListLeafKeyPaths (ChainNode node) =>
             TreeExtensions.ListLeafKeyPaths(chainNodeTreeMeta, node);

        public static string FindLinkedRootXPath(ChainPropertyNode node) =>
             TreeExtensions.FindLinkedRootXPath(chainNodeTreeMeta, node);

        public static ChainNode FindLinkedRootPath(ChainPropertyNode node) =>
             TreeExtensions.FindLinkedRootPath(chainNodeTreeMeta, node);
    }
}
