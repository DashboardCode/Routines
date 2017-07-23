using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DashboardCode.Routines.Json
{
    public class SerializerOptions
    {
        public SerializerOptions(
             Delegate serializer,
             Func<StringBuilder, bool> nullSerializer,
             bool handleNullProperty,
             InternalNodeOptions internalNodeOptions)
        {
            this.Serializer = serializer;
            this.NullSerializer = nullSerializer;
            this.HandleNullProperty = handleNullProperty;
            this.InternalNodeOptions = internalNodeOptions;
        }
        public readonly Delegate Serializer;
        public readonly Func<StringBuilder, bool> NullSerializer;
        public readonly bool HandleNullProperty;
        public readonly InternalNodeOptions InternalNodeOptions;
    }

    public class InternalNodeOptions
    {
        public bool HandleEmptyObjectLiteral { get; set; } = true;
        public bool HandleEmptyArrayLiteral { get; set; }  = true;

        public Func<StringBuilder, bool> NullSerializer { get; set; }
        public bool HandleNullProperty { get; set; } = true;
        public Func<StringBuilder, bool> NullArraySerializer { get; set; }
        public bool HandleNullArrayProperty { get; set; } = true;
    }

    internal class SerializersPair
    {
        public ConstantExpression SerializerExpression { get; set; }
        public Func<StringBuilder, bool> NullSerializer { get; set; }
        public bool HandleNullProperty { get; set; }

        public SerializersPair(ConstantExpression serializerExpression, Func<StringBuilder, bool> nullSerializer, bool handleNullProperty)
        {
            this.SerializerExpression = serializerExpression;
            this.NullSerializer = nullSerializer;
            this.HandleNullProperty = handleNullProperty;
        }
    }

    public static class JsonChainTools
    {
        //public static SerializerOptions ComposeLeafSerializerSet<TEntity>(
        //    ChainNode node, 
        //    RulesDictionary<TEntity> rulesDictionary)
        //{
        //    var serializationType = Nullable.GetUnderlyingType(node.Type) ?? node.Type;
        //    var serializerOptions = rulesDictionary.GetRule("/", serializationType);

        //    if (serializerOptions == null)
        //        throw new NotConfiguredException($"Node '{node.GetXPathOfNode()}' included as leaf but formatter for its type '{serializationType.FullName}' is not configured");
        //    return serializerOptions;
        //}

        internal static bool IsNullable(Type type)
        {
            var @value = !type.GetTypeInfo().IsValueType || Nullable.GetUnderlyingType(type) != null;
            return @value;
        }

        internal static bool IsNullable(Type type, bool handleNullProperty)
        {
            var @value = !type.GetTypeInfo().IsValueType || Nullable.GetUnderlyingType(type) != null;
            if (@value)
                @value = handleNullProperty;
            return @value;
        }

        internal static SerializersPair CreateSerializsPair(
            ChainNode node, 
            Type parentType, 
            Func<ChainNode, SerializerOptions> getLeafSerializerSet, 
            Func<ChainNode, bool, InternalNodeOptions> getInternalSerializerSet)
        {
            if (node.Children.Count == 0) // leaf node
            {
                var serializerOptions = getLeafSerializerSet(node);
                var serializationType = Nullable.GetUnderlyingType(node.Type) ?? node.Type;
                var formatterDelegateType = typeof(Func<,,>).MakeGenericType(typeof(StringBuilder), serializationType, typeof(bool));
                var serializerMethodInfo = serializerOptions.Serializer;
                var genericResolvedDelegate = serializerMethodInfo; // serializerMethodInfo.CreateDelegate(formatterDelegateType, null);
                var serializerExpression = Expression.Constant(genericResolvedDelegate, genericResolvedDelegate.GetType());
                return new SerializersPair(serializerExpression, serializerOptions.NullSerializer, serializerOptions.HandleNullProperty);
            }
            else // internal node
            {
                var internalSerializerSet = getInternalSerializerSet(node, false);
                var properies = new List<Expression>();
                foreach (var c in node.Children)
                {
                    var n = c.Value;
                    ConfigureSerializeProperty(n, node.Type, properies, getLeafSerializerSet, getInternalSerializerSet);
                }
                var objectFormatterLambda = CreateSerializeObjectLambda(node.Type, internalSerializerSet.HandleEmptyObjectLiteral, properies.ToArray());
                var @delegate = objectFormatterLambda.Compile();
                
                var delegateConstant = Expression.Constant(@delegate, @delegate.GetType());
                return new SerializersPair(delegateConstant, internalSerializerSet.NullSerializer, internalSerializerSet.HandleNullProperty);
            }
        }

        public static void ConfigureSerializeProperty(ChainPropertyNode node, Type parentType, List<Expression> propertyExpressions
            , Func<ChainNode, SerializerOptions> getLeafSerializerSet
            , Func<ChainNode, bool, InternalNodeOptions> getInternalSerializerSet)
        {

            bool? isNullableStruct = IsNullableStruct(node.Type);
            var propertyName = node.PropertyName;
            var getterLambdaExpression = CreateGetterLambdaExpression(parentType, node.Type, propertyName);
            var getterDelegate = getterLambdaExpression.Compile();
            var getterConstantExpression = Expression.Constant(getterDelegate, getterDelegate.GetType());
            var serializationType = Nullable.GetUnderlyingType(node.Type) ?? node.Type;

            Type propertyType;
            //SerializersPair expressions;
            //bool isEnumerable = node is ChainEnumerablePropertyNode;

            ConstantExpression formatterExpression = null;
            ConstantExpression nullFormatterExpression = null;

            if (node.IsEnumerable)
            {
                propertyType = typeof(IEnumerable<>).MakeGenericType(node.Type);//  ((ChainEnumerablePropertyNode)node).EnumerableType;
                // check that property should be serailizable: SerializeRefProperty
                var serializerInternalSet = getInternalSerializerSet(node, true);

                var itemSerializers = CreateSerializsPair(node, parentType, getLeafSerializerSet, getInternalSerializerSet);

                //var itemSerializerExpression = itemSerializers.Item1;
                //var itemNullSerializerExpression = itemSerializers.Item2;
                var sbParameterExpression = Expression.Parameter(typeof(StringBuilder), "sb");
                var tParameterExpression = Expression.Parameter(propertyType, "t");

                ConstantExpression nullItemSerializerExpression = null;
                if (IsNullable(node.Type))
                {
                    if (itemSerializers.NullSerializer == null)
                        throw new NotSupportedException($"Null serializer is not setuped for leaf node '{node.GetXPathOfNode()}' ");
                    nullItemSerializerExpression = CreateSerializeNullConstant(itemSerializers.NullSerializer);
                }

                MethodCallExpression methodCallExpression = CreateSerializeArrayMethodCall(
                    serializationType,
                    isNullableStruct,
                    itemSerializers.SerializerExpression,
                    nullItemSerializerExpression, //itemSerializers.NullSerializer,
                    sbParameterExpression,
                    tParameterExpression,
                    serializerInternalSet.HandleEmptyArrayLiteral);

                var serializeArrayLambda = Expression.Lambda(methodCallExpression, new[] { sbParameterExpression, tParameterExpression });
                var serializeArrayDelegate = serializeArrayLambda.Compile();

                formatterExpression = Expression.Constant(serializeArrayDelegate, serializeArrayDelegate.GetType());
                if (serializerInternalSet.HandleNullArrayProperty)
                    nullFormatterExpression = CreateSerializeNullConstant(serializerInternalSet.NullArraySerializer);
            }
            else
            {
                propertyType = serializationType;
                var serializersPair = CreateSerializsPair(node, parentType, getLeafSerializerSet, getInternalSerializerSet);
                formatterExpression = serializersPair.SerializerExpression;

                if (IsNullable(node.Type, serializersPair.HandleNullProperty))
                {
                    if (serializersPair.NullSerializer == null)
                        throw new NotSupportedException($"Null serializer is not setuped for internal node '{node.GetXPathOfNode()}' ");
                    nullFormatterExpression = CreateSerializeNullConstant(serializersPair.NullSerializer);
                }
                //nullFormatterExpression = expressions.NullSerializer;
            }

            MethodInfo propertySerializerMethodInfo = (node.IsEnumerable) ?
                GetEnumerablePropertySerializerMethodInfo(nullFormatterExpression != null) :
                GetPropertySerializerMethodInfo(isNullableStruct, nullFormatterExpression != null);

            var serializePropertyExpression = CreateSerializePropertyLambda(
                         parentType,
                         propertyType,
                         propertyName,
                         getterConstantExpression,
                         propertySerializerMethodInfo,
                         formatterExpression,
                         nullFormatterExpression);

            var @delegate = serializePropertyExpression.Compile();
            var delegateConstant = Expression.Constant(@delegate, @delegate.GetType());

            propertyExpressions.Add(delegateConstant);
        }

        #region Expressions 
        internal static ConstantExpression CreateSerializeNullConstant(Func<StringBuilder, bool> nullSerializer)
        {
            var constantExpression = Expression.Constant(nullSerializer, typeof(Func<StringBuilder, bool>));
            return constantExpression;
        }

        private static LambdaExpression CreateGetterLambdaExpression(Type entityType, Type propertyType, string propertyName)
        {
            var o = Expression.Parameter(entityType, "o");
            var getterMemberExpression = Expression.Property(o, entityType.GetTypeInfo().GetDeclaredProperty(propertyName));
            var getterExpression = Expression.Lambda(getterMemberExpression, new[] { o });
            return getterExpression;
        }

        private static LambdaExpression CreateSerializeObjectLambda(Type objectType, bool handleEmptyPropertyList, Expression[] serializeProperties)
        {
            var sbExpression = Expression.Parameter(typeof(StringBuilder), "sb");
            var tExpression = Expression.Parameter(objectType, "t");
            var serializeObjectMethodCallExpression =  CreateSerializeMethodCallExpression( sbExpression,  tExpression,  objectType,  handleEmptyPropertyList, serializeProperties);
            var objectFormatterLambda = Expression.Lambda(serializeObjectMethodCallExpression, new[] { sbExpression, tExpression });
            return objectFormatterLambda;
        }

        internal static MethodCallExpression CreateSerializeMethodCallExpression(ParameterExpression sbExpression, ParameterExpression tExpression, Type objectType, bool handleEmptyPropertyList, Expression[] serializeProperties)
        {
            MethodInfo serializeObjectGenericMethodInfo;
            if (handleEmptyPropertyList)
                serializeObjectGenericMethodInfo = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeObjectHandleEmpty));
            else
                serializeObjectGenericMethodInfo = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeObject));

            var serializeObjectResolvedMethodInfo = serializeObjectGenericMethodInfo.MakeGenericMethod(objectType);
            var serializePropertyFuncDelegateType = typeof(Func<,,>).MakeGenericType(typeof(StringBuilder), objectType, typeof(bool));

            var serializeObjectMethodCallExpression = Expression.Call(
                serializeObjectResolvedMethodInfo,
                new Expression[] { sbExpression, tExpression, Expression.NewArrayInit(serializePropertyFuncDelegateType, serializeProperties) }
            );
            return serializeObjectMethodCallExpression;
        }

        internal static bool? IsNullableStruct(Type type)
        {
            var @value = default(bool?);
            var nullableGenericType = Nullable.GetUnderlyingType(type);
            if (nullableGenericType != null)
                @value = true;
            else if (type.GetTypeInfo().IsValueType)
                @value = false;
            return @value;
        }

        

        internal static MethodCallExpression CreateSerializeArrayMethodCall(
            Type serializationType,
            bool? isNullableStruct,//SerializationPipeline propertyPipeline,
            ConstantExpression serializeExpression,
            ConstantExpression serializeNullExpression,
            ParameterExpression sbExpression,
            ParameterExpression tExpression,
            bool handleEmptyList
            )
        {
            bool itemNullSerializerRequired=true;
            MethodInfo serializePropertyMethod;
            // REM: my way to avoid (un)boxing ; other could be using something like this __refvalue(__makeref(v), int); 
            if (isNullableStruct == null)
            {
                if (handleEmptyList)
                    serializePropertyMethod = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeRefArrayHandleEmpty));
                else
                    serializePropertyMethod = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeRefArray));
            }
            else
            {
                if (isNullableStruct.Value)
                {
                    if (handleEmptyList)
                        serializePropertyMethod = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeNStructArrayHandleEmpty));
                    else
                        serializePropertyMethod = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeNStructArray));

                }
                else
                {
                    itemNullSerializerRequired = false;
                    if (handleEmptyList)
                        serializePropertyMethod = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeStructArrayHandleEmpty));
                    else
                        serializePropertyMethod = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeStructArray));
                }
            }

            var serializePropertyGenericMethodInfo = serializePropertyMethod.MakeGenericMethod(serializationType);
            MethodCallExpression methodCallExpression;
            if (itemNullSerializerRequired)
                methodCallExpression = Expression.Call(serializePropertyGenericMethodInfo, new Expression[] { sbExpression, tExpression, serializeExpression, serializeNullExpression });
            else
                methodCallExpression = Expression.Call(serializePropertyGenericMethodInfo, new Expression[] { sbExpression, tExpression, serializeExpression });
            return methodCallExpression;
        }

        private static MethodInfo GetEnumerablePropertySerializerMethodInfo(bool handleNullProperty)
        {
            MethodInfo serializePropertyMethod;
            if (!handleNullProperty)
                serializePropertyMethod = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeRefProperty));
            else
                serializePropertyMethod = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeRefPropertyHandleNull));
            return serializePropertyMethod;
        }

        private static MethodInfo GetPropertySerializerMethodInfo(bool? isNullableStruct, bool handleNullProperty)
        {
            MethodInfo serializePropertyMethod;
            if (isNullableStruct == null)
            {
                if (!handleNullProperty)
                    serializePropertyMethod = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeRefProperty));
                else
                    serializePropertyMethod = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeRefPropertyHandleNull));
            }
            else
            {
                if (isNullableStruct.Value)
                {
                    if (!handleNullProperty)
                        serializePropertyMethod = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeNStructProperty));
                    else
                        serializePropertyMethod = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeNStructPropertyHandleNull));
                }
                else
                {
                    serializePropertyMethod = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeStructProperty));
                }
            }
            return serializePropertyMethod;
        }

        private static LambdaExpression CreateSerializePropertyLambda(
            Type entityType,
            Type propertyCanonicType,
            string serializationName,
            Expression getterExpression,
            MethodInfo serializePropertyMethodInfo,
            ConstantExpression serializeLeafExpression,
            ConstantExpression serializeNullExpression)
        {
            var sb = Expression.Parameter(typeof(StringBuilder), "sb");
            var t  = Expression.Parameter(entityType, "t");

            var serializePropertyGeneric  = serializePropertyMethodInfo.MakeGenericMethod(entityType, propertyCanonicType);
            var serializationNameConstant = Expression.Constant(serializationName, typeof(string));

            MethodCallExpression methodCallExpression;
            if (serializeNullExpression == null)
                methodCallExpression = Expression.Call(serializePropertyGeneric,
                    new Expression[] { sb, t, serializationNameConstant, getterExpression, serializeLeafExpression }
                );
            else
                methodCallExpression = Expression.Call(serializePropertyGeneric,
                    new Expression[] { sb, t, serializationNameConstant, getterExpression, serializeLeafExpression, serializeNullExpression }
                );
            var serializePropertyLambda = Expression.Lambda(methodCallExpression, new[] { sb, t });
            return serializePropertyLambda;
        }
        #endregion

        public static MethodInfo GetMethodInfoExpr<T>(Expression<Func<StringBuilder, T, bool>> expression)
        {
            MethodInfo methodInfo = default(MethodInfo);
            if (expression.Body is MethodCallExpression)
            {
                var callExpression = (MethodCallExpression)expression.Body;
                var p0 = expression.Parameters[0];
                var p1 = expression.Parameters[1];
                var a0 = callExpression.Arguments[0];
                var a1 = callExpression.Arguments[1];
                if (p0 == a0 && p1 == a1)
                    methodInfo = callExpression.Method;
            }
            if (methodInfo == null)
            {
                var @delegate = expression.Compile();
                methodInfo = @delegate.GetMethodInfo();
            }
            return methodInfo;
        }
    }
}
