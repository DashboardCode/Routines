﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Vse.Routines.Json
{
    public class JsonSerializerSet
    {
        public bool HandleEmptyList    { get; set; } = true;
        public bool HandleNullProperty { get; set; } = true;
        public Tuple<MethodInfo, MethodInfo> Serializers { get; set; }
    }

    public class Config<T>
    {
        public List<Tuple<Include<T>, Func<ChainNode, JsonSerializerSet>>> rules = new List<Tuple<Include<T>, Func<ChainNode, JsonSerializerSet>>>();
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

        public Config<T> AddNodeRule(Func<ChainNode, JsonSerializerSet> getSerializerSet)
        {
            rules.Add(new Tuple<Include<T>, Func<ChainNode, JsonSerializerSet>>(include, getSerializerSet));
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

    public static class NavigationToJsonTools
    {
        public static Func<T, string> BuildFormatter<T>(Include<T> include = null, Func<ChainNode, bool, JsonSerializerSet> getSerializerSet = null)
        {
            var serializer = BuildSerializer(include, getSerializerSet);
            return (t) =>
            {
                var stringBuilder = new StringBuilder();
                serializer(stringBuilder, t);
                return stringBuilder.ToString();
            };
        }
                                                                                          
        public static Func<StringBuilder, T, bool> BuildSerializer<T>(Include<T> include, Func<ChainNode, bool, JsonSerializerSet> getSerializerSet)
        {
            var parser = new ChainVisitor<T>();
            var includable = new Chain<T>(parser);
            if (include!=null)
                include.Invoke(includable);
            var serializerNode = parser.Root;
            return BuildSerializer<T>(serializerNode, getSerializerSet);
        }

        public static Func<StringBuilder, IEnumerable<T>, bool> BuildEnumerableSerializer<T>(Include<T> include=null, Func<ChainNode, bool, JsonSerializerSet> getSerializerSet = null)
        {
            var parser = new ChainVisitor<T>();
            var includable = new Chain<T>(parser);
            if (include!=null)
                include.Invoke(includable);
            var serializerNode = parser.Root;

            Func<StringBuilder, IEnumerable<T>, bool> @value = null;
            var enumerableType = typeof(IEnumerable<T>);

            if (getSerializerSet == null)
                getSerializerSet = (n,b) => GetDefaultSerializerSet(n,b);

            var serializerSet = getSerializerSet(serializerNode, true);
            var expressions = ConfigureSerializeNode(serializerNode, serializerNode.Type, getSerializerSet);

            var sbExpression = Expression.Parameter(typeof(StringBuilder), "sb");
            var tExpression  = Expression.Parameter(enumerableType, "t");
            var isNullableStruct = IsNullableStruct(serializerNode.Type);
            var serializationType = Nullable.GetUnderlyingType(serializerNode.Type) ?? serializerNode.Type;
            MethodCallExpression methodCallExpression = CreateSerializeArrayMethodCall(
                serializationType,
                isNullableStruct,
                expressions.Item1,
                expressions.Item2,
                sbExpression,
                tExpression,
                serializerSet.HandleEmptyList);
            MethodCallExpression nullCallExpression =
                Expression.Call(serializerSet.Serializers.Item2, new Expression[] { sbExpression });
            Expression serializeConditionalExpression = Expression.Condition(
                Expression.Equal(tExpression, Expression.Constant(null)),
                nullCallExpression,
                methodCallExpression
            );

            var serializeArrayLambda = Expression.Lambda(serializeConditionalExpression, new[] { sbExpression, tExpression });
            @value = (Func<StringBuilder, IEnumerable<T>, bool>)serializeArrayLambda.Compile();
            return @value;
        }

        public static Func<IEnumerable<T>, string> BuildEnumerableFormatter<T>(
            Include<T> include=null,
            Func<ChainNode, bool, JsonSerializerSet> getSerializerSet = null
            )
        {
            //var config = new Config<T>(include);
            //configurate?.Invoke(config);
            var serializer = BuildEnumerableSerializer(include, getSerializerSet);
            return (t) =>
            {
                var stringBuilder = new StringBuilder();
                serializer(stringBuilder, t);
                return stringBuilder.ToString();
            };
        }

        public static Func<StringBuilder, T, bool> BuildSerializer<T>(ChainNode node, Func<ChainNode, bool, JsonSerializerSet> getSerializerSet = null)
        {
            Func<StringBuilder, T, bool> @value = null;
            if (getSerializerSet == null)
                getSerializerSet = (n, b) => GetDefaultSerializerSet(n, b);

            var serializerSet = getSerializerSet(node,false);
            if (/*node.IsLeaf*/ node.Children.Count == 0)
            {
                var serializeLeafExpression = CreateSerializeRootLambda(
                    node.Type,
                    //node.SerializationType,
                    //node.IsNullableStruct,
                    serializerSet.Serializers.Item1,
                    serializerSet.Serializers.Item2);
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
                var objectFormatterLambda = CreateSerializeObjectLambda(typeof(T), serializerSet.HandleEmptyList, properies.ToArray());
                @value = ((Expression<Func<StringBuilder, T, bool>>)objectFormatterLambda).Compile();
            }
            return @value;
        }

        public class TypeRulesDictionary
        {
            public static TypeRulesDictionary CreateDefault()
            {
                var d = new TypeRulesDictionary();
                d.AddTypeRule(typeof(bool),     GetMethodInfoExpr<bool>((sb, t) => NExpJsonSerializerStringBuilderExtensions.SerializeBool(sb, t)));
                d.AddTypeRule(typeof(string),   GetMethodInfoExpr<string>((sb, t) => NExpJsonSerializerStringBuilderExtensions.SerializeEscapeString(sb, t)));
                d.AddTypeRule(typeof(DateTime), GetMethodInfoExpr<DateTime>((sb, t) => NExpJsonSerializerFormatters.SerializeToIso8601WithMs(sb, t)));
                d.AddTypeRule(typeof(byte[]),   GetMethodInfoExpr<byte[]>((sb, t) => NExpJsonSerializerFormatters.SerializeBase64(sb, t)));
                d.AddTypeRule(typeof(decimal),  GetMethodInfoExpr<decimal>((sb, t) => NExpJsonSerializerStringBuilderExtensions.SerializePrimitive(sb, t)));
                return d;
            }

            Dictionary<Type, MethodInfo> dictionary = new Dictionary<Type, MethodInfo>();
            
            public void AddTypeRule<T>(
                Func<StringBuilder, T, bool> serializer
                //Func<StringBuilder, bool> nullSerializer = null
                )
            {
                var serializerMethodInfo = serializer.GetMethodInfo();
                AddTypeRule(typeof(T), serializerMethodInfo);
                //var nullSerializerMethodInfo = (nullSerializer == null)?null: nullSerializer.GetMethodInfo();
                //rules.Add(new Tuple<Include<T>, SerializerSet, ConfigItem>(
                //    include, 
                //    serializer.GetMethodInfo(),
                //    nullSerializer==null? null : nullSerializer.GetMethodInfo(), 
                //    ConfigItem.REGULAR));
                //return this;
            }

            public void AddTypeRule(
                Type type,
                MethodInfo methodInfo
                )
            {
                dictionary.Add(type, methodInfo);
            }

            public MethodInfo GetRule(Type type)
            {
                MethodInfo rule = null;
                dictionary.TryGetValue(type, out rule);
                return rule;
            }

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
        }

        public static JsonSerializerSet GetDefaultSerializerSet(
            ChainNode node, 
            bool isEnumerable,
            TypeRulesDictionary typeRulesDictionary    =null,
            bool                handleNullProperty     =true,
            bool                handleNullArrayProperty=true)
        {
            if (typeRulesDictionary == null)
                typeRulesDictionary = TypeRulesDictionary.CreateDefault();
            var serializationType = Nullable.GetUnderlyingType(node.Type) ?? node.Type;
            MethodInfo methodInfo = typeRulesDictionary.GetRule(serializationType);

            if (methodInfo == null)
                methodInfo = DefaultSerializer(serializationType);

            MethodInfo nullMethodInfo = null;
            if (isEnumerable)
            {
                if (handleNullArrayProperty)
                    nullMethodInfo = GetNullSerializer();
            }
            else if (node.Type.GetTypeInfo().IsClass || Nullable.GetUnderlyingType(node.Type)!=null)
            {
                if (handleNullProperty)
                    nullMethodInfo = GetNullSerializer();
            }

            var serializerSet = new JsonSerializerSet()
            {
                HandleNullProperty = true,
                HandleEmptyList    = false,
                Serializers        = new Tuple<MethodInfo, MethodInfo>(methodInfo, nullMethodInfo)
            };

            return serializerSet;
        }

        public static Tuple<ConstantExpression, ConstantExpression> ConfigureSerializeNode(ChainNode node, Type parentType, Func<ChainNode, bool, JsonSerializerSet> getSerializerSet)
        {
            var serializerSet = getSerializerSet(node, false);
            if (node.Children.Count == 0)
            {
                var serializationType = Nullable.GetUnderlyingType(node.Type) ?? node.Type;
                var formatterDelegateType = typeof(Func<,,>).MakeGenericType(typeof(StringBuilder), serializationType, typeof(bool));
                var genericResolvedDelegate = serializerSet.Serializers.Item1.CreateDelegate(formatterDelegateType, null);
                var serializerExpression = Expression.Constant(genericResolvedDelegate, genericResolvedDelegate.GetType());
                var nullSerializerExpression = CreateSerializeNullConstant(serializerSet.Serializers.Item2);
                return new Tuple<ConstantExpression, ConstantExpression>(serializerExpression, nullSerializerExpression);
            }
            else // object
            {
                var properies = new List<Expression>();
                foreach (var c in node.Children)
                {
                    var n = c.Value;
                    ConfigureSerializeProperty(n, node.Type, properies, getSerializerSet);
                }
                var objectFormatterLambda = CreateSerializeObjectLambda(node.Type, serializerSet.HandleEmptyList, properies.ToArray());
                var @delegate = objectFormatterLambda.Compile();
                var delegateConstant = Expression.Constant(@delegate, @delegate.GetType());
                var nullSerializerExpression = CreateSerializeNullConstant(serializerSet.Serializers.Item2);
                return new Tuple<ConstantExpression, ConstantExpression>(delegateConstant, nullSerializerExpression);
            }
        }

        public static void ConfigureSerializeProperty(ChainPropertyNode node,  Type parentType, List<Expression> propertyExpressions, Func<ChainNode, bool, JsonSerializerSet> getSerializerSet)
        {
            
            bool? isNullableStruct = IsNullableStruct(node.Type);
            var propertyName = node.PropertyName;
            var getterLambdaExpression = CreateGetterLambdaExpression(parentType, node.Type, propertyName);
            var getterDelegate = getterLambdaExpression.Compile();
            var getterConstantExpression = Expression.Constant(getterDelegate, getterDelegate.GetType());
            var serializationType = Nullable.GetUnderlyingType(node.Type) ?? node.Type;

            Type propertyType;
            Tuple<ConstantExpression, ConstantExpression> expressions;
            //bool isEnumerable = node is ChainEnumerablePropertyNode;
            if (node.IsEnumerable)
            {
                propertyType = typeof(IEnumerable<>).MakeGenericType(node.Type);//  ((ChainEnumerablePropertyNode)node).EnumerableType;
                // check that property should be serailizable: SerializeRefProperty
                var serializerSet = getSerializerSet(node, true);
                var itemSerializers = ConfigureSerializeNode(node, parentType, getSerializerSet);
                var itemSerializer = itemSerializers.Item1;
                var itemNullSerializer = itemSerializers.Item2;
                var sbParameterExpression = Expression.Parameter(typeof(StringBuilder), "sb");
                var tParameterExpression = Expression.Parameter(propertyType, "t");
                MethodCallExpression methodCallExpression = CreateSerializeArrayMethodCall(
                    serializationType,
                    isNullableStruct,
                    itemSerializer,
                    itemNullSerializer,
                    sbParameterExpression,
                    tParameterExpression,
                    serializerSet.HandleEmptyList);
                var serializeArrayLambda = Expression.Lambda(methodCallExpression, new[] { sbParameterExpression, tParameterExpression });
                var serializeArrayDelegate = serializeArrayLambda.Compile();
                var serializeArrayExpressionConstant = Expression.Constant(serializeArrayDelegate, serializeArrayDelegate.GetType());
                var nullSerializerExpressionConstant = CreateSerializeNullConstant(serializerSet.Serializers.Item2);
                expressions = new Tuple<ConstantExpression, ConstantExpression>(serializeArrayExpressionConstant, nullSerializerExpressionConstant);
            }
            else
            {
                propertyType = serializationType;
                expressions = ConfigureSerializeNode(node, parentType, getSerializerSet);
            }

            var formatterExpression = expressions.Item1;
            var nullFormatterExpression = expressions.Item2;

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

        private static LambdaExpression CreateSerializeObjectLambda(Type objectType, bool handleEmptyPropertyList, Expression[] serializeProperties)
        {
            var sb = Expression.Parameter(typeof(StringBuilder), "sb");
            var t  = Expression.Parameter(objectType, "t");
            MethodInfo serializeObjectGenericMethodInfo;
            if (handleEmptyPropertyList)
                serializeObjectGenericMethodInfo = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeObjectHandleEmpty));
            else
                serializeObjectGenericMethodInfo = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeObject));
            var serializeObjectResolvedMethodInfo = serializeObjectGenericMethodInfo.MakeGenericMethod(objectType);
            var serializePropertyFuncDelegateType = typeof(Func<,,>).MakeGenericType(typeof(StringBuilder), objectType, typeof(bool));

            var serializeObjectMethodCallExpression = Expression.Call(
                serializeObjectResolvedMethodInfo,
                new Expression[] { sb, t, Expression.NewArrayInit(serializePropertyFuncDelegateType, serializeProperties) }
            );
            var objectFormatterLambda = Expression.Lambda(serializeObjectMethodCallExpression, new[] { sb, t });

            return objectFormatterLambda;
        }

        private static bool? IsNullableStruct(Type type)
        {
            var @value = default(bool?);
            var nullableGenericType = Nullable.GetUnderlyingType(type);
            if (nullableGenericType != null)
                @value = true;
            else if (!type.GetTypeInfo().IsClass)
                @value = false;
            return @value;
        }

        private static LambdaExpression CreateSerializeRootLambda(
            Type type,
            //Type serializationType,
            //bool? isNullableStruct,
            MethodInfo serializeMethodInfo,
            MethodInfo serializeNullMethodInfo)
        {
            var sbExpressionParameter = Expression.Parameter(typeof(StringBuilder), "sb");
            var tExpressionParameter  = Expression.Parameter(type, "t");
            var isNullableStruct = IsNullableStruct(type);
            Expression serializeExpression = null;
            if (isNullableStruct!=null && isNullableStruct.Value==false)
            {
                serializeExpression = Expression.Call(serializeMethodInfo,
                    new Expression[] { sbExpressionParameter, tExpressionParameter}
                );
            }
            else 
            {
                var nullCallExpression = Expression.Call(serializeNullMethodInfo, new Expression[] { sbExpressionParameter }); 
                if (isNullableStruct == null)
                {
                    var methodCallExpression = Expression.Call(serializeMethodInfo, new Expression[] { sbExpressionParameter, tExpressionParameter });
                    serializeExpression = Expression.Condition(
                        Expression.Equal(tExpressionParameter, Expression.Constant(null)),
                        nullCallExpression,
                        methodCallExpression
                    );
                }
                else if (isNullableStruct.Value)
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
            Type serializationType,
            bool? isNullableStruct,//SerializationPipeline propertyPipeline,
            ConstantExpression serializeExpression,
            ConstantExpression serializeNullExpression,
            ParameterExpression sbExpression,
            ParameterExpression tExpression,
            bool handleEmptyList
            )
        {
            MethodInfo serializePropertyMethod;
            // REM: my way to avoid (un)boxing ; other could be using something like this __refvalue(__makeref(v), int); 
            if (isNullableStruct == null)
            {
                if (handleEmptyList)
                    serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeRefArrayHandleEmpty));
                else
                    serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeRefArray));
            }
            else
            {
                if (isNullableStruct.Value)
                {
                    if (handleEmptyList)
                        serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeNStructArrayHandleEmpty));
                    else
                        serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeNStructArray));

                }
                else
                {
                    if (handleEmptyList)
                        serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeStructArrayHandleEmpty));
                    else
                        serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeStructArray));
                }
            }

            var serializePropertyGenericMethodInfo = serializePropertyMethod.MakeGenericMethod(serializationType);

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
                serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeRefProperty));
            else
                serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeRefPropertyHandleNull));
            return serializePropertyMethod;
        }

        private static MethodInfo GetPropertySerializerMethodInfo(bool? isNullableStruct, bool handleNullProperty)
        {
            MethodInfo serializePropertyMethod;
            if (isNullableStruct == null)
            {
                if (!handleNullProperty)
                    serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeRefProperty));
                else
                    serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeRefPropertyHandleNull));
            }
            else
            {
                if (isNullableStruct.Value)
                {
                    if (!handleNullProperty)
                        serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeNStructProperty));
                    else
                        serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeNStructPropertyHandleNull));
                }
                else
                {
                    serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeStructProperty));
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

        public static MethodInfo DefaultSerializer(Type type)
        {
            bool isPrimitive = type.GetTypeInfo().IsPrimitive;
            var methodInfo = default(MethodInfo);
            if (isPrimitive)
            {
                var genericMethodInfo = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializePrimitive));
                methodInfo = genericMethodInfo.MakeGenericMethod(type);
            }
            else
            {
                MethodInfo genericMethodInfo = null;
                if ( type.GetTypeInfo().IsClass)
                    genericMethodInfo = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeEscapingTextRef));
                else
                    genericMethodInfo = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeEscapeTextVal));
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