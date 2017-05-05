using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Vse.Routines
{
    public class SerializerNavigationExpressionParser<TRootEntity> : INavigationExpressionParser<TRootEntity>
    {
        public readonly SerializerNode Root = new SerializerNode(typeof(TRootEntity));

        private SerializerNode CurrentNode;

        private static SerializerNode AddIfAbsent(
            SerializerNode parent,
            PropertyInfo propertyInfo,
            Type navigationType,
            bool isEnumerable,
            Func<Func<object, object>> getGeneralized)
        {
            var dictionary = parent.Children;
            var name = propertyInfo.Name;
            if (!dictionary.TryGetValue(name, out SerializerNode node))
            {
                var generalized = getGeneralized();
                node = new SerializerNode(parent, /*propertyInfo.PropertyType*/ navigationType, isEnumerable, generalized, name);
                dictionary.Add(name, node);
            }
            return node;
        }

        public void ParseRoot<TEntity>(Expression<Func<TRootEntity, TEntity>> navigationExpression)
        {
            var propertyInfo = ((MemberExpression)navigationExpression.Body).Member as PropertyInfo;
            var node = AddIfAbsent(Root, propertyInfo, typeof(TEntity), false, () =>
            {
                //TODO: it is interesing how to convert Func<TRootEntity, TEntity> to Func<object, object> before compile
                //Also it is interesting if my prognose about performance true: current straight-forward casting is the best.

                //var param = Expression.Parameter(typeof(object));
                //var casted = Expression.Convert(navigationExpression, typeof(object));
                //Expression<Func<object, object>> getterExpression =
                //    Expression.Lambda<Func<object, object>>(
                //        navigationExpression.Body
                //         casted
                //         , Expression.Parameter(typeof(object)));
                //Func<object, object> generalized = getterExpression.Compile();

                Func<TRootEntity, TEntity> func = navigationExpression.Compile();
                Func<object, object> generalized = (o) => { return func((TRootEntity)o); };
                return generalized;
            });
            CurrentNode = node;
        }
        public void ParseRootEnumerable<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> navigationExpression)
        {
            var propertyInfo = ((MemberExpression)navigationExpression.Body).Member as PropertyInfo;

            var node = AddIfAbsent(Root, propertyInfo, typeof(TEntity), true, () =>
            {
                Func<TRootEntity, IEnumerable<TEntity>> func = navigationExpression.Compile();
                Func<object, object> generalized = (o) => { return func((TRootEntity)o); };
                return generalized;
            });
            CurrentNode = node;
        }
        public void Parse<TThenEntity, TEntity>(Expression<Func<TThenEntity, TEntity>> navigationExpression)
        {
            var propertyInfo = ((MemberExpression)navigationExpression.Body).Member as PropertyInfo;

            var node = AddIfAbsent(CurrentNode, propertyInfo, typeof(TEntity), false, () =>
            {
                Func<TThenEntity, TEntity> func = navigationExpression.Compile();
                Func<object, object> generalized = (o) => { return func((TThenEntity)o); };
                return generalized;
            });
            CurrentNode = node;
        }
        public void ParseEnumerable<TThenEntity, TEntity>(Expression<Func<TThenEntity, IEnumerable<TEntity>>> navigationExpression)
        {
            var propertyInfo = ((MemberExpression)navigationExpression.Body).Member as PropertyInfo;

            var node = AddIfAbsent(CurrentNode, propertyInfo, typeof(TEntity), true, () =>
            {
                Func<TThenEntity, IEnumerable<TEntity>> func = navigationExpression.Compile();
                Func<object, object> generalized = (o) => { return func((TThenEntity)o); };
                return generalized;
            });
            CurrentNode = node;
        }
    }

    public class SerializerNode
    {
        public readonly string PropertyName;
        public readonly Func<object, object> func;
        //public readonly PropertyInfo PropertyInfo;
        public readonly bool IsEnumerable;
        public readonly bool IsLeaf;
        public readonly bool IsPrimitive;
        public readonly bool IsSimpleText;
        public readonly bool IsSimpleNumber;
        public readonly bool IsString;
        public readonly bool IsBoolean;
        public readonly bool IsDateTime;
        public readonly Type Type;
        public readonly SerializerNode Parent;
        public readonly Dictionary<string, SerializerNode> Children = new Dictionary<string, SerializerNode>();
        public SerializerNode(SerializerNode parent, Type type, bool isEnumerable, Func < object, object> func, string name)
        {
            Parent = parent;
            Type = type;
            this.func = func;
            PropertyName = name;
            IsEnumerable = isEnumerable;
            
            var typeInfo = type.GetTypeInfo();
            if (type == typeof(string))
                IsString = true;
            else if (type == typeof(bool) || type == typeof(bool?))
                IsBoolean = true;
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
                IsDateTime = true;
            else if (type == typeof(char) || type == typeof(char?) || SystemTypesExtensions.DefaultSimpleTextTypes.Contains(type))
                IsSimpleText = true;
            else if (SystemTypesExtensions.DefaultSimpleNumberTypes.Contains(type))
                IsSimpleNumber = true;
            else if (typeInfo.IsPrimitive)
                IsPrimitive = true;
            else
            {
                var baseNullableType = Nullable.GetUnderlyingType(type);
                if (baseNullableType != null && baseNullableType.GetTypeInfo().IsPrimitive)
                    IsPrimitive = true;
            }
            IsLeaf = IsPrimitive || IsSimpleText || IsString || IsBoolean || IsDateTime || IsSimpleNumber;
        }

        public SerializerNode(Type type)
        {
            Type = type;
        }

        public void Append()
        {
            //if (!IsEnumerable)
            {
                var containsLeafs = Children.Values.Any(c => c.IsLeaf);
                if (!containsLeafs)
                {
                    //TODO: compare performance
                    //var childProperties = MemberExpressionExtensions.GetSimpleProperties(propertyType, SystemTypesExtensions.SystemTypes);
                    var childProperties = MemberExpressionExtensions.GetPrimitiveOrSimpleProperties(Type, 
                        SystemTypesExtensions.DefaultSimpleTextTypes,
                        SystemTypesExtensions.DefaultSimpleNumberTypes);
                    foreach (var p in childProperties)
                    {
                        ParameterExpression parameterExpression = Expression.Parameter(typeof(object), "o");
                        UnaryExpression unaryExpression
                            = Expression.Convert(
                                Expression.Property(
                                    Expression.Convert(parameterExpression, Type),
                                    p), typeof(object));
                        Func<object, object> func = Expression.Lambda<Func<object, object>>(unaryExpression, new[] { parameterExpression }).Compile();
                        Children.Add(p.Name, new SerializerNode(this, p.PropertyType, false, func, p.Name));
                    }
                }
            }
            foreach (var node in Children.Values)
            {
                node.Append();
            }
        }

        public override string ToString()
        {
            string parents = "";
            var p = this;
            while (p!=null)
            {
                parents = p.PropertyName + "\\"+ parents;
                p = p.Parent;
            };
            return parents + " (" + Type.Name + ")";
        }
    }
}
