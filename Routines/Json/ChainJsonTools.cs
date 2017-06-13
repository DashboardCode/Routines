using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Vse.Routines.Json
{
    //public enum ConfigItem {REGULAR, GENERIC_PRIMITIVE, GENERIC_VAL, GENERIC_REF};

    public class JsonSerializerSet
    {
        public JsonSerializerSet() {

        }
        public bool HandleEmptyArray                    { get; set; } = true;
        public bool HandleEmptyPropertiesList           { get; set; } = true;
        public bool HandleNullProperty                  { get; set; } = true;
        public bool HandleNullArrayProperty             { get; set; } = true;
        public MethodInfo SerializerMethodInfo          { get; set; }
        public MethodInfo NullSerializerMethodInfo      { get; set; }
        public MethodInfo NullArraySerializerMethodInfo { get; set; }
    }

    public class Config<T>
    {
        public List<Tuple<Include<T>, Func<SerializerBaseNode, JsonSerializerSet>>> rules = new List<Tuple<Include<T>, Func<SerializerBaseNode, JsonSerializerSet>>>();
        private Include<T> include;
        
        public Config(Include<T> include)
        {
            this.include = include;
            //var nullSerializerMethodInfo = GetDefaultNullSerializer();

            //var serializeRefMethodInfo = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeEscapingTextRef));
            //AddRefGenericRule       (serializeRefMethodInfo, nullSerializerMethodInfo);

            //var serializeValMethodInfo = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeEscapeTextVal));
            //AddStructGenericRule    (serializeValMethodInfo, nullSerializerMethodInfo);

            //var serializePrimitiveMethodInfo = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializePrimitive));
            //AddPrimitiveGenericRule (serializePrimitiveMethodInfo, nullSerializerMethodInfo);
        }

        //public static MethodInfo GetDefaultNullSerializer()
        //{
        //    var nullMethodInfo = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeNull));
        //    return nullMethodInfo;
        //}

        //public Config<T> AddTypeRule<TProp>(
        //    Func<StringBuilder, TProp, bool> serializer,
        //    Func<StringBuilder, bool> nullSerializer = null)
        //{
        //    var serializerMethodInfo = serializer.GetMethodInfo();
        //    var nullSerializerMethodInfo = (nullSerializer == null)?null: nullSerializer.GetMethodInfo();
        //    rules.Add(new Tuple<Include<T>, SerializerSet, ConfigItem>(
        //        include, 
        //        serializer.GetMethodInfo(),
        //        nullSerializer==null? null : nullSerializer.GetMethodInfo(), 
        //        ConfigItem.REGULAR));
        //    return this;
        //}

        public Config<T> AddNodeRule(
           Func<SerializerBaseNode, JsonSerializerSet> getSerializerSet)
        {
            rules.Add(new Tuple<Include<T>, Func<SerializerBaseNode, JsonSerializerSet>>(include, getSerializerSet));
            //var serializerMethodInfo = serializer.GetMethodInfo();
            //var nullSerializerMethodInfo = (nullSerializer == null) ? null : nullSerializer.GetMethodInfo();
            //rules.Add(new Tuple<Include<T>, MethodInfo, MethodInfo, ConfigItem>(
            //    include,
            //    serializer.GetMethodInfo(),
            //    nullSerializer == null ? null : nullSerializer.GetMethodInfo(),
            //    ConfigItem.REGULAR));
            return this;
        }

        //public Config<T> AddRule<TProp>(
        //    Func<StringBuilder, TProp, bool> serializer,
        //    Func<StringBuilder, bool> nullSerializer = null)
        //{
        //    var serializerMethodInfo = serializer.GetMethodInfo();
        //    var nullSerializerMethodInfo = (nullSerializer == null) ? null : nullSerializer.GetMethodInfo();
        //    rules.Add(new Tuple<Include<T>, MethodInfo, MethodInfo, ConfigItem>(
        //        include,
        //        serializer.GetMethodInfo(),
        //        nullSerializer == null ? null : nullSerializer.GetMethodInfo(),
        //        ConfigItem.REGULAR));
        //    return this;
        //}
        #region Generics
        //public Config<T> AddPrimitiveGenericRule(
        //    MethodInfo methodInfo,
        //    MethodInfo nullSerializerMethodInfo = null)
        //{
        //    rules.Add(new Tuple<Include<T>, SerializerSet, ConfigItem>(include, methodInfo, nullSerializerMethodInfo, ConfigItem.GENERIC_PRIMITIVE));
        //    return this;
        //}

        //public Config<T> AddStructGenericRule(
        //    MethodInfo methodInfo,
        //    MethodInfo nullSerializerMethodInfo = null)
        //{
        //    rules.Add(new Tuple<Include<T>, SerializerSet, ConfigItem>(include, methodInfo, nullSerializerMethodInfo, ConfigItem.GENERIC_VAL));
        //    return this;
        //}

        //public Config<T> AddRefGenericRule(
        //    MethodInfo methodInfo,
        //    MethodInfo nullSerializerMethodInfo = null)
        //{
        //    rules.Add(new Tuple<Include<T>, SerializerSet, ConfigItem>(include, methodInfo, nullSerializerMethodInfo, ConfigItem.GENERIC_REF));
        //    return this;
        //}

        //public Config<T> AddGenericRule(
        //    Func<StringBuilder, TProp, bool> serializer,
        //    Func<StringBuilder, bool> nullSerializer = null)
        //{
        //    return this;
        //}
        #endregion



        public Config<T> ForInclude(Include<T> include)
        {
            this.include = include;
            return this;
        }
    }

    public static class ChainJsonTools
    {
        public static Func<T, string> BuildFormatter<T>(Include<T> include = null, Action<Config<T>> configurate=null)
        {
            var config = new Config<T>(include);
            configurate?.Invoke(config);

            var serializer = BuildSerializer(include, config);
            return (t) =>
            {
                var stringBuilder = new StringBuilder();
                serializer(stringBuilder, t);
                return stringBuilder.ToString();
            };
        }

        public static Func<StringBuilder, T, bool> BuildSerializer<T>(Include<T> include, Config<T> config)
        {
            var parser = new SerializerChainParser<T>();
            var includable = new Includable<T>(parser);
            if (include!=null)
                include.Invoke(includable);
            var serializerNode = parser.Root;
            return BuildSerializer(serializerNode, config);
        }

        public static Func<StringBuilder, IEnumerable<T>, bool> BuildEnumerableSerializer<T>(Include<T> include=null, Config<T> config=null)
        {
            var parser = new SerializerChainParser<T>();
            var includable = new Includable<T>(parser);
            if (include!=null)
                include.Invoke(includable);
            var serializerNode = parser.Root;

            Func<StringBuilder, IEnumerable<T>, bool> @value = null;
            var enumerableType = typeof(IEnumerable<T>);

            Func<SerializerBaseNode, JsonSerializerSet> getSerializerSet = (n) => GetSerializerSet(config, n);


            var serializerSet = getSerializerSet(serializerNode);
            var expressions = ConfigureSerializeNode(serializerNode, serializerNode.Type, getSerializerSet);

            var sbExpression = Expression.Parameter(typeof(StringBuilder), "sb");
            var tExpression  = Expression.Parameter(enumerableType, "t");

            MethodCallExpression methodCallExpression = CreateSerializeArrayMethodCall(
                serializerNode.CanonicType,
                serializerNode.SerializerPropertyPipeline,
                serializerSet.HandleEmptyArray,
                expressions.Item1,
                expressions.Item2,
                sbExpression,
                tExpression);
            MethodCallExpression nullCallExpression =
                Expression.Call(serializerSet.NullArraySerializerMethodInfo, new Expression[] { sbExpression });
            Expression serializeConditionalExpression = Expression.Condition(
                Expression.Equal(tExpression, Expression.Constant(null)),
                nullCallExpression,
                methodCallExpression
            );

            var serializeArrayLambda = Expression.Lambda(serializeConditionalExpression, new[] { sbExpression, tExpression });
            @value = (Func<StringBuilder, IEnumerable<T>, bool>)serializeArrayLambda.Compile();
            return @value;
        }

        public static Func<IEnumerable<T>, string> BuildEnumerableFormatter<T>(Include<T> include=null, Action<Config<T>> configurate = null)
        {
            var config = new Config<T>(include);
            configurate?.Invoke(config);
            var serializer = BuildEnumerableSerializer(include, config);
            return (t) =>
            {
                var stringBuilder = new StringBuilder();
                serializer(stringBuilder, t);
                return stringBuilder.ToString();
            };
        }

        public static Func<StringBuilder, T, bool> BuildSerializer<T>(SerializerBaseNode node, Config<T> config = null)
        {
            Func<StringBuilder, T, bool> @value = null;
            Func<SerializerBaseNode, JsonSerializerSet> getSerializerSet = (n)=>GetSerializerSet(config, n);
            if (node.IsLeaf)
            {
                var serializerSet = getSerializerSet(node);
                var serializeLeafExpression = CreateSerializeRootLambda(
                    node.Type,
                    node.CanonicType,
                    node.SerializerPropertyPipeline,
                    serializerSet.SerializerMethodInfo,
                    serializerSet.NullSerializerMethodInfo);
                @value = (Func<StringBuilder, T, bool>)serializeLeafExpression.Compile();
            }
            else
            {
                var properies = new List<Expression>();
                foreach (var c in node.Children)
                {
                    var n = c.Value;
                    ConfigureSerializeProperty(n, node.Type, properies, getSerializerSet);
                }
                var objectFormatterLambda = CreateSerializeObjectLambda(typeof(T), properies.ToArray());
                @value = ((Expression<Func<StringBuilder, T, bool>>)objectFormatterLambda).Compile();
            }
            return @value;
        }
        
        public static JsonSerializerSet GetSerializerSet<T>(Config<T> config, SerializerBaseNode node)
        {
            
            var serializerSet = new JsonSerializerSet();
            //config.rules
            // --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- ---
            if (node.IsBoolean || node.IsNBoolean)
                serializerSet.SerializerMethodInfo = GetMethodInfoExpr<bool>    ((sb, t) => NExpJsonSerializerStringBuilderExtensions.SerializeBool(sb, t));
            else if (node.IsString)
                serializerSet.SerializerMethodInfo = GetMethodInfoExpr<string>  ((sb, t) => NExpJsonSerializerStringBuilderExtensions.SerializeEscapeString(sb, t));
            else if (node.IsDateTime || node.IsNDateTime)
                serializerSet.SerializerMethodInfo = GetMethodInfoExpr<DateTime>((sb, t) => NExpJsonSerializerFormatters.SerializeToIso8601WithMs(sb, t));
            else if (node.IsByteArray)
                serializerSet.SerializerMethodInfo = GetMethodInfoExpr<byte[]>  ((sb, t) => NExpJsonSerializerFormatters.SerializeBase64(sb, t));
            else if (node.IsDecimal || node.IsNDecimal)
                serializerSet.SerializerMethodInfo = GetMethodInfoExpr<decimal> ((sb, t) => NExpJsonSerializerStringBuilderExtensions.SerializePrimitive(sb, t));
            // --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- ---
            else
                serializerSet.SerializerMethodInfo = DefaultSerializer(node.CanonicType, node.SerializerPropertyPipeline, node.IsNPrimitive || node.IsPrimitive);
            // --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- ---
            if (serializerSet.HandleNullProperty && node.IsNullable)
                serializerSet.NullSerializerMethodInfo = GetNullSerializer();
            if (serializerSet.HandleNullArrayProperty)
                serializerSet.NullArraySerializerMethodInfo = GetNullSerializer(); 
            return serializerSet;
        }

        public static Tuple<ConstantExpression, ConstantExpression> ConfigureSerializeNode(SerializerBaseNode node, Type parentType, Func<SerializerBaseNode, JsonSerializerSet> getSerializerSet)
        {
            if (node.IsLeaf)
            {
                var serializerSet = getSerializerSet(node);
                var formatterDelegateType = typeof(Func<,,>).MakeGenericType(typeof(StringBuilder), node.CanonicType, typeof(bool));
                var genericResolvedDelegate = serializerSet.SerializerMethodInfo.CreateDelegate(formatterDelegateType, null);
                var serializerExpression = Expression.Constant(genericResolvedDelegate, genericResolvedDelegate.GetType());
                var nullSerializerExpression = CreateSerializeNullConstant(serializerSet.NullSerializerMethodInfo);
                return new Tuple<ConstantExpression, ConstantExpression>(serializerExpression, nullSerializerExpression);
            }
            else // object
            {
                var serializerSet = getSerializerSet(node);

                var properies = new List<Expression>();
                foreach (var c in node.Children)
                {
                    var n = c.Value;
                    ConfigureSerializeProperty(n, node.Type, properies, getSerializerSet);
                }
                var objectFormatterLambda = CreateSerializeObjectLambda(node.Type, properies.ToArray());
                var @delegate = objectFormatterLambda.Compile();
                var delegateConstant = Expression.Constant(@delegate, @delegate.GetType());
                var nullSerializerExpression = CreateSerializeNullConstant(serializerSet.NullSerializerMethodInfo);
                return new Tuple<ConstantExpression, ConstantExpression>(delegateConstant, nullSerializerExpression);
            }
        }

        public static void ConfigureSerializeProperty(SerializerPropertyNode node,  Type parentType, List<Expression> propertyExpressions, Func<SerializerBaseNode, JsonSerializerSet> getSerializerSet)
        {
            bool isEnumerable = node is SerializerEnumerablePropertyNode;
            
            var propertyName = node.PropertyName;
            var getterLambdaExpression = CreateGetterLambdaExpression(parentType, node.Type, propertyName);
            var getterDelegate = getterLambdaExpression.Compile();
            var getterConstantExpression = Expression.Constant(getterDelegate, getterDelegate.GetType());

            var propertyType = node.CanonicType;
            if (isEnumerable)
                propertyType = ((SerializerEnumerablePropertyNode)node).EnumerableType;

            Tuple<ConstantExpression, ConstantExpression> expressions;
            if (isEnumerable)
            {
                // check that property should be serailizable: SerializeRefProperty
                var serializerSet = getSerializerSet(node);
                var itemSerializers = ConfigureSerializeNode(node, parentType, getSerializerSet);
                var sbExpression = Expression.Parameter(typeof(StringBuilder), "sb");
                var tExpression = Expression.Parameter(propertyType, "t");
                MethodCallExpression methodCallExpression = CreateSerializeArrayMethodCall(
                    node.CanonicType,
                    node.SerializerPropertyPipeline,
                    serializerSet.HandleEmptyArray,
                    itemSerializers.Item1,
                    itemSerializers.Item2,
                    sbExpression,
                    tExpression);
                var serializeArrayLambda = Expression.Lambda(methodCallExpression, new[] { sbExpression, tExpression });
                var serializeArrayDelegate = serializeArrayLambda.Compile();
                var serializeArrayExpressionConstant = Expression.Constant(serializeArrayDelegate, serializeArrayDelegate.GetType());
                var nullSerializerExpressionConstant = CreateSerializeNullConstant(serializerSet.NullArraySerializerMethodInfo);
                expressions= new Tuple<ConstantExpression, ConstantExpression>(serializeArrayExpressionConstant, nullSerializerExpressionConstant);
            }
            else
                expressions = ConfigureSerializeNode(node, parentType, getSerializerSet);

            var formatterExpression = expressions.Item1;
            var nullFormatterExpression = expressions.Item2;

            MethodInfo propertySerializerMethodInfo = (isEnumerable) ?
                GetEnumerablePropertySerializerMethodInfo(nullFormatterExpression != null) :
                GetPropertySerializerMethodInfo(node.SerializerPropertyPipeline,
                         nullFormatterExpression != null);

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
        
        #region ConstantExpressions
        private static ConstantExpression CreateSerializeNullConstant(MethodInfo methodInfo)
        {
            if (methodInfo == null)
                return null;
            var formatterDelegateType = typeof(Func<,>).MakeGenericType(typeof(StringBuilder), typeof(bool));
            var @delegate = methodInfo.CreateDelegate(formatterDelegateType,null);
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

        private static LambdaExpression CreateSerializeRootLambda(
            Type type,
            Type underlyingNullableType,
            SerializerPropertyPipeline propertyPipeline,
            MethodInfo serializeMethodInfo,
            MethodInfo serializeNullMethodInfo)
        {
            var sbExpressionParameter = Expression.Parameter(typeof(StringBuilder), "sb");
            var tExpressionParameter  = Expression.Parameter(type, "t");
            
            Expression serializeExpression = null;
            if (propertyPipeline == SerializerPropertyPipeline.Struct)
            {
                serializeExpression = Expression.Call(serializeMethodInfo,
                    new Expression[] { sbExpressionParameter, tExpressionParameter}
                );
            }
            else 
            {
                var nullCallExpression = Expression.Call(serializeNullMethodInfo, new Expression[] { sbExpressionParameter }); 
                if (propertyPipeline == SerializerPropertyPipeline.Object)
                {
                    var methodCallExpression = Expression.Call(serializeMethodInfo, new Expression[] { sbExpressionParameter, tExpressionParameter });
                    serializeExpression = Expression.Condition(
                        Expression.Equal(tExpressionParameter, Expression.Constant(null)),
                        nullCallExpression,
                        methodCallExpression
                    );
                }
                else if (propertyPipeline == SerializerPropertyPipeline.NullableStruct)
                {
                    var hasValueExpression   = Expression.Property(tExpressionParameter, "HasValue");
                    var valueExpression      = Expression.Property(tExpressionParameter, "Value");
                    var methodCallExpression = Expression.Call(serializeMethodInfo, new Expression[] { sbExpressionParameter, valueExpression });
                    serializeExpression = Expression.Condition(
                        hasValueExpression,
                        methodCallExpression,
                        nullCallExpression
                    );
                }
                else
                {
                    throw new NotSupportedException();
                }
            }

            var serializePropertyLambda = Expression.Lambda(serializeExpression, new[] { sbExpressionParameter, tExpressionParameter });
            return serializePropertyLambda;
        }

        private static MethodCallExpression CreateSerializeArrayMethodCall(
            Type entityType,
            SerializerPropertyPipeline propertyPipeline,
            bool handleEmptyList,
            ConstantExpression serializeExpression,
            ConstantExpression serializeNullExpression,
            ParameterExpression sbExpression,
            ParameterExpression tExpression
            )
        {

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

            var serializePropertyGenericMethodInfo = serializePropertyMethod.MakeGenericMethod(entityType);

            MethodCallExpression methodCallExpression;
            if (serializeNullExpression == null)
                methodCallExpression = Expression.Call(serializePropertyGenericMethodInfo, new Expression[] {sbExpression, tExpression, serializeExpression});
            else
                methodCallExpression = Expression.Call(serializePropertyGenericMethodInfo, new Expression[] {sbExpression, tExpression, serializeExpression, serializeNullExpression});
            return methodCallExpression;
        }

        //public static LambdaExpression ConfigureSerializeArrayLambda(SerializerBaseNode node, Type typeUnderlyingEnumerable, bool handleEmptyList, ConstantExpression serializerExpression, ConstantExpression nullSerializerExpression)
        //{
            
        //}

        private static MethodInfo GetEnumerablePropertySerializerMethodInfo(bool handleNullProperty)
        {
            MethodInfo serializePropertyMethod;
            if (!handleNullProperty)
                serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeRefPropertyNotNull));
            else
                serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeRefProperty));
            return serializePropertyMethod;
        }

        private static MethodInfo GetPropertySerializerMethodInfo(SerializerPropertyPipeline propertyPipeline, bool handleNullProperty)
        {
            MethodInfo serializePropertyMethod;
            if (propertyPipeline == SerializerPropertyPipeline.Object)
            {
                if (!handleNullProperty)
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
                        if (!handleNullProperty)
                            serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeNStructPropertyNotNull));
                        else
                            serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeNStructProperty));
                        break;
                    default:
                        throw new NotImplementedException("Unsupported pipeline");
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
            var t = Expression.Parameter(entityType, "t");

            var serializePropertyGeneric = serializePropertyMethodInfo.MakeGenericMethod(entityType, propertyCanonicType);

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

        public static MethodInfo GetMethodInfo<T>(Func<StringBuilder, T, bool> func)
        {
            var methodInfo = func.GetMethodInfo();
            return methodInfo;
        }

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
                methodInfo = GetMethodInfo(@delegate);
            }
            return methodInfo;
        }

        public static MethodInfo DefaultSerializer(Type type, SerializerPropertyPipeline serializerPropertyPipeline, bool isPrimitive)
        {
            var methodInfo = default(MethodInfo);
            if (isPrimitive)
            {
                var genericMethodInfo = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializePrimitive));
                methodInfo = genericMethodInfo.MakeGenericMethod(type);
            }
            else
            {
                MethodInfo genericMethodInfo = null;
                if (serializerPropertyPipeline == SerializerPropertyPipeline.Struct || serializerPropertyPipeline == SerializerPropertyPipeline.NullableStruct)
                    genericMethodInfo = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeEscapeTextVal));
                else
                    genericMethodInfo = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeEscapingTextRef));
                methodInfo = genericMethodInfo.MakeGenericMethod(type);
            }
            return methodInfo;
        }

        public static MethodInfo GetNullSerializer()
        {
            var nullMethodInfo = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeNull));
            return nullMethodInfo;
        }
    }
}
