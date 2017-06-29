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
        public static Func<StringBuilder, TestClass, bool> BuildSerializer2()
        {
            var formatterAsParameter = Expression.Constant(
                  typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeStringValue))
                , typeof(MethodInfo));

            var sb = Expression.Parameter(typeof(StringBuilder), "sb");
            var t = Expression.Parameter(typeof(TestClass), "t");
            var o = Expression.Parameter(typeof(TestClass), "o");

            var propertyExpression = Expression.Property(o, typeof(TestClass).GetTypeInfo().GetDeclaredProperty(nameof(TestClass.TextField1)));
            var propertyLambda = Expression.Lambda<Func<TestClass, string>>(propertyExpression, new[] { o });

            var createDelegateMethod =
                typeof(MethodInfo).GetTypeInfo().GetDeclaredMethods(nameof(MethodInfo.CreateDelegate)).First(m => m.GetParameters().Length == 2);

            var expressionConvert = Expression.Convert(Expression.Call(formatterAsParameter, createDelegateMethod, new Expression[] {
                Expression.Constant(typeof(Func<StringBuilder, string, bool>), typeof(Type)),
                Expression.Constant(null, typeof(object))
            }), typeof(Func<StringBuilder, string, bool>));

            var serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeStructProperty));
            var serializePropertyGeneric = serializePropertyMethod.MakeGenericMethod(typeof(TestClass), typeof(string));

            var serializeProperty = Expression.Call(serializePropertyGeneric,
                    new Expression[] { sb, Expression.Constant("TextField1", typeof(string)), t, propertyLambda, expressionConvert }
                    );

            var serializePropertyLambda = Expression.Lambda<Func<StringBuilder, TestClass, bool>>(serializeProperty, new[] { sb, t });
            // ---------------------
            var sbP = Expression.Parameter(typeof(StringBuilder), "sbP");
            var tP = Expression.Parameter(typeof(TestClass), "tP");

            var objectFormatterMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeObject));
            var objectFormatterGeneric = objectFormatterMethod.MakeGenericMethod(typeof(TestClass));

            MethodCallExpression objectFormatterExpression = Expression.Call(
                objectFormatterGeneric,
                new Expression[] { sbP, tP, Expression.NewArrayInit(typeof(Func<StringBuilder, TestClass, bool>), new[] { serializePropertyLambda }) }
            );

            var lambdaExpression = Expression.Lambda<Func<StringBuilder, TestClass, bool>>(objectFormatterExpression, new[] { sbP, tP });
            var func = lambdaExpression.Compile();
            return func;
        }

        public static Func<StringBuilder, T, bool> BuildSerializer3<T>(/*SerializerBaseNode node*/)
        {
            Func<StringBuilder, T, bool> @value = null;
            //if (!node.IsPrimitive)
            {
                var formatter0 = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeBool));
                var propertyName0 = "BoolField1";
                var unaryExpression0 = CreateOneStepConvert<bool>(formatter0);
                var getterExpression0 = CreateGetterLambda<T, bool>(propertyName0);
                var serializePropertyLambda0 = CreateSerializePropertyLambda(propertyName0, getterExpression0, unaryExpression0);

                var formatter1 = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeBool));
                var propertyName1 = "NBoolField2";
                var unaryExpression1 = CreateOneStepConvert<bool>(formatter1);
                var getterExpression1 = CreateGetterLambda<T, bool?>(propertyName1);
                var serializePropertyLambda1 = CreateSerializeNPropertyLambda<T, bool>("NBoolField2", getterExpression1, unaryExpression1);

                var formatter2 = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeStringValue));
                var propertyName2 = "TextField1";
                var unaryExpression2 = CreateOneStepConvert<string>(formatter2);
                var getterExpression2 = CreateGetterLambda<T, string>(propertyName2);
                var serializePropertyLambda2 = CreateSerializePropertyLambda<T, string>("TextField1", getterExpression2, unaryExpression2);

                var formatter3 = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeStruct));
                var propertyName3 = "Number";
                var unaryExpression3 = CreateOneStepConvert<int>(formatter3);
                var getterExpression3 = CreateGetterLambda<T, int>(propertyName3);
                var serializePropertyLambda3 = CreateSerializePropertyLambda<T, int>("Number", getterExpression3, unaryExpression3);

                var objectFormatterLambda = CreateObjectFormatterLambda<T>(new[] { serializePropertyLambda0, serializePropertyLambda1, serializePropertyLambda2, serializePropertyLambda3 });

                @value = objectFormatterLambda.Compile();
            }
            //else
            //    throw new NotImplementedException("");
            return @value;
        }

        private static Expression<Func<StringBuilder, T, bool>> CreateObjectFormatterLambda<T>(Expression[] serializeProperties)
        {
            var sbP = Expression.Parameter(typeof(StringBuilder), "sbP");
            var tP = Expression.Parameter(typeof(T), "tP");
            var objectFormatterMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeObject));
            var objectFormatterGeneric = objectFormatterMethod.MakeGenericMethod(typeof(T));

            MethodCallExpression objectFormatterExpression = Expression.Call(
                objectFormatterGeneric,
                new Expression[] { sbP, tP, Expression.NewArrayInit(typeof(Func<StringBuilder, T, bool>), serializeProperties) }
            );
            var objectFormatterLambda = Expression.Lambda<Func<StringBuilder, T, bool>>(objectFormatterExpression, new[] { sbP, tP });
            return objectFormatterLambda;
        }

        private static Expression<Func<StringBuilder, T, bool>> CreateSerializeNPropertyLambda<T, TProp>(
            string serializationName,
            Expression<Func<T, TProp?>> getterExpression,
            Expression formatExpression) where TProp : struct
        {
            var sb = Expression.Parameter(typeof(StringBuilder), "sb");
            var t = Expression.Parameter(typeof(T), "t");

            var serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeNStructProperty));
            var serializePropertyGeneric = serializePropertyMethod.MakeGenericMethod(typeof(T), typeof(TProp));
            var expr = Expression.Constant(serializationName, typeof(string));

            var serializeProperty = Expression.Call(serializePropertyGeneric,
                    new Expression[] { sb, t, expr, getterExpression, formatExpression }
            );
            var serializePropertyLambda = Expression.Lambda<Func<StringBuilder, T, bool>>(serializeProperty, new[] { sb, t });
            return serializePropertyLambda;
        }

        private static Expression<Func<StringBuilder, T, bool>> CreateSerializePropertyLambda<T, TProp>(
                string serializationName,
                Expression<Func<T, TProp>> getterExpression,
                Expression formatExpression)
        {
            var sb = Expression.Parameter(typeof(StringBuilder), "sb");
            var t = Expression.Parameter(typeof(T), "t");

            var serializePropertyMethod = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeStructProperty));
            var serializePropertyGeneric = serializePropertyMethod.MakeGenericMethod(typeof(T), typeof(TProp));
            var expr = Expression.Constant(serializationName, typeof(string));

            var serializeProperty = Expression.Call(serializePropertyGeneric,
                    new Expression[] { sb, t, expr, getterExpression, formatExpression }
            );
            var serializePropertyLambda = Expression.Lambda<Func<StringBuilder, T, bool>>(serializeProperty, new[] { sb, t });
            return serializePropertyLambda;
        }

        private static UnaryExpression CreateOneStepConvert<TProp>(MethodInfo methodInfo)
        {
            MethodInfo tmp = (methodInfo.IsGenericMethod) ? methodInfo.MakeGenericMethod(typeof(TProp)) : methodInfo;

            var formatterAsParameter = Expression.Constant(
                      tmp
                    , typeof(MethodInfo));

            var createDelegateMethod =
                typeof(MethodInfo).GetTypeInfo().GetDeclaredMethods(nameof(MethodInfo.CreateDelegate)).First(m => m.GetParameters().Length == 2);

            var expressionConvert = Expression.Convert(Expression.Call(formatterAsParameter, createDelegateMethod, new Expression[] {
                Expression.Constant(typeof(Func<StringBuilder, TProp, bool>), typeof(Type)),
                Expression.Constant(null, typeof(object))
            }), typeof(Func<StringBuilder, TProp, bool>));
            return expressionConvert;
        }

        private static Expression<Func<T, TProp>> CreateGetterLambda<T, TProp>(string propertyName)
        {
            var o = Expression.Parameter(typeof(T), "o");
            var getterMemberExpression = Expression.Property(o, typeof(T).GetTypeInfo().GetDeclaredProperty(propertyName));
            var getterExpression = Expression.Lambda<Func<T, TProp>>(getterMemberExpression, new[] { o });
            return getterExpression;
        }

        public static Func<StringBuilder, TestClass, bool> BuildSerializer()
        {
            var serializer = ExpressionBuilder<TestClass>(
                    (sbP, tP) => NExpJsonSerializerStringBuilderExtensions.SerializeObject(sbP, tP,
                        //(sb, t) => SerializeNStuctProperty(sb,     "NBoolField2", t, o => o.NBoolField2, FormatBool, WriteNull),
                        //(sb, t) => SerializeRefPropertyNotNull(sb, "TextField1",  t, o => o.TextField1,  FormatString),

                        //(sb, t) => SerializeRefProperty(sb, "TestField", t, o => o.TestClass1,
                        //    (sb2, t2) =>
                        //        ObjectFormatter(sb2, t2,
                        //           //(sb3, t3) => SerializeRefProperty(sb3, "TextField1", t3, o => o.TextField1, FormatString, WriteNull),
                        //           (sb3, t3) => SerializeStuctProperty(sb3, "BoolField1", t3, o => o.BoolField, FormatBool)
                        //           //,
                        //           //(sb3, t3) => SerializeProperty(sb3, "TestField", t3, o => o.TestField,
                        //           //     (sb4, t4) => ObjectFormatter(sb4, t4,
                        //           //        (sb5, t5) => SerializeProperty(sb5, "TextField1", t5, o => o.TextField1, FormatString)
                        //           //     )
                        //           //)
                        //        ), WriteNull
                        //)
                        //,
                        //(sb, t) => SerializeStuctProperty(sb, "Number",      t, o => o.Number,      FormatStruct),
                        //(sb, t) => SerializeStuctProperty(sb, "BoolField1",  t, o => o.BoolField,  FormatBool)

                        (sb, t) => NExpJsonSerializerStringBuilderExtensions.SerializeRefPropertyHandleNull(sb,  t, "ListItems", o => o.ListItems,
                            (sb2, t2) => NExpJsonSerializerStringBuilderExtensions.SerializeRefArrayHandleEmpty(sb2, t2,
                                (sb3, t3) =>
                                    NExpJsonSerializerStringBuilderExtensions.SerializeObject(sb3, t3,
                                        (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeStructProperty(sb4,  t4, "DateTime", o => o.DateTime, NExpJsonSerializerFormatters.SerializeToIso8601WithSecUtc),
                                        (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeRefPropertyHandleNull(sb4,  t4, "RowData", o => o.RowData, NExpJsonSerializerFormatters.SerializeBase64, NExpJsonSerializerStringBuilderExtensions.SerializeNull)
                                     ), 
                                NExpJsonSerializerStringBuilderExtensions.SerializeNull
                              ),
                            NExpJsonSerializerStringBuilderExtensions.SerializeNull
                        ),
                        (sb, t) => NExpJsonSerializerStringBuilderExtensions.SerializeRefPropertyHandleNull(sb, t, "Ints", o => o.Ints,
                            (sb2, t2) => NExpJsonSerializerStringBuilderExtensions.SerializeStructArrayHandleEmpty(sb2, t2,
                                (sb3, t3) => NExpJsonSerializerStringBuilderExtensions.SerializeStruct(sb, t3)
                              ),
                            NExpJsonSerializerStringBuilderExtensions.SerializeNull
                        )
                    )
                );
            return serializer;
        }

        public static Func<StringBuilder, T, bool> ExpressionBuilder<T>(Expression<Func<StringBuilder, T, bool>> expression)
        {
            var f = expression.Compile();
            return f;
        }

        public static Func<StringBuilder, TestClass, bool> BuildSerializerX()
        {
            ParameterExpression parameterExpression1 = Expression.Parameter(typeof(StringBuilder), "sbP");
            ParameterExpression parameterExpression2 = Expression.Parameter(typeof(TestClass), "tP");
            // ISSUE: variable of the null type
            //__Null local1 = null;
            // ISSUE: method reference
            MethodInfo objectFormatterMethodInfo1 = null; // (MethodInfo)MethodBase.GetMethodFromHandle(__methodref(NExpJsonSerializerFormatters.ObjectFormatter));
            Expression[] expressionArray1 = new Expression[3];
            int index1 = 0;
            ParameterExpression parameterExpression3 = parameterExpression1;
            expressionArray1[index1] = (Expression)parameterExpression3;
            int index2 = 1;
            ParameterExpression parameterExpression4 = parameterExpression2;
            expressionArray1[index2] = (Expression)parameterExpression4;
            int index3 = 2;
            Type type1 = typeof(Func<StringBuilder, TestClass, bool>);
            Expression[] expressionArray2 = new Expression[1];
            int index4 = 0;
            ParameterExpression parameterExpression5 = Expression.Parameter(typeof(StringBuilder), "sb");
            ParameterExpression parameterExpression6 = Expression.Parameter(typeof(TestClass), "t");
            // ISSUE: variable of the null type
            //__Null local2 = null;
            // ISSUE: method reference
            MethodInfo serializeRefPropertyMethodInfo = null;// (MethodInfo)MethodBase.GetMethodFromHandle(__methodref(NExpJsonSerializerFormatters.SerializeRefProperty));
            Expression[] expressionArray3 = new Expression[6];
            int index5 = 0;
            ParameterExpression parameterExpression7 = parameterExpression5;
            expressionArray3[index5] = (Expression)parameterExpression7;
            int index6 = 1;
            ConstantExpression constantExpression1 = Expression.Constant((object)"ListItems", typeof(string));
            expressionArray3[index6] = (Expression)constantExpression1;
            int index7 = 2;
            ParameterExpression parameterExpression8 = parameterExpression6;
            expressionArray3[index7] = (Expression)parameterExpression8;
            int index8 = 3;
            ParameterExpression parameterExpression9 = Expression.Parameter(typeof(TestClass), "o");
            // ISSUE: method reference
            MemberExpression getterListItemExpression = null;// Expression.Property((Expression)parameterExpression9, (MethodInfo)MethodBase.GetMethodFromHandle(__methodref(TestClass.get_ListItems)));
            ParameterExpression[] parameterExpressionArray1 = new ParameterExpression[1];
            int index9 = 0;
            ParameterExpression parameterExpression10 = parameterExpression9;
            parameterExpressionArray1[index9] = parameterExpression10;
            Expression<Func<TestClass, List<ListItem>>> expression1 = Expression.Lambda<Func<TestClass, List<ListItem>>>((Expression)getterListItemExpression, parameterExpressionArray1);
            expressionArray3[index8] = (Expression)expression1;
            int index10 = 4;
            ParameterExpression parameterExpression11 = Expression.Parameter(typeof(StringBuilder), "sb2");
            ParameterExpression parameterExpression12 = Expression.Parameter(typeof(List<ListItem>), "t2");
            
            // ISSUE: variable of the null type
            //__Null local3 = null;
            // ISSUE: method reference
            MethodInfo arrayForatterMethodInfo = null; // (MethodInfo)MethodBase.GetMethodFromHandle(__methodref(NExpJsonSerializerFormatters.ArrayFormatter));
            Expression[] expressionArray4 = new Expression[3];
            int index11 = 0;
            ParameterExpression parameterExpression13 = parameterExpression11;
            expressionArray4[index11] = (Expression)parameterExpression13;
            int index12 = 1;
            ParameterExpression parameterExpression14 = parameterExpression12;
            expressionArray4[index12] = (Expression)parameterExpression14;
            //int index13 = 2;
            ParameterExpression parameterExpression15 = Expression.Parameter(typeof(StringBuilder), "sb3");
            ParameterExpression parameterExpression16 = Expression.Parameter(typeof(ListItem), "t3");
            // ISSUE: variable of the null type
            //__Null local4 = null;
            // ISSUE: method reference
            MethodInfo objectFormatterMethodInfo4 = null; // (MethodInfo)MethodBase.GetMethodFromHandle(__methodref(NExpJsonSerializerFormatters.ObjectFormatter));
            Expression[] expressionArray5 = new Expression[3];
            int index14 = 0;
            ParameterExpression parameterExpression17 = parameterExpression15;
            expressionArray5[index14] = (Expression)parameterExpression17;
            int index15 = 1;
            ParameterExpression parameterExpression18 = parameterExpression16;
            expressionArray5[index15] = (Expression)parameterExpression18;
            int index16 = 2;
            Type type2 = typeof(Func<StringBuilder, ListItem, bool>);
            Expression[] expressionArray6 = new Expression[1];
            int index17 = 0;
            ParameterExpression parameterExpression19 = Expression.Parameter(typeof(StringBuilder), "sb4");
            ParameterExpression parameterExpression20 = Expression.Parameter(typeof(ListItem), "t4");
            // ISSUE: variable of the null type
            //__Null local5 = null;
            // ISSUE: method reference
            MethodInfo serializeStuctPropertyMethodInfo = null; // (MethodInfo)MethodBase.GetMethodFromHandle(__methodref(NExpJsonSerializerFormatters.SerializeStuctProperty));
            Expression[] expressionArray7 = new Expression[5];
            int index18 = 0;
            ParameterExpression parameterExpression21 = parameterExpression19;
            expressionArray7[index18] = (Expression)parameterExpression21;
            int index19 = 1;
            ConstantExpression constantExpression2 = Expression.Constant((object)"DateTime", typeof(string));
            expressionArray7[index19] = (Expression)constantExpression2;
            int index20 = 2;
            ParameterExpression parameterExpression22 = parameterExpression20;
            expressionArray7[index20] = (Expression)parameterExpression22;
            int index21 = 3;
            ParameterExpression parameterExpression23 = Expression.Parameter(typeof(ListItem), "o");
            // ISSUE: method reference
            MemberExpression getterDateTimeMemberExpression = null; // Expression.Property((Expression)parameterExpression23, (MethodInfo)MethodBase.GetMethodFromHandle(__methodref(ListItem.get_DateTime)));
            ParameterExpression[] parameterExpressionArray2 = new ParameterExpression[1];
            int index22 = 0;
            ParameterExpression parameterExpression24 = parameterExpression23;
            parameterExpressionArray2[index22] = parameterExpression24;
            Expression<Func<ListItem, DateTime>> expression2 = Expression.Lambda<Func<ListItem, DateTime>>((Expression)getterDateTimeMemberExpression, parameterExpressionArray2);
            expressionArray7[index21] = (Expression)expression2;
            int index23 = 4;
            // ISSUE: method reference
            ConstantExpression serializeToIso8601WithSecUtcExpression = null; // Expression.Constant((object)(MethodInfo)MethodBase.GetMethodFromHandle(__methodref(NExpJsonSerializerFormatters.SerializeToIso8601WithSecUtc)), typeof(MethodInfo));
            // ISSUE: method reference
            MethodInfo createDelegateMethodInfo6 = null; // (MethodInfo)MethodBase.GetMethodFromHandle(__methodref(MethodInfo.CreateDelegate));
            Expression[] expressionArray8 = new Expression[2];
            int index24 = 0;
            ConstantExpression constantExpression4 = Expression.Constant((object)typeof(Func<StringBuilder, DateTime, bool>), typeof(Type));
            expressionArray8[index24] = (Expression)constantExpression4;
            int index25 = 1;
            ConstantExpression constantExpression5 = Expression.Constant((object)null, typeof(object));
            expressionArray8[index25] = (Expression)constantExpression5;
            UnaryExpression unaryExpression1 = Expression.Convert((Expression)Expression.Call((Expression)serializeToIso8601WithSecUtcExpression, createDelegateMethodInfo6, expressionArray8), typeof(Func<StringBuilder, DateTime, bool>));
            expressionArray7[index23] = (Expression)unaryExpression1;
            MethodCallExpression methodCallExpression1 = Expression.Call(serializeStuctPropertyMethodInfo, expressionArray7);
            ParameterExpression[] parameterExpressionArray3 = new ParameterExpression[2];
            int index26 = 0;
            ParameterExpression parameterExpression25 = parameterExpression19;
            parameterExpressionArray3[index26] = parameterExpression25;
            int index27 = 1;
            ParameterExpression parameterExpression26 = parameterExpression20;
            parameterExpressionArray3[index27] = parameterExpression26;
            Expression<Func<StringBuilder, ListItem, bool>> expression3 = Expression.Lambda<Func<StringBuilder, ListItem, bool>>((Expression)methodCallExpression1, parameterExpressionArray3);
            expressionArray6[index17] = (Expression)expression3;
            NewArrayExpression newArrayExpression1 = Expression.NewArrayInit(type2, expressionArray6);
            expressionArray5[index16] = (Expression)newArrayExpression1;

            MethodCallExpression methodCallExpression2 = Expression.Call( objectFormatterMethodInfo4, expressionArray5);
            
            Expression<Func<StringBuilder, ListItem, bool>> expression4 = Expression.Lambda<Func<StringBuilder, ListItem, bool>>((Expression)methodCallExpression2, new[] { parameterExpression15, parameterExpression16 });
            expressionArray4[2] = (Expression)expression4;

            MethodCallExpression arrayForatterCallExpression = Expression.Call(arrayForatterMethodInfo, expressionArray4);

            ParameterExpression[] parameterExpressionArray5 = new ParameterExpression[2];
            ParameterExpression parameterExpression29 = parameterExpression11;
            parameterExpressionArray5[0] = parameterExpression29;
            ParameterExpression parameterExpression30 = parameterExpression12;
            parameterExpressionArray5[1] = parameterExpression30;
            Expression<Func<StringBuilder, List<ListItem>, bool>> expression5 = Expression.Lambda<Func<StringBuilder, List<ListItem>, bool>>(arrayForatterCallExpression, parameterExpressionArray5);


            expressionArray3[index10] = (Expression)expression5;
            int index32 = 5;
            // ISSUE: method reference
            ConstantExpression formatNullExpression6 = null; // Expression.Constant((object)(MethodInfo)MethodBase.GetMethodFromHandle(__methodref(NExpJsonSerializerFormatters.FormatNull)), typeof(MethodInfo));
            // ISSUE: method reference
            MethodInfo createDelegateMethodInfo7 = null;// (MethodInfo)MethodBase.GetMethodFromHandle(__methodref(MethodInfo.CreateDelegate));
            Expression[] expressionArray9 = new Expression[2];
            int index33 = 0;
            ConstantExpression constantExpression7 = Expression.Constant((object)typeof(Func<StringBuilder, bool>), typeof(Type));
            expressionArray9[index33] = (Expression)constantExpression7;
            int index34 = 1;
            ConstantExpression constantExpression8 = Expression.Constant((object)null, typeof(object));
            expressionArray9[index34] = (Expression)constantExpression8;
            UnaryExpression unaryExpression2 = Expression.Convert((Expression)Expression.Call((Expression)formatNullExpression6, createDelegateMethodInfo7, expressionArray9), typeof(Func<StringBuilder, bool>));
            expressionArray3[index32] = (Expression)unaryExpression2;
            MethodCallExpression methodCallExpression4 = Expression.Call( serializeRefPropertyMethodInfo, expressionArray3);
            ParameterExpression[] parameterExpressionArray6 = new ParameterExpression[2];
            int index35 = 0;
            ParameterExpression parameterExpression31 = parameterExpression5;
            parameterExpressionArray6[index35] = parameterExpression31;
            int index36 = 1;
            ParameterExpression parameterExpression32 = parameterExpression6;
            parameterExpressionArray6[index36] = parameterExpression32;
            Expression<Func<StringBuilder, TestClass, bool>> expression6 = Expression.Lambda<Func<StringBuilder, TestClass, bool>>((Expression)methodCallExpression4, parameterExpressionArray6);
            expressionArray2[index4] = (Expression)expression6;
            NewArrayExpression newArrayExpression2 = Expression.NewArrayInit(type1, expressionArray2);
            expressionArray1[index3] = (Expression)newArrayExpression2;
            MethodCallExpression methodCallExpression5 = Expression.Call( objectFormatterMethodInfo1, expressionArray1);
            ParameterExpression[] parameterExpressionArray7 = new ParameterExpression[2];
            int index37 = 0;
            ParameterExpression parameterExpression33 = parameterExpression1;
            parameterExpressionArray7[index37] = parameterExpression33;
            int index38 = 1;
            ParameterExpression parameterExpression34 = parameterExpression2;
            parameterExpressionArray7[index38] = parameterExpression34;
            return NExpJsonSerializerTools.ExpressionBuilder<TestClass>(Expression.Lambda<Func<StringBuilder, TestClass, bool>>((Expression)methodCallExpression5, parameterExpressionArray7));
        }
    }
}
