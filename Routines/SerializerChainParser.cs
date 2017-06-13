using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Vse.Routines
{
    public class SerializerChainParser<TRootEntity> : IChainParser<TRootEntity>
    {
        static SerializerChainParser(){
            var isEnumerable = typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(typeof(TRootEntity).GetTypeInfo());
            if (typeof(TRootEntity) != typeof(string) && typeof(TRootEntity) != typeof(byte[]) && isEnumerable)
                throw new NotSupportedException("Navigation expression root type can't be defined as collection (except two types byte[] and string)");
        }

        public readonly SerializerBaseNode Root = new SerializerBaseNode(typeof(TRootEntity));

        private SerializerPropertyNode CurrentNode;

        private static SerializerPropertyNode AddIfAbsent(
            SerializerBaseNode parent,
            PropertyInfo propertyInfo,
            Type navigationType,
            Type navigationEnumerableType,
            bool isEnumerable,
            Func<Func<object, object>> getGeneralized)
        {
            var dictionary = parent.Children;
            var name = propertyInfo.Name;
            if (!dictionary.TryGetValue(name, out SerializerPropertyNode node))
            {
                var generalized = getGeneralized();
                if (isEnumerable)
                {
                    node = new SerializerEnumerablePropertyNode(parent, navigationType, name, navigationEnumerableType);
                }
                else
                {
                    node = new SerializerPropertyNode(parent, navigationType, name);
                }
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

        //public void ParseRootNullable<TEntity>(Expression<Func<TRootEntity, TEntity?>> getterExpression) where TEntity : struct
        //{
        //    throw new NotImplementedException();
        //}

        //public void ParseNullable<TThenEntity, TEntity>(Expression<Func<TThenEntity, TEntity?>> getterExpression) where TEntity : struct
        //{
        //    throw new NotImplementedException();
        //}
    }

    public enum SerializerPropertyPipeline { Struct = 0, Object = 1, NullableStruct = 2 }; 

    public class SerializerBaseNode
    {
        public readonly Dictionary<string, SerializerPropertyNode> Children = new Dictionary<string, SerializerPropertyNode>();
        public readonly SerializerPropertyPipeline SerializerPropertyPipeline;
        public readonly Type Type;
        public readonly Type TypeUnderlyingNullable;
        public readonly Type CanonicType;
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

        public readonly bool IsNullableStruct;
        public readonly bool IsNullable;

        public bool IsRoot { get; protected set; } = true;
        public Action<object> Serialize;
        
        public SerializerBaseNode(Type type)
        {
            Type = type;
            CanonicType = type;
            TypeUnderlyingNullable = Nullable.GetUnderlyingType(type);
            if (TypeUnderlyingNullable != null)
            {
                IsNullableStruct = true;
                CanonicType = TypeUnderlyingNullable;
            }
            
                
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsClass)
            {
                SerializerPropertyPipeline = SerializerPropertyPipeline.Object;
                IsNullable = true;
            }
            else
            {
                if (IsNullableStruct)
                {
                    SerializerPropertyPipeline = SerializerPropertyPipeline.NullableStruct;
                    IsNullable = true;
                }
                else
                {
                    SerializerPropertyPipeline = SerializerPropertyPipeline.Struct;
                    IsNullable = false;
                }
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
            else if (IsNullableStruct && TypeUnderlyingNullable.GetTypeInfo().IsPrimitive)
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
                    Children.Add(p.Name, new SerializerPropertyNode(this, p.PropertyType, p.Name));
                }
            }

            foreach (var node in Children.Values)
            {
                node.AppendLeafs();
            }
        }
    }

    public class SerializerPropertyNode: SerializerBaseNode
    {
        public readonly string PropertyName;
        //public readonly bool IsEnumerable;
        //public readonly Type TypeUnderlyingEnumerable;
        public readonly SerializerBaseNode Parent;

        public SerializerPropertyNode(SerializerBaseNode parent, Type type, /*Type underlyingEnumerable, bool isEnumerable,*/ string name)
            :base(type)
        {
            IsRoot = false;
            Parent = parent;
            //this.func = func;
            PropertyName = name;
            //IsEnumerable = isEnumerable;
            //if (isEnumerable)
            //{
            //    TypeUnderlyingEnumerable = underlyingEnumerable;
            //}
        }

        public override string ToString()
        {
            string parents = "";
            SerializerBaseNode p = this;
            while (p!=null)
            {
                if (p is SerializerPropertyNode)
                {
                    var s = (SerializerPropertyNode)p;
                    parents = s.PropertyName + "\\" + parents;
                    p = s.Parent;
                }
            };
            return parents + " (" + Type.Name + ")";
        }
    }

    public class SerializerEnumerablePropertyNode : SerializerPropertyNode
    {
        public readonly Type EnumerableType;

        public SerializerEnumerablePropertyNode(SerializerBaseNode parent, Type type, string name, Type underlyingEnumerable)
            : base(parent, type, name)
        {
            EnumerableType = underlyingEnumerable;
        }
    }
}
