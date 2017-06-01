using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Vse.Routines.Json
{
    public static class NExpJsonSerializerTools
    {
        #region Setting
        public class NodeSetting
        {
            public bool HandleNull   { get; set; } = true;
            public bool EscapeString { get; set; } = true;
            public MethodInfo FormatterMethodInfo { get; set; } 
        }

        private static Dictionary<Type, NodeSetting> Default = new Dictionary<Type, NodeSetting>();
        static NExpJsonSerializerTools()
        {
            Default = new Dictionary<Type, NodeSetting>()
            {
                {   typeof(bool),
                    new NodeSetting() {
                        FormatterMethodInfo = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeBool))
                    }
                },
                {   typeof(string),
                    new NodeSetting() {
                        FormatterMethodInfo = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeStringValue))
                    }
                }
            };
        }
        #endregion

        public static Func<T, string> BuildFormatter<T>()
        {
            var serializer = BuildSerializer<T>();
            return (t) =>
            {
                var stringBuilder = new StringBuilder();
                serializer(stringBuilder, t);
                return stringBuilder.ToString();
            };
        }

        public static Func<T, string> BuildFormatter<T>(Include<T> include)
        {
            var serializer = BuildSerializer<T>(include);
            return (t) =>
            {
                var stringBuilder = new StringBuilder();
                serializer(stringBuilder, t);
                return stringBuilder.ToString();
            };
        }
        public static Func<StringBuilder, T, bool> BuildSerializer<T>(Include<T> include)
        {
            var parser = new SerializerNExpParser<T>();
            var includable = new Includable<T>(parser);
            include.Invoke(includable);
            var serializerNode = parser.Root;
            return BuildSerializer<T>(serializerNode);
        }

        public static Func<StringBuilder, T, bool> BuildSerializer<T>()
        {
            var parser = new SerializerNExpParser<T>();
            var includable = new Includable<T>(parser);
            var serializerNode = parser.Root;
            return BuildSerializer<T>(serializerNode);
        }

        public static Func<StringBuilder, T, bool> BuildSerializer<T>(SerializerBaseNode node)
        {
            var defaultNodeSetting = new NodeSetting();
            Func<StringBuilder, T, bool> @value = null;

            if (!node.IsLeaf)
            {
                var properies = new List<Expression>();
                foreach(var c in node.Children)
                {
                    var n = c.Value;
                    ConfigureSerializeProperty(n, defaultNodeSetting.HandleNull, node.Type, properies);
                }
                var objectFormatterLambda = CreateSerializeObjectLambda(typeof(T), properies.ToArray());
                @value = ((Expression<Func<StringBuilder, T, bool>>)objectFormatterLambda).Compile();
            }
            else
            {
                var type = node.IsNullable ? node.TypeUnderlyingNullable : node.Type;
                var serializers = ConfigureSerializeValue(node, type, defaultNodeSetting.HandleNull, node.Type, false);
                var formatterExpression = serializers.Item1;
                var nullFormatterExpression = serializers.Item2;

                var serializeLeafExpression = CreateSerializeLeafLambda(
                         node.Type,
                         type,
                         node.SerializerPropertyPipeline,
                         formatterExpression,
                         nullFormatterExpression);

                @value = (Func<StringBuilder, T, bool>)serializeLeafExpression.Compile();

            }
            return @value;
        }

        public static Func<StringBuilder, IEnumerable<T>, bool> BuildEnumerableSerializer<T>(Include<T> include=null)
        {
            var parser = new SerializerNExpParser<T>();
            var includable = new Includable<T>(parser);
            if (include!=null)
                include.Invoke(includable);
            var serializerNode = parser.Root;
            return BuildEnumerableSerializer<T>(serializerNode);
        }

        public static Func<IEnumerable<T>, string> BuildEnumerableFormatter<T>(Include<T> include=null)
        {
            var serializer = BuildEnumerableSerializer(include);
            return (t) =>
            {
                var stringBuilder = new StringBuilder();
                serializer(stringBuilder, t);
                return stringBuilder.ToString();
            };
        }

        public static Func<StringBuilder, IEnumerable<T>, bool> BuildEnumerableSerializer<T>(SerializerBaseNode node)
        {
            var defaultNodeSetting = new NodeSetting();
            Func<StringBuilder, IEnumerable<T>, bool> @value = null;
            var enumerableType = typeof(IEnumerable<T>);
            var serializers = ConfigureSerializeValue(node, node.Type, defaultNodeSetting.HandleNull, node.Type, false);
            var serializerExpression = serializers.Item1;
            var nullSerializerExpression = serializers.Item2;

            var type = node.IsNullable ? node.TypeUnderlyingNullable : node.Type;
            var serializeArrayLambda = CreateSerializeArrayLambda(
                type,
                enumerableType,
                node.SerializerPropertyPipeline,
                true,
                serializerExpression,
                nullSerializerExpression);
            @value = (Func<StringBuilder, IEnumerable<T>, bool>)serializeArrayLambda.Compile();
            return @value;
        }

        public static Tuple<Expression,Expression> ConfigureSerializeValue(SerializerBaseNode node, Type typeUnderlyingEnumerable, bool handleNull, Type parentType, bool isEnumerable)
        {
            Expression serializeNullExpression = null;
            if (handleNull && (node.SerializerPropertyPipeline == SerializerPropertyPipeline.NullableStruct || node.SerializerPropertyPipeline == SerializerPropertyPipeline.Object))
            {
                var nullMethodInfo = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeNull));
                serializeNullExpression = CreateSerializeNullConstant(nullMethodInfo);
            }

            Expression serializeExpression;
            if (isEnumerable)
            {
                // check that property should be serailizable: SerializeRefProperty
                serializeExpression = ConfigureSerializeArray(node, typeUnderlyingEnumerable, handleNull, parentType);
            }
            else if (node.IsLeaf)
            {
                serializeExpression = ConfigureSerializeLeaf(node);
            }
            else // object
            {
                serializeExpression = ConfigureSerializeObject(node, handleNull, parentType);
            }
            return new Tuple<Expression, Expression>(serializeExpression, serializeNullExpression);
        }

        public static void ConfigureSerializeProperty(SerializerNode n, bool handleNull, Type parentType, List<Expression> propertyExpressions)
        {
            var propertyName = n.PropertyName;
            var getterLambdaExpression = CreateGetterLambdaExpression(parentType, n.Type, propertyName);
            var getterDelegate = getterLambdaExpression.Compile();
            var getterConstantExpression = Expression.Constant(getterDelegate, getterDelegate.GetType());

            var expressions      = ConfigureSerializeValue(n, n.TypeUnderlyingEnumerable, handleNull, parentType, n.IsEnumerable);
            var formatterExpression     = expressions.Item1;
            var nullFormatterExpression = expressions.Item2;

            var propertyType = n.Type;
            if (n.IsEnumerable)
                propertyType = n.TypeUnderlyingEnumerable;
            else if (n.SerializerPropertyPipeline == SerializerPropertyPipeline.NullableStruct)
                propertyType = n.TypeUnderlyingNullable;
            var serializePropertyExpression = CreateSerializePropertyLambda(
                         parentType,
                         propertyType,
                         propertyName,
                         n.SerializerPropertyPipeline,
                         n.IsEnumerable,
                         getterConstantExpression,
                         formatterExpression,
                         nullFormatterExpression);

            var @delegate = serializePropertyExpression.Compile();
            var delegateConstant = Expression.Constant(@delegate, @delegate.GetType());

            propertyExpressions.Add(delegateConstant);
        }

        public static MethodInfo GetFormatterForNode(SerializerBaseNode node)
        {
            MethodInfo formatterMethodInfo = default(MethodInfo);
            if (node.IsBoolean || node.IsNBoolean)
                formatterMethodInfo = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeBool));
            else if (node.IsString)
                formatterMethodInfo = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeEscapeString));
            else if (node.IsDateTime || node.IsNDateTime)
                formatterMethodInfo = typeof(NExpJsonSerializerFormatters).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerFormatters.SerializeToIso8601WithMs));
            else if (node.IsByteArray)
                formatterMethodInfo = typeof(NExpJsonSerializerFormatters).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerFormatters.SerializeBase64));

            else if (node.IsPrimitive || node.IsNPrimitive || node.IsDecimal || node.IsNDecimal)
                formatterMethodInfo = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeStruct));
            else if (node.SerializerPropertyPipeline == SerializerPropertyPipeline.Struct || node.SerializerPropertyPipeline == SerializerPropertyPipeline.NullableStruct)
                formatterMethodInfo = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeEscapeTextStruct));
            else
                formatterMethodInfo = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeEscapingTextRef));
            return formatterMethodInfo;
        }

        public static Delegate CreateSerializeLeafDelegate(Type propertyType, MethodInfo methodInfo)
        {
            var resolvedTypesMethodInfo = (methodInfo.IsGenericMethod) ? methodInfo.MakeGenericMethod(propertyType) : methodInfo;

            var formatterDelegateType = typeof(Func<,,>).MakeGenericType(typeof(StringBuilder), propertyType, typeof(bool));
            var @delegate = resolvedTypesMethodInfo.CreateDelegate(formatterDelegateType);
            return @delegate;
        }

        public static ConstantExpression ConfigureSerializeLeaf(SerializerBaseNode n)
        {
            MethodInfo formatterMethodInfo = GetFormatterForNode(n);
            Delegate @delegate = default(Delegate);
            if (n.IsNullable)
                @delegate = CreateSerializeLeafDelegate(n.TypeUnderlyingNullable, formatterMethodInfo);
            else
                @delegate = CreateSerializeLeafDelegate(n.Type, formatterMethodInfo);
            var constantExpression = Expression.Constant(@delegate, @delegate.GetType());
            return constantExpression;
        }

        public static ConstantExpression ConfigureSerializeArray(SerializerBaseNode node, Type typeUnderlyingEnumerable, bool handleNull, Type propertyType)
        {
            var serializers = ConfigureSerializeValue(node, typeUnderlyingEnumerable, handleNull, propertyType, false);
            var type = node.IsNullable ? node.TypeUnderlyingNullable : node.Type;
            var serializeArrayLambda = CreateSerializeArrayLambda(
                type, typeUnderlyingEnumerable, node.SerializerPropertyPipeline, false, 
                serializers.Item1, serializers.Item2);
            var serializeArrayDelegate = serializeArrayLambda.Compile();
            var serializeArrayExpressionConstant = Expression.Constant(serializeArrayDelegate, serializeArrayDelegate.GetType());
            return serializeArrayExpressionConstant;
        }

        public static ConstantExpression ConfigureSerializeObject(SerializerBaseNode node, bool handleNull, Type propertyType)
        {
            var properies = new List<Expression>();
            foreach (var c in node.Children)
            {
                var n = c.Value;
                ConfigureSerializeProperty(n, handleNull, node.Type, properies);
            }
            var objectFormatterLambda = CreateSerializeObjectLambda(node.Type, properies.ToArray());
            var @delegate = objectFormatterLambda.Compile();
            var delegateConstant = Expression.Constant(@delegate, @delegate.GetType());
            return delegateConstant;
        }

        #region ConstantExpressions

        private static ConstantExpression CreateSerializeNullConstant(MethodInfo methodInfo)
        {
            var formatterDelegateType = typeof(Func<,>).MakeGenericType(typeof(StringBuilder), typeof(bool));
            var @delegate = methodInfo.CreateDelegate(formatterDelegateType);
            var constantExpression = Expression.Constant(@delegate, formatterDelegateType);
            return constantExpression;
        }
        #endregion

        #region Lambdas
        private static LambdaExpression CreateGetterLambdaExpression(Type entityType, Type propertyType, string propertyName)
        {
            var o = Expression.Parameter(entityType, "o");
            var getterMemberExpression = Expression.Property(o, entityType.GetTypeInfo().GetDeclaredProperty(propertyName));
            var getterExpression = Expression.Lambda(getterMemberExpression, new[] { o });
            return getterExpression;
        }

        private static LambdaExpression CreateSerializeObjectLambda(Type objectType, Expression[] serializeProperties)
        {
            var sb = Expression.Parameter(typeof(StringBuilder), "sb");
            var t  = Expression.Parameter(objectType, "t");
            var serializeObjectGenericMethodInfo = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeObjectNotEmpty));
            var serializeObjectResolvedMethodInfo = serializeObjectGenericMethodInfo.MakeGenericMethod(objectType);
            var serializePropertyFuncDelegateType = typeof(Func<,,>).MakeGenericType(typeof(StringBuilder), objectType, typeof(bool));

            var serializeObjectMethodCallExpression = Expression.Call(
                serializeObjectResolvedMethodInfo,
                new Expression[] { sb, t, Expression.NewArrayInit(serializePropertyFuncDelegateType, serializeProperties) }
            );
            var objectFormatterLambda = Expression.Lambda(serializeObjectMethodCallExpression, new[] { sb, t });

            return objectFormatterLambda;
        }

        private static LambdaExpression CreateSerializeArrayLambda(
            Type entityType,
            Type enumerableType,
            SerializerPropertyPipeline propertyPipeline,
            bool handleEmptyList,
            Expression serializeExpression,
            Expression serializeNullExpression)
        {
            var sb = Expression.Parameter(typeof(StringBuilder), "sb");
            var t  = Expression.Parameter(enumerableType, "t");

            MethodInfo serializePropertyMethod;
            switch (propertyPipeline)
            {
                case SerializerPropertyPipeline.Struct:
                    if (handleEmptyList)
                        serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeStructArray));
                    else
                        serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeStructArrayNotEmpty));
                    break;
                case SerializerPropertyPipeline.Object:
                    if (handleEmptyList)
                        serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeRefArray));
                    else
                        serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeRefArrayNotEmpty));
                    break;
                case SerializerPropertyPipeline.NullableStruct:
                    if (handleEmptyList)
                        serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeNStructArray));
                    else
                        serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeNStructArrayNotEmpty));
                    break;
                default:
                    throw new NotImplementedException("Unsupported pipeline");
            }

            var serializePropertyGeneric = serializePropertyMethod.MakeGenericMethod(entityType);

            MethodCallExpression methodCallExpression;
            if (serializeNullExpression == null)
                methodCallExpression = Expression.Call(serializePropertyGeneric, new Expression[] {sb, t, serializeExpression});
            else
                methodCallExpression = Expression.Call(serializePropertyGeneric, new Expression[] {sb, t, serializeExpression, serializeNullExpression});
            var serializeArrayLambda = Expression.Lambda(methodCallExpression, new[] {sb, t});
            return serializeArrayLambda;
        }

        private static LambdaExpression CreateSerializeLeafLambda(
            Type type,
            Type underlyingNullableType,
            SerializerPropertyPipeline propertyPipeline,
            Expression serializeLeafExpression,
            Expression serializeNullExpression)
        {
            var sb = Expression.Parameter(typeof(StringBuilder), "sb");
            var t  = Expression.Parameter(type, "t");

            MethodInfo serializePropertyMethod;
            if (propertyPipeline == SerializerPropertyPipeline.Object)
            {
                if (serializeNullExpression == null)
                    serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeRefLeafNotNull));
                else
                    serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeRefLeaf));
            }
            else
            {
                switch (propertyPipeline)
                {
                    case SerializerPropertyPipeline.Struct:
                        serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeStructLeaf));
                        break;
                    case SerializerPropertyPipeline.NullableStruct:
                        if (serializeNullExpression == null)
                            serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeNStructLeafNotNull));
                        else
                            serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeNStructLeaf));
                        break;
                    default:
                        throw new NotImplementedException("Unsupported pipeline");
                }
            }

            var serializePropertyGeneric = serializePropertyMethod.MakeGenericMethod(underlyingNullableType);

            MethodCallExpression methodCallExpression;
            if (serializeNullExpression == null)
                methodCallExpression = Expression.Call(serializePropertyGeneric,
                    new Expression[] { sb, t, serializeLeafExpression }
                );
            else
                methodCallExpression = Expression.Call(serializePropertyGeneric,
                    new Expression[] { sb, t, serializeLeafExpression, serializeNullExpression }
                );
            var serializePropertyLambda = Expression.Lambda(methodCallExpression, new[] { sb, t });
            return serializePropertyLambda;
        }

        private static LambdaExpression CreateSerializePropertyLambda(
            Type entityType, 
            Type propertyType, 
            string serializationName,
            SerializerPropertyPipeline propertyPipeline,
            bool isEnumerable,
            Expression getterExpression,
            Expression serializeLeafExpression,
            Expression serializeNullExpression)
        {
            var sb = Expression.Parameter(typeof(StringBuilder), "sb");
            var t  = Expression.Parameter(entityType, "t");

            MethodInfo serializePropertyMethod;
            if (isEnumerable || propertyPipeline == SerializerPropertyPipeline.Object)
            {
                if (serializeNullExpression == null)
                    serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeRefPropertyNotNull));
                else
                    serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeRefProperty));
            }
            else
            {
                switch (propertyPipeline)
                {
                    case SerializerPropertyPipeline.Struct:
                        serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeStructProperty));
                        break;
                    case SerializerPropertyPipeline.NullableStruct:
                        if (serializeNullExpression == null)
                            serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeNStructPropertyNotNull));
                        else
                            serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeNStructProperty));
                        break;
                    default:
                        throw new NotImplementedException("Unsupported pipeline");
                }
            }

            var serializePropertyGeneric = serializePropertyMethod.MakeGenericMethod(entityType, propertyType);

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
    }
}
