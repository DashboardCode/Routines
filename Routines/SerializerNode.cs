using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Vse.Routines
{
    public class SerializerNode
    {
        public readonly Dictionary<string, SerializerPropertyNode> Children = new Dictionary<string, SerializerPropertyNode>();
        public readonly Type Type;
        public readonly Expression expression;

        public SerializerNode(Type type, Expression expression)
        {
            Type = type;
        }

        public void AppendLeafs()
        {
            var containsLeafs = Children.Values.Any(c => c.Children.Count == 0);
            if (!containsLeafs)
            {
                //TODO: compare performance
                //var childProperties = MemberExpressionExtensions.GetSimpleProperties(propertyType, SystemTypesExtensions.SystemTypes);
                var childProperties = MemberExpressionExtensions.GetPrimitiveOrSimpleProperties(Type,
                    SystemTypesExtensions.DefaultSimpleTextTypes,
                    SystemTypesExtensions.DefaultSimpleSymbolTypes);
                foreach (var p in childProperties)
                {
                    ParameterExpression parameterExpression = Expression.Parameter(typeof(object), "o");
                    UnaryExpression unaryExpression
                        = Expression.Convert(
                            Expression.Property(
                                Expression.Convert(parameterExpression, Type),
                                p), typeof(object));
                    Children.Add(p.Name, new SerializerPropertyNode(p.PropertyType, null, this, p.Name));
                }
            }

            foreach (var node in Children.Values)
            {
                node.AppendLeafs();
            }
        }
    }

    public class SerializerPropertyNode : SerializerNode
    {
        public readonly string PropertyName;
        public readonly SerializerNode Parent;
        public readonly Expression Expression;

        public SerializerPropertyNode(Type type, Expression expression, SerializerNode parent, string name)
            : base(type, expression)
        {
            Parent = parent;
            PropertyName = name;
            Expression = expression;
        }

        public override string ToString()
        {
            string parents = "";
            SerializerNode p = this;
            while (p != null)
            {
                if (p is SerializerPropertyNode)
                {
                    var s = (SerializerPropertyNode)p;
                    parents = s.PropertyName + "\\" + parents;
                    p = s.Parent;
                }
                else
                {
                    p = null;
                }
            };
            return parents + " (" + Type.Name + ")";
        }
    }

    public class SerializerEnumerablePropertyNode : SerializerPropertyNode
    {
        public readonly Type EnumerableType;

        public SerializerEnumerablePropertyNode(Type type, Expression expression, SerializerNode parent, string name, Type underlyingEnumerable)
            : base(type, expression, parent, name)
        {
            EnumerableType = underlyingEnumerable;
        }
    }
}
