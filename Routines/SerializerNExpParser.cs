using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Vse.Routines
{
    public class SerializerNExpParser<TRootEntity> : INExpParser<TRootEntity>
    {
        static SerializerNExpParser(){
            var isEnumerable = typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(typeof(TRootEntity).GetTypeInfo());
            if (typeof(TRootEntity) != typeof(string) && typeof(TRootEntity) != typeof(byte[]) && isEnumerable)
                throw new NotSupportedException("Navigation expression root type can't be defined as collection (except two types byte[] and string)");
        }

        public readonly SerializerBaseNode Root = new SerializerBaseNode(typeof(TRootEntity));

        private SerializerNode CurrentNode;

        private static SerializerNode AddIfAbsent(
            SerializerBaseNode parent,
            PropertyInfo propertyInfo,
            Type navigationType,
            Type navigationEnumerableType,
            bool isEnumerable,
            Func<Func<object, object>> getGeneralized)
        {
            var dictionary = parent.Children;
            var name = propertyInfo.Name;
            if (!dictionary.TryGetValue(name, out SerializerNode node))
            {
                var generalized = getGeneralized();
                node = new SerializerNode(parent, navigationType, navigationEnumerableType, isEnumerable, generalized, name);
                dictionary.Add(name, node);
            }
            return node;
        }

        public void ParseRoot<TEntity>(Expression<Func<TRootEntity, TEntity>> navigationExpression)
        {
            var propertyInfo = ((MemberExpression)navigationExpression.Body).Member as PropertyInfo;
            var node = AddIfAbsent(Root, propertyInfo,  typeof(TEntity), null, false, () =>
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
            var node = AddIfAbsent(Root, propertyInfo, typeof(TEntity), typeof(IEnumerable<TEntity>), true, () =>
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
            var node = AddIfAbsent(CurrentNode, propertyInfo, typeof(TEntity), null, false, () =>
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
            var node = AddIfAbsent(CurrentNode, propertyInfo, typeof(TEntity), typeof(IEnumerable<TEntity>), true, () =>
            {
                Func<TThenEntity, IEnumerable<TEntity>> func = navigationExpression.Compile();
                Func<object, object> generalized = (o) => { return func((TThenEntity)o); };
                return generalized;
            });
            CurrentNode = node;
        }
    }

    public enum SerializerPropertyPipeline { Struct = 0, Object = 1, NullableStruct = 2 }; 

    public class SerializerBaseNode
    {
        public readonly Dictionary<string, SerializerNode> Children = new Dictionary<string, SerializerNode>();
        public readonly SerializerPropertyPipeline SerializerPropertyPipeline;
        public readonly Type Type;
        public readonly Type TypeUnderlyingNullable;
        public readonly bool IsLeaf;
        public readonly bool IsPrimitive;
        public readonly bool IsNPrimitive;
        public readonly bool IsDecimal;
        public readonly bool IsNDecimal;
        public readonly bool IsSimpleText;
        public readonly bool IsSimpleSymbol;
        public readonly bool IsString;
        public readonly bool IsBoolean;
        public readonly bool IsDateTime;
        public readonly bool IsByteArray;
        public readonly bool IsNDateTime;
        public readonly bool IsNBoolean;

        public readonly bool IsNullable;

        public bool IsRoot { get; protected set; } = true;
        public Action<object> Serialize;
        
        public SerializerBaseNode(Type type)
        {
            Type = type;
            TypeUnderlyingNullable = Nullable.GetUnderlyingType(type);
            if (TypeUnderlyingNullable!=null)
                IsNullable = true;
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsClass)
            {
                SerializerPropertyPipeline = SerializerPropertyPipeline.Object;
            }
            else
            {
                if (IsNullable)
                    SerializerPropertyPipeline = SerializerPropertyPipeline.NullableStruct;
                else
                    SerializerPropertyPipeline = SerializerPropertyPipeline.Struct;
            }
            
            if (type == typeof(string))
                IsString = true;
            else if (type == typeof(bool))
                IsBoolean = true;
            else if (type == typeof(bool?))
                IsNBoolean = true;
            else if (type == typeof(DateTime))
                IsDateTime = true;
            else if (type == typeof(DateTime?))
                IsNDateTime = true;
            else if (type == typeof(decimal))
                IsDecimal = true;
            else if (type == typeof(decimal?))
                IsNDecimal = true;
            else if (type == typeof(byte[]) /*typeof(IEnumerable<byte>).GetTypeInfo().IsAssignableFrom(typeInfo)*/)
                IsByteArray = true;
            else if (type == typeof(char) || type == typeof(char?) || SystemTypesExtensions.DefaultSimpleTextTypes.Contains(type))
            {
                IsSimpleText = true;
            }
            else if (SystemTypesExtensions.DefaultSimpleSymbolTypes.Contains(type))
                IsSimpleSymbol = true;
            else if (typeInfo.IsPrimitive)
                IsPrimitive = true;
            else if (IsNullable && TypeUnderlyingNullable.GetTypeInfo().IsPrimitive)
                IsNPrimitive = true;
            IsLeaf = IsNPrimitive || IsDecimal || IsNDecimal|| IsPrimitive || IsNBoolean || IsNDateTime || IsSimpleText || IsString || IsBoolean || IsDateTime || IsSimpleSymbol || IsByteArray;
        }

        public void AppendLeafs()
        {
            var containsLeafs = Children.Values.Any(c => c.IsLeaf);
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
                    Func<object, object> func = Expression.Lambda<Func<object, object>>(unaryExpression, new[] { parameterExpression }).Compile();
                    Children.Add(p.Name, new SerializerNode(
                        this, 
                        p.PropertyType, 
                        null,
                        false, 
                        func, 
                        p.Name));
                }
            }

            foreach (var node in Children.Values)
            {
                node.AppendLeafs();
            }
        }
    }

    public class SerializerNode: SerializerBaseNode
    {
        public readonly string PropertyName;
        public readonly Func<object, object> func;
        public readonly bool IsEnumerable;
        public readonly Type TypeUnderlyingEnumerable;
        public readonly SerializerBaseNode Parent;

        public SerializerNode(SerializerBaseNode parent, Type type, Type underlyingEnumerable,  /*Action serialize,*/ bool isEnumerable, Func <object, object> func, string name)
            :base(type/*, serialize*/)
        {
            IsRoot = false;
            Parent = parent;
            this.func = func;
            PropertyName = name;
            IsEnumerable = isEnumerable;
            if (isEnumerable)
            {
                TypeUnderlyingEnumerable = underlyingEnumerable;
            }
        }

        public override string ToString()
        {
            string parents = "";
            SerializerBaseNode p = this;
            while (p!=null)
            {
                if (p is SerializerNode)
                {
                    var s = (SerializerNode)p;
                    parents = s.PropertyName + "\\" + parents;
                    p = s.Parent;
                }
            };
            return parents + " (" + Type.Name + ")";
        }
    }
}
