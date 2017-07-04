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

        //public ChainNode Clone()
        //{
        //    var @value = new ChainNode(Type);
        //    foreach(var c in Children)
        //    {
        //        @value.Children.Add(c.Key, c.Value.Clone(@value));
        //    }
        //    return @value;
        //}
    }

    public class ChainPropertyNode : ChainNode
    {
        public readonly LambdaExpression Expression;
        public readonly PropertyInfo PropertyInfo;
        public readonly string PropertyName;
        public readonly bool IsEnumerable;

        //public readonly ChainNode Parent;


        public ChainPropertyNode(Type type, LambdaExpression expression, PropertyInfo propertyInfo, /*ChainNode parent, */string propertyName, bool isEnumerable)
            : base(type)
        {
            //Parent = parent;
            PropertyName = propertyName;
            Expression = expression;
            PropertyInfo = propertyInfo;
            IsEnumerable = isEnumerable;
        }

        //public override string ToString()
        //{
        //    string @value = "";
        //    ChainNode chainNode = this;
        //    do
        //    {
        //        if (chainNode is ChainPropertyNode)
        //        {
        //            var propertyNode = (ChainPropertyNode)chainNode;
        //            @value = $"{propertyNode.PropertyName}\\{@value}";
        //            chainNode = propertyNode.Parent;
        //        }
        //        else
        //        {
        //            chainNode = null;
        //        }
        //    } while (chainNode != null);
        //    return @value;
        //}

        //internal ChainPropertyNode Clone(ChainNode parent)
        //{
        //    ChainPropertyNode @value = new ChainPropertyNode(Type, Expression, PropertyInfo, parent, PropertyName, IsEnumerable);
            
        //    foreach (var c in Children)
        //    {
        //        @value.Children.Add(c.Key, c.Value.Clone(@value));
        //    }
        //    return @value;
        //}

        //public ChainNode CreateAncestorsAndSelfChain()
        //{
        //    var ancestorsAndSelf = new List<ChainPropertyNode>();
        //    ancestorsAndSelf.Add(this);
        //    ChainNode root = this.Parent;
        //    while(root is ChainPropertyNode)
        //    {
        //        ancestorsAndSelf.Add(this);
        //        root = ((ChainPropertyNode)root).Parent;
        //    }
        //    ancestorsAndSelf.Reverse();
        //    var @value = new ChainNode(Type);
        //    var p = @value;
        //    foreach(var a in ancestorsAndSelf)
        //    {
        //        var c = new ChainPropertyNode(a.Type, a.Expression, a.PropertyInfo, p, a.PropertyName, a.IsEnumerable);
        //        p.Children.Add(a.PropertyName, c);
        //        p = c;
        //    }
        //    return @value;
        //}
    }
}
