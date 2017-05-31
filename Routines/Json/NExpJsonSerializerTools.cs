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
        public static Func<StringBuilder, T, bool> ExpressionBuilder<T>(Expression<Func<StringBuilder, T, bool>> expression)
        {
            var f = expression.Compile();
            return f;
        }

        public static Func<StringBuilder, T, bool> ExpressionBuilder<T, TProp>(Expression<Func<StringBuilder, T, bool>> expression)
        {
            var f = expression.Compile();
            return f;
        }

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
                throw new NotImplementedException("Not implemented yet. To be continued.");
            return @value;
        }

        public static Tuple<Expression,Expression> ConfigureSerializeValue(SerializerNode n, bool handleNull, Type parentType, bool isEnumerable )
        {
            Expression nullFormatterExpression = null;
            if (handleNull && (n.SerializerPropertyPipeline == SerializerPropertyPipeline.NullableStruct || n.SerializerPropertyPipeline == SerializerPropertyPipeline.Object))
            {
                var nullMethodInfo = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeNull));
                nullFormatterExpression = CreateSerializeNullConstant(nullMethodInfo);
            }

            Expression formatterExpression;
            if (isEnumerable)
            {
                // check that property should be serailizable: SerializeRefProperty
                formatterExpression = ConfigureSerializeArray(n, handleNull, parentType);
            }
            else if (n.IsLeaf)
            {
                formatterExpression = ConfigureSerializeLeaf(n);
            }
            else // object
            {
                formatterExpression = ConfigureSerializeObject(n, handleNull, parentType);
            }
            return new Tuple<Expression, Expression>(formatterExpression, nullFormatterExpression);
        }

        public static void ConfigureSerializeProperty(SerializerNode n, bool handleNull, Type parentType, List<Expression> propertyExpressions)
        {
            var propertyName = n.PropertyName;
            var getterLambdaExpression = CreateGetterLambdaExpression(parentType, n.Type, propertyName);
            var getterDelegate = getterLambdaExpression.Compile();
            var getterConstantExpression = Expression.Constant(getterDelegate, getterDelegate.GetType());
            //var @delegate = getterExpression.Compile();
            //var constantExpression = Expression.Constant(@delegate, @delegate.GetType());

            var expressions      = ConfigureSerializeValue(n, handleNull, parentType, n.IsEnumerable);
            var formatterExpression = expressions.Item1;
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

            propertyExpressions.Add(delegateConstant/*serializePropertyExpression*/);
        }

        private static Delegate CreateSerializer<T>(Expression<Func<StringBuilder, T, bool>> serializer)
        {
            var func = serializer.Compile();
            return func;
        }

        public static Expression ConfigureSerializeLeaf(SerializerNode n)
        {
            MethodInfo formatterMethodInfo = null;
            if (n.IsBoolean || n.IsNBoolean)
                formatterMethodInfo = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeBool));
            else if (n.IsString)
                formatterMethodInfo = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeEscapeString));
            else if (n.IsDateTime || n.IsNDateTime)
                formatterMethodInfo = typeof(NExpJsonSerializerFormatters).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerFormatters.SerializeToIso8601WithMs));
            else if (n.IsByteArray)
                formatterMethodInfo = typeof(NExpJsonSerializerFormatters).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerFormatters.SerializeBase64));

            else if (n.IsPrimitive || n.IsNPrimitive || n.IsDecimal || n.IsNDecimal)
                formatterMethodInfo = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeStruct));
            else if (n.SerializerPropertyPipeline == SerializerPropertyPipeline.Struct || n.SerializerPropertyPipeline == SerializerPropertyPipeline.NullableStruct)
                formatterMethodInfo = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeEscapeTextStruct));
            else
                formatterMethodInfo = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeEscapingTextRef));

            Expression formatterExpression = null;
            if (n.IsNullable)
                formatterExpression = CreateSerializeLeafConstant(n.TypeUnderlyingNullable, formatterMethodInfo);
            else
                formatterExpression = CreateSerializeLeafConstant(n.Type, formatterMethodInfo);
            return formatterExpression;
        }

        /*
        (sb, t) => NExpJsonSerializerStringBuilderExtensions.SerializeRefProperty(sb, "ListItems", t, o => o.ListItems,
                            (sb2, t2) => NExpJsonSerializerStringBuilderExtensions.ArrayFormatter(sb2, t2,
                                (sb3, t3) =>
                                    NExpJsonSerializerStringBuilderExtensions.ObjectFormatter(sb3, t3,
                                        (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeStuctProperty(sb4, "DateTime", t4, o => o.DateTime, NExpJsonSerializerFormatters.SerializeToIso8601WithSecUtc),
                                        (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeRefProperty(sb4, "RowData", t4, o => o.RowData, NExpJsonSerializerFormatters.SerializeBase64, NExpJsonSerializerStringBuilderExtensions.SerializeNull)
                                     )
                              ),
                            NExpJsonSerializerStringBuilderExtensions.FormatNull
                        )
        */
        public static Expression ConfigureSerializeArray(SerializerNode node, bool handleNull, Type propertyType)
        {
            var serializers = ConfigureSerializeValue(node, handleNull, propertyType, false);
            // create array formatter
            // parameter is generic formatter
            // SerializeRecursive(SerializerNode n, bool handleNull, Type parentType, List<Expression> propertyExpressions)
            // SerializerPropertyPipeline propertyPipeline,
            // bool notEmpty,
            var type = node.IsNullable ? node.TypeUnderlyingNullable : node.Type;
            Expression arrayFormatterLambda = CreateSerializeArrayLambda(
                type, node.TypeUnderlyingEnumerable, node.SerializerPropertyPipeline, false, 
                serializers.Item1, serializers.Item2);
            return arrayFormatterLambda;
        }

        public static Expression ConfigureSerializeObject(SerializerNode node, bool handleNull, Type propertyType)
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

        #region UnaryExpressions
        private static ConstantExpression CreateSerializeLeafConstant(Type propertyType, MethodInfo methodInfo)
        {
            var resolvedTypesMethodInfo = (methodInfo.IsGenericMethod) ? methodInfo.MakeGenericMethod(propertyType) : methodInfo;

            var formatterDelegateType = typeof(Func<,,>).MakeGenericType(typeof(StringBuilder), propertyType, typeof(bool));
            var @delegate = resolvedTypesMethodInfo.CreateDelegate(formatterDelegateType);
            var constantExpression = Expression.Constant(@delegate, formatterDelegateType);
            return constantExpression;
        }

        private static ConstantExpression CreateSerializeNullConstant(MethodInfo methodInfo)
        {
            var formatterDelegateType = typeof(Func<,>).MakeGenericType(typeof(StringBuilder), typeof(bool));
            var @delegate = methodInfo.CreateDelegate(formatterDelegateType);
            var constantExpression = Expression.Constant(@delegate, formatterDelegateType);
            return constantExpression;
        }

        public static LambdaExpression CreateGetterLambdaExpression(Type entityType, Type propertyType, string propertyName)
        {
            var o = Expression.Parameter(entityType, "o");
            var getterMemberExpression = Expression.Property(o, entityType.GetTypeInfo().GetDeclaredProperty(propertyName));
            var getterExpression = Expression.Lambda(getterMemberExpression, new[] { o });
            return getterExpression;
        }
        #endregion

        #region Lambdas
        private static LambdaExpression CreateSerializeObjectLambda(Type objectType, Expression[] serializeProperties)
        {
            //  (sbP, tP) => NExpJsonSerializerStringBuilderExtensions.SerializeObjectNotEmpty(sbP, tP, Func<StringBuilder, T, bool>[] propertySerializers)
            var sb = Expression.Parameter(typeof(StringBuilder), "sb");
            var t  = Expression.Parameter(objectType, "t");
            //Delegate x = NExpJsonSerializerStringBuilderExtensions.SerializeObjectNotEmpty;
            var serializeObjectGenericMethodInfo = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeObjectNotEmpty));
            var serializeObjectResolvedMethodInfo = serializeObjectGenericMethodInfo.MakeGenericMethod(objectType);

            var serializePropertyFuncDelegateType = typeof(Func<,,>).MakeGenericType(typeof(StringBuilder), objectType, typeof(bool));

            //var serializersArrayType = serializePropertyFuncDelegateType.MakeArrayType();
            //var paramFuncDelegateType = typeof(Func<,,>).MakeGenericType(typeof(StringBuilder), objectType, typeof(bool));
            ////should be like bool SerializeObjectNotEmpty<T>(StringBuilder stringBuilder, T t, params Func<StringBuilder, T, bool>[] propertySerializers)
            //var serializerObjectFuncDelegateType = typeof(Func<,,,>).MakeGenericType(typeof(StringBuilder), objectType, serializersArrayType, typeof(bool));
            
            //var x = serializeObjectResolvedMethodInfo.CreateDelegate(serializerObjectFuncDelegateType);
            //var @delegate =  serializeObjectResolvedMethodInfo.CreateDelegate(serializerObjectFuncDelegateType);
            //var constantExpression = Expression.Constant(@delegate, @delegate.GetType());
            
            //var getterExpression = Expression.Lambda(getterMemberExpression, new[] { sb, t });
            // create Func<StringBuilder,T,bool> delegate type
            //var funcDelegateType = typeof(Func<,,>).MakeGenericType(typeof(StringBuilder), objectType, typeof(bool));
            //serializeObjectResolvedMethodInfo.CreateDelegate(funcDelegateType);

            var serializeObjectMethodCallExpression = Expression.Call(
                serializeObjectResolvedMethodInfo,
                new Expression[] { sb, t, Expression.NewArrayInit(serializePropertyFuncDelegateType, serializeProperties) }
            );
            var objectFormatterLambda = Expression.Lambda(/*constantExpression*/ serializeObjectMethodCallExpression, new[] { sb, t });

            return objectFormatterLambda;
        }

        private static Expression CreateSerializeArrayLambda(
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
                methodCallExpression = Expression.Call(serializePropertyGeneric,
                    new Expression[] { sb, t, serializeExpression }
                );
            else
                methodCallExpression = Expression.Call(serializePropertyGeneric,
                    new Expression[] { sb, t, serializeExpression, serializeNullExpression }
                );
            var serializeArrayLambda = Expression.Lambda(methodCallExpression, new[] { sb, t });

            var @delegate = serializeArrayLambda.Compile();
            var delegateConstant = Expression.Constant(@delegate, @delegate.GetType());
            return delegateConstant;

            //return serializePropertyLambda;
        }


        //private static LambdaExpression CreateGetterLambda(Type entityType, Type propertyType, string propertyName)
        //{
        //    var o = Expression.Parameter(entityType, "o");
        //    var getterMemberExpression = Expression.Property(o, entityType.GetTypeInfo().GetDeclaredProperty(propertyName));
        //    var getterExpression = Expression.Lambda(getterMemberExpression, new[] { o });
        //    return getterExpression;
        //}

        public static LambdaExpression CreateSerializePropertyLambda(
            Type entityType, 
            Type propertyType, 
            string serializationName,
            SerializerPropertyPipeline propertyPipeline,
            bool isEnumerable,
            Expression getterExpression,
            Expression serializeExpression,
            Expression serializeNullExpression)
        {
            var sb = Expression.Parameter(typeof(StringBuilder), "sb");
            var t  = Expression.Parameter(entityType, "t");

            MethodInfo serializePropertyMethod;
            if (isEnumerable || propertyPipeline== SerializerPropertyPipeline.Object)
                if (serializeNullExpression == null)
                    serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeRefPropertyNotNull));
                else
                    serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeRefProperty));
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
                    new Expression[] { sb, t, serializationNameConstant, getterExpression, serializeExpression }
                );
            else
                methodCallExpression = Expression.Call(serializePropertyGeneric,
                    new Expression[] { sb, t, serializationNameConstant, getterExpression, serializeExpression, serializeNullExpression }
                );
            var serializePropertyLambda = Expression.Lambda(methodCallExpression, new[] { sb, t });
            return serializePropertyLambda;

            //var @delegate = serializePropertyLambda.Compile();
            //var delegateConstant = Expression.Constant(@delegate, @delegate.GetType());
            //return delegateConstant;
        }
        #endregion
    }
}
