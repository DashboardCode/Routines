using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DashboardCode.Routines.Json
{
    public static class JsonManager
    {
        public static Func<T, string> ComposeFormatter<T>(
            Action<RulesDictionary<T>> config = null
            , bool useToString = false
            , string dateTimeFormat = null
            , string floatingPointFormat = null
            , bool handleEmptyObjectLiteral = true
            , bool handleEmptyArrayLiteral = true
            , Func<StringBuilder, bool> nullSerializer = null
            , bool handleNullProperty = true
            , Func<StringBuilder, bool> nullArraySerializer = null
            , bool handleNullArrayProperty = true
            , bool rootHandleNull = true
            , bool rootHandleEmptyObjectLiteral = true
            , int stringBuilderCapacity = 16
            , Func<LambdaExpression, Delegate> compile = null
            , bool doCompileInnerLambdas = true
            )
        {
            return ComposeFormatter(include: null, config: config, useToString: useToString, dateTimeFormat: dateTimeFormat,
                    floatingPointFormat: floatingPointFormat, handleEmptyObjectLiteral: handleEmptyObjectLiteral, handleEmptyArrayLiteral: handleEmptyArrayLiteral,
                    nullSerializer: nullSerializer, handleNullProperty: handleNullProperty, nullArraySerializer: nullArraySerializer,
                    handleNullArrayProperty: handleNullArrayProperty, rootHandleNull: rootHandleNull, rootHandleEmptyObjectLiteral: rootHandleEmptyObjectLiteral, stringBuilderCapacity: stringBuilderCapacity,
                    compile: compile, doCompileInnerLambdas: doCompileInnerLambdas
                );
        }

        public static Func<T, string> ComposeFormatter<T>(
           this Include<T> include
           , Action<RulesDictionary<T>> config = null
           , bool useToString = false
           , string dateTimeFormat = null
           , string floatingPointFormat = null
           , bool handleEmptyObjectLiteral = true
           , bool handleEmptyArrayLiteral = true
           , Func<StringBuilder, bool> nullSerializer = null
           , bool handleNullProperty = true
           , Func<StringBuilder, bool> nullArraySerializer = null
           , bool handleNullArrayProperty = true
           , bool rootHandleNull = true
           , bool rootHandleEmptyObjectLiteral = true
           , int stringBuilderCapacity = 16
           , Func<LambdaExpression, Delegate> compile = null
           , bool doCompileInnerLambdas = true)
        {
            ChainNode root = IncludeExtensions.CreateChainNode(include);
            if (include == null && config == null)
            {
                var type = typeof(T);
                if (type.IsAssociativeArrayType())
                {
                    root.AppendLeafs();
                }
            }
            return ComposeFormatter(root, config, useToString, dateTimeFormat, floatingPointFormat, handleEmptyObjectLiteral,
                    handleEmptyArrayLiteral, nullSerializer, handleNullProperty, nullArraySerializer,
                    handleNullArrayProperty, rootHandleNull, rootHandleEmptyObjectLiteral, stringBuilderCapacity, compile, doCompileInnerLambdas
                );
        }


        public static Func<T, string> ComposeFormatter<T>(
           this ChainNode root
           , Action<RulesDictionary<T>> config = null
           , bool useToString = false
           , string dateTimeFormat = null
           , string floatingPointFormat = null
           , bool handleEmptyObjectLiteral = true
           , bool handleEmptyArrayLiteral = true
           , Func<StringBuilder, bool> nullSerializer = null
           , bool handleNullProperty = true
           , Func<StringBuilder, bool> nullArraySerializer = null
           , bool handleNullArrayProperty = true
           , bool rootHandleNull = true
           , bool rootHandleEmptyObjectLiteral = true
           , int stringBuilderCapacity = 16
           , Func<LambdaExpression, Delegate> compile= null
           , bool doCompileInnerLambdas = true)
        {
            if (compile == null)
                compile = StandardCompile;
            var rulesDictionary = new RulesDictionary<T>(useToString, dateTimeFormat, floatingPointFormat,
                /*stringAsJsonLiteral*/ false,
                /*stringJsonEscape*/    true,
                nullSerializer ?? JsonValueStringBuilderExtensions.NullSerializer,
                handleNullProperty,
                    new InternalNodeOptions(
                        handleEmptyObjectLiteral: handleEmptyObjectLiteral,
                        handleEmptyArrayLiteral: handleEmptyArrayLiteral,
                        nullSerializer: nullSerializer ?? JsonValueStringBuilderExtensions.NullSerializer,
                        handleNullProperty: handleNullProperty,
                        nullArraySerializer: nullArraySerializer ?? JsonValueStringBuilderExtensions.NullSerializer,
                        handleNullArrayProperty: handleNullArrayProperty,
                        serializationName: null
                        ),
                compile: compile
                );
            config?.Invoke(rulesDictionary);

            Func<ChainNode, SerializerOptions> getSerializerOptions = n => rulesDictionary.GetLeafSerializerOptions(n);
            Func<ChainNode, InternalNodeOptions> getInternalNodeOptions = n => rulesDictionary.GeInternalNodeOptions(n);

            var serializer = ComposeSerializer<T>(root, getSerializerOptions, getInternalNodeOptions, rootHandleNull, rootHandleEmptyObjectLiteral, compile, doCompileInnerLambdas);
            return (t) => {
                var stringBuilder = new StringBuilder(stringBuilderCapacity);
                serializer(stringBuilder, t);
                return stringBuilder.ToString();
            };
        }

        public static Func<StringBuilder, T, bool> ComposeSerializer<T>(
            ChainNode root
            , Func<ChainNode, SerializerOptions> getSerializerOptions
            , Func<ChainNode, InternalNodeOptions> getInternalNodeOptions
            , bool rootHandleNull
            , bool rootHandleEmptyObjectLiteral
            , Func<LambdaExpression, Delegate> compile
            , bool doCompileInnerLambdas
            )
        {
            Func<StringBuilder, T, bool> @value = null;
            Expression serializeExpression;

            var objectType = typeof(T);
            var sbParameterExpression = Expression.Parameter(typeof(StringBuilder), "sb");
            var tParameterExpression = Expression.Parameter(objectType, "t");

            if (root.Children.Count == 0) // leaf
            {
                var serializerSet = getSerializerOptions(root);
                var isNullableStruct = JsonChainTools.IsNullableStruct(root.Type);
                if (isNullableStruct != null && isNullableStruct.Value == false)
                {
                    var delegateMethodInfo = serializerSet.Serializer.GetMethodInfo();
                    var target = serializerSet.Serializer.Target;
                    serializeExpression = Expression.Call(target != null ? Expression.Constant(target) : null, delegateMethodInfo, new Expression[] { sbParameterExpression, tParameterExpression });
                }
                else
                {
                    Expression nullCallExpression;
                    if (rootHandleNull)
                    {
                        if (serializerSet.NullSerializer == null)
                            throw new NotSupportedException($"Null serializer is not setuped for root node");
                        var rootNullSerializeMethodInfo = serializerSet.NullSerializer.GetMethodInfo();
                        nullCallExpression = Expression.Call(rootNullSerializeMethodInfo, new Expression[] { sbParameterExpression });
                    }
                    else
                    {
                        nullCallExpression = Expression.Constant(false);
                    }

                    if (isNullableStruct == null) // class
                    {
                        var @delegate = serializerSet.Serializer.GetMethodInfo();
                        var methodCallExpression = Expression.Call(@delegate, new Expression[] { sbParameterExpression, tParameterExpression });
                        serializeExpression = Expression.Condition(
                            Expression.Equal(tParameterExpression, Expression.Constant(null)),
                            nullCallExpression,
                            methodCallExpression
                        );
                    }
                    else if (isNullableStruct.Value)
                    {
                        var @delegate = serializerSet.Serializer.GetMethodInfo();
                        var hasValueExpression = Expression.Property(tParameterExpression, "HasValue");
                        var valueExpression = Expression.Property(tParameterExpression, "Value");
                        var methodCallExpression = Expression.Call(@delegate, new Expression[] { sbParameterExpression, valueExpression });
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
                var serializerSet = getInternalNodeOptions(root);
                var properies = new List<Expression>();
                foreach (var c in root.Children)
                {
                    var n = c.Value;
                    JsonChainTools.ConfigureSerializeProperty(n, root.Type, properies, getSerializerOptions, getInternalNodeOptions, compile, doCompileInnerLambdas);
                }
                var methodCallExpression = JsonChainTools.CreateSerializeMethodCallExpression(sbParameterExpression, tParameterExpression, objectType, rootHandleEmptyObjectLiteral, properies.ToArray());

                if (JsonChainTools.IsNullable(root.Type))
                {
                    if (rootHandleNull)
                    {
                        if (serializerSet.NullSerializer == null)
                            throw new NotSupportedException($"Null serializer is not setuped for internal node '{root.FindLinkedRootXPath()}' ");
                        var rootNullSerializeMethodInfo = serializerSet.NullSerializer.GetMethodInfo();
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
                else // struct
                {
                    serializeExpression = methodCallExpression;
                }
            }
            var serializeLambda = Expression.Lambda(serializeExpression, new[] { sbParameterExpression, tParameterExpression });
            @value = (Func<StringBuilder, T, bool>)compile(serializeLambda);
            return @value;
        }

        public static Func<IEnumerable<T>, string> ComposeEnumerableFormatter<T>(
            this Include<T> include
            , Action<RulesDictionary<T>> config = null
            , bool useToString = false
            , string dateTimeFormat = null
            , string floatingPointFormat = null
            , bool handleEmptyObjectLiteral = true
            , bool handleEmptyArrayLiteral = true
            , Func<StringBuilder, bool> nullSerializer = null
            , bool handleNullProperty = true
            , Func<StringBuilder, bool> nullArraySerializer = null
            , bool handleNullArrayProperty = true
            , bool rootHandleNullArray = true
            , bool rootHandleEmptyArrayLiteral = true
            , int stringBuilderCapacity = 16
            , Func<LambdaExpression, Delegate> compile=null
            , bool doCompileInnerLambdas = true)
        {
            if (compile == null)
                compile = StandardCompile;
            ChainNode root = IncludeExtensions.CreateChainNode(include);
            if (include == null && config == null)
            {
                var type = typeof(T);
                if (type.IsAssociativeArrayType())
                {
                    root.AppendLeafs();
                }
            }

            return ComposeEnumerableFormatter(root, config, useToString, dateTimeFormat, floatingPointFormat,
                handleEmptyObjectLiteral, handleEmptyArrayLiteral, nullSerializer, handleNullProperty, nullArraySerializer,
                handleNullArrayProperty, rootHandleNullArray, rootHandleEmptyArrayLiteral, stringBuilderCapacity, compile, doCompileInnerLambdas);

        }

        private static Delegate StandardCompile(LambdaExpression expression)
        {
            return expression.Compile();
        }

        public static Func<IEnumerable<T>, string> ComposeEnumerableFormatter<T>(
            this ChainNode root
            , Action<RulesDictionary<T>> config = null
            , bool useToString = false
            , string dateTimeFormat = null
            , string floatingPointFormat = null
            , bool handleEmptyObjectLiteral = true
            , bool handleEmptyArrayLiteral = true
            , Func<StringBuilder, bool> nullSerializer = null
            , bool handleNullProperty = true
            , Func<StringBuilder, bool> nullArraySerializer = null
            , bool handleNullArrayProperty = true
            , bool rootHandleNullArray = true
            , bool rootHandleEmptyArrayLiteral = true
            , int stringBuilderCapacity = 16
            , Func<LambdaExpression, Delegate> compile = null
            , bool doCompileInnerLambdas = true)
        {
            if (compile == null)
                compile = StandardCompile;
            var rulesDictionary = new RulesDictionary<T>(useToString, dateTimeFormat, floatingPointFormat,
                /*stringAsJsonLiteral*/ false,
                /*stringJsonEscape*/    true,
                nullSerializer ?? JsonValueStringBuilderExtensions.NullSerializer, handleNullProperty,
                    new InternalNodeOptions(handleNullProperty: handleNullProperty,
                        handleNullArrayProperty: handleNullArrayProperty,
                        handleEmptyObjectLiteral: handleEmptyObjectLiteral,
                        handleEmptyArrayLiteral: handleEmptyArrayLiteral,
                        nullSerializer: nullSerializer ?? JsonValueStringBuilderExtensions.NullSerializer,
                        nullArraySerializer: nullArraySerializer ?? JsonValueStringBuilderExtensions.NullSerializer,
                        serializationName: null),
                compile:compile
                );
            config?.Invoke(rulesDictionary);

            Func<ChainNode, SerializerOptions> getLeafSerializerOptions = n => rulesDictionary.GetLeafSerializerOptions(n);
            Func<ChainNode, InternalNodeOptions> getInternalNodeOptions = n => rulesDictionary.GeInternalNodeOptions(n);

            var serializer = ComposeEnumerableSerializer<T>(root
                , getLeafSerializerOptions
                , getInternalNodeOptions
                , rootHandleNullArray
                , rootHandleEmptyArrayLiteral
                , compile: compile
                , doCompileInnerLambdas: doCompileInnerLambdas
                );
            return (t) =>
            {
                var stringBuilder = new StringBuilder(stringBuilderCapacity);
                serializer(stringBuilder, t);
                return stringBuilder.ToString();
            };
        }

        public static Func<IEnumerable<T>, string> ComposeEnumerableFormatter<T>(
            Action<RulesDictionary<T>> config = null
            , bool useToString = false
            , string dateTimeFormat = null
            , string floatingPointFormat = null
            , bool handleEmptyObjectLiteral = true
            , bool handleEmptyArrayLiteral = true
            , Func<StringBuilder, bool> nullSerializer = null
            , bool handleNullProperty = true
            , Func<StringBuilder, bool> nullArraySerializer = null
            , bool handleNullArrayProperty = true
            , bool rootHandleNullArray = true
            , bool rootHandleEmptyArrayLiteral = true
            , int stringBuilderCapacity = 16
            , Func<LambdaExpression, Delegate> compile = null
            , bool doCompileInnerLambdas = true)
        {
            return ComposeEnumerableFormatter<T>(include: null, config: config, useToString: useToString, dateTimeFormat: dateTimeFormat,
                floatingPointFormat: floatingPointFormat, handleEmptyObjectLiteral: handleEmptyObjectLiteral, handleEmptyArrayLiteral: handleEmptyArrayLiteral, nullSerializer: nullSerializer, handleNullProperty: handleNullProperty,
                rootHandleNullArray: rootHandleNullArray, rootHandleEmptyArrayLiteral: rootHandleEmptyArrayLiteral, stringBuilderCapacity: stringBuilderCapacity, compile: compile, doCompileInnerLambdas: doCompileInnerLambdas);
        }

        public static Func<StringBuilder, IEnumerable<T>, bool> ComposeEnumerableSerializer<T>(
            ChainNode root
            , Func<ChainNode, SerializerOptions> getSerializerOptions
            , Func<ChainNode, InternalNodeOptions> getInternalNodeOptions
            , bool rootHandleNullArray
            , bool rootHandleEmptyArrayLiteral
            , Func<LambdaExpression, Delegate> compile
            , bool doCompileInnerLambdas = true)
        {
            Func<StringBuilder, IEnumerable<T>, bool> @value = null;
            var enumerableType = typeof(IEnumerable<T>);

            var expressions = JsonChainTools.CreateSerializsPair(root,/* node.Type,*/ getSerializerOptions, getInternalNodeOptions, compile, doCompileInnerLambdas);

            ConstantExpression nullSerializerExpression = null;
            if (JsonChainTools.IsNullable(root.Type))
            {
                if (expressions.NullSerializer == null)
                    throw new NotSupportedException($"Null serializer is not setuped for root node");
                nullSerializerExpression = JsonChainTools.CreateSerializeNullConstant(expressions.NullSerializer);
            }

            var sbExpression = Expression.Parameter(typeof(StringBuilder), "sb");
            var tExpression = Expression.Parameter(enumerableType, "t");
            var isNullableStruct = JsonChainTools.IsNullableStruct(root.Type);

            var serializationType = Nullable.GetUnderlyingType(root.Type) ?? root.Type;

            var methodCallExpression = JsonChainTools.CreateSerializeArrayMethodCall(
                serializationType,
                isNullableStruct,
                expressions.SerializerExpression,
                nullSerializerExpression,
                sbExpression,
                tExpression,
                rootHandleEmptyArrayLiteral);

            Expression serializeConditionalExpression;
            if (rootHandleNullArray)
            {
                var serializers = getInternalNodeOptions(root);
                var rootNullMethodInfo = serializers.NullSerializer.GetMethodInfo();
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
            @value = (Func<StringBuilder, IEnumerable<T>, bool>)compile(serializeArrayLambda);
            return @value;
        }
    }
}
