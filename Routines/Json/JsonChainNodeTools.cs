using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Vse.Routines.Json
{
    public class JsonLeafSerializerSet
    {
        public bool HandleNullProperty { get; set; } = true;
        public MethodInfo SerializerMethodInfo { get; set; }
        public MethodInfo NullSerializerMethodInfo { get; set; }
    }

    public class JsonInternalSerializerSet
    {
        public bool HandleEmptyObjectLiteral { get; set; } = true;
        public bool HandleEmptyArrayLiteral { get; set; } = true;
        public bool HandleNullProperty { get; set; } = true;
        public bool HandleNullArrayProperty { get; set; } = true;
        public MethodInfo NullSerializer { get; set; }
    }

    public class Config<T>
    {
        public List<Tuple<Include<T>, Func<ChainNode, JsonLeafSerializerSet>>> rules = new List<Tuple<Include<T>, Func<ChainNode, JsonLeafSerializerSet>>>();
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

        public Config<T> AddNodeRule(Func<ChainNode, JsonLeafSerializerSet> getLeafSerializerSet)
        {
            rules.Add(new Tuple<Include<T>, Func<ChainNode, JsonLeafSerializerSet>>(include, getLeafSerializerSet));
            //var serializerMethodInfo = serializer.GetMethodInfo();
            //var nullSerializerMethodInfo = (nullSerializer == null) ? null : nullSerializer.GetMethodInfo();
            //rules.Add(new Tuple<Include<T>, MethodInfo, MethodInfo, ConfigItem>(
            //    include,
            //    serializer.GetMethodInfo(),
            //    nullSerializer == null ? null : nullSerializer.GetMethodInfo(),
            //    ConfigItem.REGULAR));
            return this;
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

    public static class JsonChainNodeTools
    {
        public static Func<T, string> BuildFormatter<T>(
            Include<T> include = null
            , Func<ChainNode, JsonLeafSerializerSet> getLeafSerializerSet = null
            , Func<ChainNode, bool, JsonInternalSerializerSet> getInternalSerializerSet = null
            , bool rootHandleNull = true
            , bool rootHandleEmptyObjectLiteral = true
            , MethodInfo rootNullSerializeMethodInfo = null)
        {
            var serializer = BuildSerializer(include, getLeafSerializerSet, getInternalSerializerSet, rootHandleNull, rootHandleEmptyObjectLiteral, rootNullSerializeMethodInfo);
            return (t) =>
            {
                var stringBuilder = new StringBuilder();
                serializer(stringBuilder, t);
                return stringBuilder.ToString();
            };
        }
                                                                                          
        public static Func<StringBuilder, T, bool> BuildSerializer<T>(
            Include<T> include
            , Func<ChainNode, JsonLeafSerializerSet> getLeafSerializerSet
            , Func<ChainNode, bool, JsonInternalSerializerSet> getInternalSerializerSet = null
            , bool rootHandleNull = true
            , bool rootHandleEmptyObjectLiteral = true
            , MethodInfo rootNullSerializeMethodInfo = null)
        {
            var parser = new ChainVisitor<T>();
            var includable = new Chain<T>(parser);
            if (include!=null)
                include.Invoke(includable);
            var serializerNode = parser.Root;
            return BuildSerializer<T>(serializerNode, getLeafSerializerSet, getInternalSerializerSet, rootHandleNull, rootHandleEmptyObjectLiteral, rootNullSerializeMethodInfo);
        }

        public static Func<IEnumerable<T>, string> BuildEnumerableFormatter<T>(
            Include<T> include = null
            , Func<ChainNode, JsonLeafSerializerSet> getLeafSerializerSet = null
            , Func<ChainNode, bool, JsonInternalSerializerSet> getInternalSerializerSet = null
            , bool rootHandleNullArray = true
            , bool rootHandleEmptyArrayLiteral = true
            , MethodInfo nullSerializer = null
        )
        {
            //var config = new Config<T>(include);
            //configurate?.Invoke(config);
            var serializer = BuildEnumerableSerializer(include, getLeafSerializerSet, getInternalSerializerSet, rootHandleNullArray, rootHandleEmptyArrayLiteral, nullSerializer);
            return (t) =>
            {
                var stringBuilder = new StringBuilder();
                serializer(stringBuilder, t);
                return stringBuilder.ToString();
            };
        }

        public static Func<StringBuilder, IEnumerable<T>, bool> BuildEnumerableSerializer<T>(Include<T> include=null
            , Func<ChainNode, JsonLeafSerializerSet> getLeafSerializerSet = null
            , Func<ChainNode, bool, JsonInternalSerializerSet> getInternalSerializerSet = null
            , bool rootHandleNullArray = true
            , bool rootHandleEmptyArray = true
            , MethodInfo rootNullMethodInfo = null)
        {
            var parser = new ChainVisitor<T>();
            var includable = new Chain<T>(parser);
            if (include!=null)
                include.Invoke(includable);

            var node = parser.Root;

            Func<StringBuilder, IEnumerable<T>, bool> @value = null;
            var enumerableType = typeof(IEnumerable<T>);

            if (getLeafSerializerSet == null)
                getLeafSerializerSet = (n) => GetDefaultLeafSerializerSet(n);
            if (getInternalSerializerSet == null)
                getInternalSerializerSet = (n, b) => GetDefaultInternalSerializerSet(n, b);
            if (rootNullMethodInfo == null)
                rootNullMethodInfo = defaultNullMethodInfo;

            var expressions = ConfigureSerializeNode(node, node.Type, getLeafSerializerSet, getInternalSerializerSet);

            ConstantExpression nullSerializerExpression=null;
            if (IsNullable(node.Type)) {
                if (expressions.NullSerializer == null)
                    throw new NotSupportedException($"Null serializer is not setuped for root node");
                nullSerializerExpression = CreateSerializeNullConstant(expressions.NullSerializer);
            }

            var sbExpression = Expression.Parameter(typeof(StringBuilder), "sb");
            var tExpression  = Expression.Parameter(enumerableType, "t");
            var isNullableStruct = IsNullableStruct(node.Type);

            var serializationType = Nullable.GetUnderlyingType(node.Type) ?? node.Type;

            MethodCallExpression methodCallExpression = CreateSerializeArrayMethodCall(
                serializationType,
                isNullableStruct,
                expressions.Serializer,
                nullSerializerExpression,
                sbExpression,
                tExpression,
                rootHandleEmptyArray);


            Expression serializeConditionalExpression;
            if (rootHandleNullArray)
            {
                MethodCallExpression nullCallExpression = Expression.Call(rootNullMethodInfo, new Expression[] { sbExpression });
                serializeConditionalExpression = Expression.Condition(
                    Expression.Equal(tExpression, Expression.Constant(null)),
                    nullCallExpression,
                    methodCallExpression
                );
            }
            else
            {
                serializeConditionalExpression = Expression.Condition(
                    Expression.Equal(tExpression, Expression.Constant(null)),
                    Expression.Constant(false),
                    methodCallExpression
                );
            }

            var serializeArrayLambda = Expression.Lambda(serializeConditionalExpression, new[] { sbExpression, tExpression });
            @value = (Func<StringBuilder, IEnumerable<T>, bool>)serializeArrayLambda.Compile();
            return @value;
        }

        public static Func<StringBuilder, T, bool> BuildSerializer<T>(ChainNode node
            , Func<ChainNode, JsonLeafSerializerSet> getLeafSerializerSet = null
            , Func<ChainNode, bool, JsonInternalSerializerSet> getInternalSerializerSet = null
            , bool rootHandleNull = true
            , bool rootHandleEmptyObjectLiteral = true
            , MethodInfo rootNullSerializeMethodInfo = null
            )
        {
            Func<StringBuilder, T, bool> @value = null;
            if (getLeafSerializerSet == null)
                getLeafSerializerSet = (n) => GetDefaultLeafSerializerSet(n);
            if (getInternalSerializerSet == null)
                getInternalSerializerSet = (n, b) => GetDefaultInternalSerializerSet(n, b);
            if (rootNullSerializeMethodInfo == null)
                rootNullSerializeMethodInfo = defaultNullMethodInfo;

            Expression serializeExpression;

            var objectType = typeof(T);
            var sbParameterExpression = Expression.Parameter(typeof(StringBuilder), "sb");
            var tParameterExpression  = Expression.Parameter(objectType, "t");

            if (node.Children.Count == 0) // leaf
            {
                var serializerSet = getLeafSerializerSet(node);
                var isNullableStruct = IsNullableStruct(node.Type);
                if (!IsNullable(node.Type, rootHandleNull))
                {
                    var methodCallExpression = Expression.Call(serializerSet.SerializerMethodInfo, new Expression[] { sbParameterExpression, tParameterExpression });
                    serializeExpression = Expression.Condition(
                        Expression.Equal(tParameterExpression, Expression.Constant(null)),
                        Expression.Constant(false),
                        methodCallExpression
                    );
                }
                else
                {
                    if (rootNullSerializeMethodInfo == null)
                        throw new NotSupportedException($"Null serializer is not setuped for root node");
                    var nullCallExpression = Expression.Call(rootNullSerializeMethodInfo, new Expression[] { sbParameterExpression });
                    if (isNullableStruct == null) // class
                    {
                        var methodCallExpression = Expression.Call(serializerSet.SerializerMethodInfo, new Expression[] { sbParameterExpression, tParameterExpression });
                        serializeExpression = Expression.Condition(
                            Expression.Equal(tParameterExpression, Expression.Constant(null)),
                            nullCallExpression,
                            methodCallExpression
                        );
                    }
                    else if (isNullableStruct.Value)
                    {
                        var hasValueExpression = Expression.Property(tParameterExpression, "HasValue");
                        var valueExpression = Expression.Property(tParameterExpression, "Value");
                        var methodCallExpression = Expression.Call(serializerSet.SerializerMethodInfo, new Expression[] { sbParameterExpression, valueExpression });
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
            }
            else // internal (note: currently can't be nullable struct)
            {
                var properies = new List<Expression>();
                foreach (var c in node.Children)
                {
                    var n = c.Value;
                    ConfigureSerializeProperty(n, node.Type, properies, getLeafSerializerSet, getInternalSerializerSet);
                }
                var methodCallExpression = CreateSerializeMethodCallExpression(sbParameterExpression, tParameterExpression, objectType, rootHandleEmptyObjectLiteral, properies.ToArray());
                
                if (IsNullable(node.Type, rootHandleNull))
                {
                    if (rootNullSerializeMethodInfo == null)
                        throw new NotSupportedException($"Null serializer is not setuped for internal node '{node.GetXPathOfNode()}' ");
                    MethodCallExpression nullCallExpression = Expression.Call(rootNullSerializeMethodInfo, new Expression[] { sbParameterExpression });

                    serializeExpression = Expression.Condition(
                        Expression.Equal(tParameterExpression, Expression.Constant(null)),
                        nullCallExpression,
                        methodCallExpression
                    );
                }
                else
                {
                    serializeExpression = Expression.Condition(
                        Expression.Equal(tParameterExpression, Expression.Constant(null)),
                        Expression.Constant(false),
                        methodCallExpression
                    );
                }
            }
            var serializeLambda = Expression.Lambda(serializeExpression, new[] { sbParameterExpression, tParameterExpression });
            @value = ((Expression<Func<StringBuilder, T, bool>>)serializeLambda).Compile();
            return @value;
        }

        public static JsonLeafSerializerSet GetDefaultLeafSerializerSet(
                  ChainNode node, 
            RulesDictionary rulesDictionary    =null,
                       bool handleNullProperty =true,
                       bool useToString        =true)
        {
            if (rulesDictionary == null)
                rulesDictionary = RulesDictionary.CreateDefault();
            var serializationType = Nullable.GetUnderlyingType(node.Type) ?? node.Type;
            var methodInfo = rulesDictionary.GetRule(serializationType);

            if (methodInfo == null)
            {
                bool isPrimitive = serializationType.GetTypeInfo().IsPrimitive;
                if (isPrimitive)
                {
                    var genericMethodInfo = typeof(JsonValueStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonValueStringBuilderExtensions.SerializePrimitive));
                    methodInfo = genericMethodInfo.MakeGenericMethod(serializationType);
                }
                else
                {
                    if (useToString)
                    {
                        MethodInfo genericMethodInfo = null;
                        if (serializationType.GetTypeInfo().IsValueType)
                            genericMethodInfo = typeof(JsonValueStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonValueStringBuilderExtensions.SerializeEscapingTextVal));
                        else
                            genericMethodInfo = typeof(JsonValueStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonValueStringBuilderExtensions.SerializeEscapingTextRef));
                        methodInfo = genericMethodInfo.MakeGenericMethod(serializationType);
                    }
                    else
                    {
                        var msg = default(string);
                        if (node is ChainPropertyNode)
                        {
                            var n = ChainNodeTree.Instance.GetXPathOfNode((ChainPropertyNode)node, i => i.Parent);
                            msg = $"Node '{n}' included as leaf but formatter of its type '{serializationType.FullName}' is not setuped";
                        }
                        else
                            msg = $"Root node included as leaf but formatter of its type '{serializationType.FullName}' is not setuped";
                        throw new NotSupportedException(msg);
                    }
                }
            }

            return new JsonLeafSerializerSet()
            {
                HandleNullProperty = handleNullProperty, 
                SerializerMethodInfo = methodInfo,
                NullSerializerMethodInfo = defaultNullMethodInfo
            };
        }

        public static JsonInternalSerializerSet GetDefaultInternalSerializerSet(
            ChainNode node,
            bool isEnumerable,
            bool handleNullProperty = true,
            bool handleNullArrayProperty = true,
            bool handleEmptyObjectLiteral = true,
            bool handleEmptyArrayLiteral = true)
        {
            return new JsonInternalSerializerSet()
            {
                HandleNullProperty = handleNullProperty,
                HandleNullArrayProperty = handleNullArrayProperty,
                HandleEmptyObjectLiteral = handleEmptyObjectLiteral,
                HandleEmptyArrayLiteral = handleEmptyArrayLiteral,
                NullSerializer = defaultNullMethodInfo
            };
        }

        private static bool IsNullable(Type type)
        {
            var @value = !type.GetTypeInfo().IsValueType || Nullable.GetUnderlyingType(type) != null;
            return @value;
        }

        private static bool IsNullable(Type type, bool handleNullProperty)
        {
            var @value = !type.GetTypeInfo().IsValueType || Nullable.GetUnderlyingType(type) != null;
            if (@value)
                @value = handleNullProperty;
            return @value;
        }

        public class SerializersPair
        {
            public ConstantExpression Serializer { get; set; }
            public MethodInfo NullSerializer { get; set; }
            public bool HandleNullProperty { get; set; }

            public SerializersPair(ConstantExpression Serializer, MethodInfo NullSerializer, bool HandleNullProperty)
            {
                this.Serializer = Serializer;
                this.NullSerializer = NullSerializer;
                this.HandleNullProperty = HandleNullProperty;
            }
        }

        public static SerializersPair ConfigureSerializeNode(ChainNode node, Type parentType, 
            Func<ChainNode, JsonLeafSerializerSet> getLeafSerializerSet, Func<ChainNode, bool, JsonInternalSerializerSet> getInternalSerializerSet/*, bool isEnumerable*/)
        {
            if (node.Children.Count == 0) // leaf node
            {
                var leafSerializerSet = getLeafSerializerSet(node);
                var serializationType = Nullable.GetUnderlyingType(node.Type) ?? node.Type;
                var formatterDelegateType = typeof(Func<,,>).MakeGenericType(typeof(StringBuilder), serializationType, typeof(bool));
                var serializerMethodInfo = leafSerializerSet.SerializerMethodInfo;
                var genericResolvedDelegate = serializerMethodInfo.CreateDelegate(formatterDelegateType, null);
                var serializerExpression = Expression.Constant(genericResolvedDelegate, genericResolvedDelegate.GetType());
                return new SerializersPair(serializerExpression, leafSerializerSet.NullSerializerMethodInfo, leafSerializerSet.HandleNullProperty);
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
            , Func<ChainNode, JsonLeafSerializerSet> getLeafSerializerSet
            , Func<ChainNode, bool, JsonInternalSerializerSet> getInternalSerializerSet)
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

                var itemSerializers = ConfigureSerializeNode(node, parentType, getLeafSerializerSet, getInternalSerializerSet/*, true*/);

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
                    itemSerializers.Serializer,
                    nullItemSerializerExpression, //itemSerializers.NullSerializer,
                    sbParameterExpression,
                    tParameterExpression,
                    serializerInternalSet.HandleEmptyArrayLiteral);

                var serializeArrayLambda = Expression.Lambda(methodCallExpression, new[] { sbParameterExpression, tParameterExpression });
                var serializeArrayDelegate = serializeArrayLambda.Compile();

                formatterExpression = Expression.Constant(serializeArrayDelegate, serializeArrayDelegate.GetType());
                if (serializerInternalSet.HandleNullArrayProperty)
                    nullFormatterExpression = CreateSerializeNullConstant(serializerInternalSet.NullSerializer);
            }
            else
            {
                propertyType = serializationType;
                var expressions = ConfigureSerializeNode(node, parentType, getLeafSerializerSet, getInternalSerializerSet/*, false*/);
                formatterExpression = expressions.Serializer;

                if (IsNullable(node.Type, expressions.HandleNullProperty))
                {
                    if (expressions.NullSerializer == null)
                        throw new NotSupportedException($"Null serializer is not setuped for internal node '{node.GetXPathOfNode()}' ");
                    nullFormatterExpression = CreateSerializeNullConstant(expressions.NullSerializer);
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
        
        #region ConstantExpressions
        private static ConstantExpression CreateSerializeNullConstant(MethodInfo methodInfo)
        {
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

        private static LambdaExpression CreateSerializeObjectLambda(Type objectType, bool handleEmptyPropertyList, Expression[] serializeProperties)
        {
            var sbExpression = Expression.Parameter(typeof(StringBuilder), "sb");
            var tExpression = Expression.Parameter(objectType, "t");
            var serializeObjectMethodCallExpression =  CreateSerializeMethodCallExpression( sbExpression,  tExpression,  objectType,  handleEmptyPropertyList, serializeProperties);
            var objectFormatterLambda = Expression.Lambda(serializeObjectMethodCallExpression, new[] { sbExpression, tExpression });
            return objectFormatterLambda;
        }

        private static MethodCallExpression CreateSerializeMethodCallExpression(ParameterExpression sbExpression, ParameterExpression tExpression, Type objectType, bool handleEmptyPropertyList, Expression[] serializeProperties)
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

        private static bool? IsNullableStruct(Type type)
        {
            var @value = default(bool?);
            var nullableGenericType = Nullable.GetUnderlyingType(type);
            if (nullableGenericType != null)
                @value = true;
            else if (type.GetTypeInfo().IsValueType)
                @value = false;
            return @value;
        }

        

        private static MethodCallExpression CreateSerializeArrayMethodCall(
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

        //public static LambdaExpression ConfigureSerializeArrayLambda(SerializerBaseNode node, Type typeUnderlyingEnumerable, bool handleEmptyList, ConstantExpression serializerExpression, ConstantExpression nullSerializerExpression)
        //{
            
        //}

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

        static readonly MethodInfo defaultNullMethodInfo = typeof(JsonValueStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonValueStringBuilderExtensions.SerializeNull));
    }
}
