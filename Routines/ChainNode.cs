using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Vse.Routines
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

    public class ChainNodeTree : Tree<ChainNode, ChainPropertyNode, string>
    {
        public static readonly ChainNodeTree Instance = new ChainNodeTree((n)=>n.Parent);

        private readonly Func<ChainPropertyNode, ChainNode> GetParent;
        private ChainNodeTree(Func<ChainPropertyNode, ChainNode> GetParent) : base(
            n => n.Children.Values, 
            n => n.PropertyName, 
            (n,k)   => { var child = default(ChainPropertyNode); n.Children.TryGetValue(k, out child); return child; }, 
            (n)     => new ChainNode(n.Type), 
            (n,p)   => {
                var child = new ChainPropertyNode(n.Type, n.Expression, n.PropertyInfo, n.PropertyName, n.IsEnumerable, p);
                p.Children.Add(n.PropertyName, child);
                return child;
                }, 
            (n1,n2) => n1.Type==n2.Type)
        {
            this.GetParent = GetParent;
        }

        public ChainNode PathOfNode(ChainPropertyNode node)
        {
            return this.GetPathOfNode(node, GetParent);
        }
    }
}
