using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Vse.Routines.Json
{
    public class ListItem
    {
        public byte[] RowData { get; set; } 
        public DateTime DateTime { get; set; }
    }

    public class Test
    {
        public string TextField1 { get; set; }
        public Test   TestField  { get; set; }
        public bool   BoolField1 { get; set; }
        public List<ListItem> ListItems { get; set; }
        public int  Number         { get; set; }
        public int? NumberNullable { get; set; }
    }

    public static class NExpJsonSerializerTools
    {
        static NExpJsonSerializerTools(){
            
        }
        public static Func<StringBuilder, Test, bool> BuildSerializer2()
        {
            var formatterAsParameter = Expression.Constant(
                  typeof(NExpJsonSerializerTools).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerTools.SerializeString))
                , typeof(MethodInfo));

            var sb = Expression.Parameter(typeof(StringBuilder), "sb");
            var t  = Expression.Parameter(typeof(Test), "t");
            var o  = Expression.Parameter(typeof(Test), "o");
            
            var propertyExpression = Expression.Property(o, typeof(Test).GetTypeInfo().GetDeclaredProperty(nameof(Test.TextField1)));
            var propertyLambda     = Expression.Lambda<Func<Test, string>>(propertyExpression, new[] {o});

            var createDelegateMethod = 
                typeof(MethodInfo).GetTypeInfo().GetDeclaredMethods(nameof(MethodInfo.CreateDelegate)).First(m=>m.GetParameters().Length==2);

            var expressionConvert = Expression.Convert(Expression.Call(formatterAsParameter, createDelegateMethod, new Expression[] {
                Expression.Constant(typeof(Func<StringBuilder, string, bool>), typeof(Type)),
                Expression.Constant(null, typeof(object))
            }), typeof(Func<StringBuilder, string, bool>));

            var serializePropertyMethod = typeof(NExpJsonSerializerTools).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerTools.SerializeProperty));
            var serializePropertyGeneric = serializePropertyMethod.MakeGenericMethod(typeof(Test), typeof(string));

            var serializeProperty = Expression.Call(serializePropertyGeneric,
                    new Expression[] { sb, Expression.Constant("TextField1", typeof(string)), t, propertyLambda, expressionConvert }
                    );

            var serializePropertyLambda = Expression.Lambda<Func<StringBuilder, Test, bool>>(serializeProperty, new[] {sb,t});
            // ---------------------
            var sbP = Expression.Parameter(typeof(StringBuilder), "sbP");
            var tP  = Expression.Parameter(typeof(Test), "tP");

            var objectFormatterMethod  = typeof(NExpJsonSerializerTools).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerTools.ObjectFormatter));
            var objectFormatterGeneric = objectFormatterMethod.MakeGenericMethod(typeof(Test));

            MethodCallExpression objectFormatterExpression = Expression.Call(
                objectFormatterGeneric, 
                new Expression[] { sbP, tP, Expression.NewArrayInit(typeof(Func<StringBuilder, Test, bool>), new[] { serializePropertyLambda } )}
            );

            var func = NExpJsonSerializerTools.ExpressionBuilder(Expression.Lambda<Func<StringBuilder, Test, bool>>(objectFormatterExpression, new[] {sbP,tP}));
            return func;
        }

        public static Func<StringBuilder, T, bool> BuildSerializer3<T>(/*SerializerBaseNode node*/)
        {
            Func<StringBuilder, T, bool> @value = null;
            //if (!node.IsPrimitive)
            {
                var formatter1 = typeof(NExpJsonSerializerTools).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerTools.WriteBool));
                var unaryExpression1 = CreateOneStepConvert<bool>(formatter1);
                var serializePropertyLambda1 = CreateSerializePropertyLambda<T, bool>("BoolField1", "BoolField1", unaryExpression1);

                var formatter2 = typeof(NExpJsonSerializerTools).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerTools.SerializeString));
                var unaryExpression2 = CreateOneStepConvert<string>(formatter2);
                var serializePropertyLambda2 = CreateSerializePropertyLambda<T, string>("TextField1", "TextField1", unaryExpression2);

                var formatter3 = typeof(NExpJsonSerializerTools).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerTools.WriteNumber));
                var unaryExpression3 = CreateOneStepConvert<int>(formatter3);
                var serializePropertyLambda3 = CreateSerializePropertyLambda<T, int>("Number", "Number", unaryExpression3);

                var objectFormatterLambda = CreateObjectFormatterLambda<T>(new[] { /*serializePropertyLambda1, serializePropertyLambda2,*/ serializePropertyLambda3 });

                @value = objectFormatterLambda.Compile();
            }
            //else
            //    throw new NotImplementedException("");
            return @value;
        }

        public static Func<StringBuilder, T, bool> BuildSerializerReflection<T>(/*SerializerBaseNode node*/)
        {
            Func<StringBuilder, T, bool> @value = null;
            //if (!node.IsPrimitive)
            {
                var formatter1 = typeof(NExpJsonSerializerTools).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerTools.WriteBool));
                var unaryExpression1 = CreateOneStepConvertReflection(typeof(bool), formatter1);
                var serializePropertyLambda1 = CreateSerializePropertyLambdaReflection(typeof(T), typeof(bool), "BoolField1", "BoolField1", unaryExpression1);

                var formatter2 = typeof(NExpJsonSerializerTools).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerTools.SerializeString));
                var unaryExpression2 = CreateOneStepConvertReflection(typeof(string), formatter2);
                var serializePropertyLambda2 = CreateSerializePropertyLambdaReflection(typeof(T), typeof(string), "TextField1", "TextField1", unaryExpression2);

                var formatter3 = typeof(NExpJsonSerializerTools).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerTools.WriteNumber));
                var unaryExpression3 = CreateOneStepConvertReflection(typeof(int), formatter3);
                var serializePropertyLambda3 = CreateSerializePropertyLambdaReflection(typeof(T), typeof(int), "Number", "Number", unaryExpression3);

                var objectFormatterLambda = CreateObjectFormatterLambdaReflection(typeof(T), new[] { serializePropertyLambda1, serializePropertyLambda2, serializePropertyLambda3 });

                @value = ((Expression<Func<StringBuilder, T, bool>>)objectFormatterLambda).Compile();
            }
            //else
            //    throw new NotImplementedException("");
            return @value;
        }

        public static Func<StringBuilder, Test, bool> BuildSerializerX()
        {
            ParameterExpression parameterExpression1 = Expression.Parameter(typeof(StringBuilder), "sbP");
            ParameterExpression parameterExpression2 = Expression.Parameter(typeof(Test), "tP");
            // ISSUE: variable of the null type
            //__Null local1 = null;
            // ISSUE: method reference
            var objectFormatterMethod = typeof(NExpJsonSerializerTools).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerTools.ObjectFormatter));
            //MethodInfo method1 = (MethodInfo)MethodBase.GetMethodFromHandle(__methodref(NExpJsonSerializerTools.ObjectFormatter));
            Expression[] expressionArray1 = new Expression[3];
            int index1 = 0;
            ParameterExpression parameterExpression3 = parameterExpression1;
            expressionArray1[index1] = (Expression)parameterExpression3;
            int index2 = 1;
            ParameterExpression parameterExpression4 = parameterExpression2;
            expressionArray1[index2] = (Expression)parameterExpression4;
            int index3 = 2;
            Type type = typeof(Func<StringBuilder, Test, bool>);
            Expression[] expressionArray2 = new Expression[1];
            int index4 = 0;
            ParameterExpression parameterExpression5 = Expression.Parameter(typeof(StringBuilder), "sb");
            ParameterExpression parameterExpression6 = Expression.Parameter(typeof(Test), "t");
            // ISSUE: variable of the null type
            //__Null local2 = null;
            // ISSUE: method reference
            var serializePropertyMethod = typeof(NExpJsonSerializerTools).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerTools.SerializeProperty));
            //MethodInfo method2 = (MethodInfo)MethodBase.GetMethodFromHandle(__methodref(NExpJsonSerializerTools.SerializeProperty));
            Expression[] expressionArray3 = new Expression[5];
            int index5 = 0;
            ParameterExpression parameterExpression7 = parameterExpression5;
            expressionArray3[index5] = (Expression)parameterExpression7;
            int index6 = 1;
            ConstantExpression constantExpression1 = Expression.Constant((object)"Number", typeof(string));
            expressionArray3[index6] = (Expression)constantExpression1;
            int index7 = 2;
            ParameterExpression parameterExpression8 = parameterExpression6;
            expressionArray3[index7] = (Expression)parameterExpression8;
            int index8 = 3;
            ParameterExpression parameterExpression9 = Expression.Parameter(typeof(Test), "o");
            // ISSUE: method reference
            var propertyExpression = Expression.Property(parameterExpression9, typeof(Test).GetTypeInfo().GetDeclaredProperty("Number"));
            //MemberExpression memberExpression = Expression.Property((Expression)parameterExpression9, (MethodInfo)MethodBase.GetMethodFromHandle(__methodref(Test.get_Number)));
            ParameterExpression[] parameterExpressionArray1 = new ParameterExpression[1];
            int index9 = 0;
            ParameterExpression parameterExpression10 = parameterExpression9;
            parameterExpressionArray1[index9] = parameterExpression10;
            Expression<Func<Test, int>> expression1 = Expression.Lambda<Func<Test, int>>(propertyExpression, parameterExpressionArray1);
            expressionArray3[index8] = (Expression)expression1;
            int index10 = 4;
            // ISSUE: method reference

            var mi = typeof(NExpJsonSerializerTools).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerTools.WriteNumber));
            ConstantExpression constantExpression2 = Expression.Constant((object)mi, typeof(MethodInfo));
            // ISSUE: method reference
            //MethodInfo method3 = (MethodInfo)MethodBase.GetMethodFromHandle(__methodref(MethodInfo.CreateDelegate));
            var createDelegateMethod =
                typeof(MethodInfo).GetTypeInfo().GetDeclaredMethods(nameof(MethodInfo.CreateDelegate)).First(m => m.GetParameters().Length == 2);

            Expression[] expressionArray4 = new Expression[2];
            ConstantExpression constantExpression3 = Expression.Constant((object)typeof(Func<StringBuilder, int, bool>), typeof(Type));
            expressionArray4[0] = (Expression)constantExpression3;
            ConstantExpression constantExpression4 = Expression.Constant((object)null, typeof(object));
            expressionArray4[1] = (Expression)constantExpression4;
            UnaryExpression unaryExpression = Expression.Convert((Expression)Expression.Call((Expression)constantExpression2, createDelegateMethod, expressionArray4), typeof(Func<StringBuilder, int, bool>));

            expressionArray3[index10] = (Expression)unaryExpression;

            var serializePropertyGeneric = serializePropertyMethod.MakeGenericMethod(typeof(Test), typeof(int));
            var local2 = serializePropertyGeneric;
            MethodCallExpression methodCallExpression1 = Expression.Call(serializePropertyGeneric, expressionArray3);
            ParameterExpression[] parameterExpressionArray2 = new ParameterExpression[2];
            int index13 = 0;
            ParameterExpression parameterExpression11 = parameterExpression5;
            parameterExpressionArray2[index13] = parameterExpression11;
            int index14 = 1;
            ParameterExpression parameterExpression12 = parameterExpression6;
            parameterExpressionArray2[index14] = parameterExpression12;
            Expression<Func<StringBuilder, Test, bool>> expression2 = Expression.Lambda<Func<StringBuilder, Test, bool>>((Expression)methodCallExpression1, parameterExpressionArray2);
            expressionArray2[index4] = (Expression)expression2;
            NewArrayExpression newArrayExpression = Expression.NewArrayInit(type, expressionArray2);
            expressionArray1[index3] = (Expression)newArrayExpression;

            //var objectFormatterMethod = typeof(NExpJsonSerializerTools).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerTools.ObjectFormatter));
            var objectFormatterGeneric = objectFormatterMethod.MakeGenericMethod(typeof(Test));

            MethodCallExpression methodCallExpression2 = Expression.Call(/*(Expression)*/objectFormatterGeneric,  expressionArray1);

            ParameterExpression[] parameterExpressionArray3 = new ParameterExpression[2];
            int index15 = 0;
            ParameterExpression parameterExpression13 = parameterExpression1;
            parameterExpressionArray3[index15] = parameterExpression13;
            int index16 = 1;
            ParameterExpression parameterExpression14 = parameterExpression2;
            parameterExpressionArray3[index16] = parameterExpression14;
            return NExpJsonSerializerTools.ExpressionBuilder<Test>(Expression.Lambda<Func<StringBuilder, Test, bool>>((Expression)methodCallExpression2, parameterExpressionArray3));
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

        private static UnaryExpression CreateOneStepConvertReflection(Type propertyType, MethodInfo methodInfo)
        {
            MethodInfo tmp = (methodInfo.IsGenericMethod) ? methodInfo.MakeGenericMethod(propertyType) : methodInfo;

            var formatterAsParameter = Expression.Constant(
                      tmp
                    , typeof(MethodInfo));

            var createDelegateMethod =
                typeof(MethodInfo).GetTypeInfo().GetDeclaredMethods(nameof(MethodInfo.CreateDelegate)).First(m => m.GetParameters().Length == 2);

            var formatterDelegateType = typeof(Func<,,>).MakeGenericType(typeof(StringBuilder), propertyType, typeof(bool));

            var expressionConvert = Expression.Convert(Expression.Call(formatterAsParameter, createDelegateMethod, new Expression[] {
                Expression.Constant(formatterDelegateType, typeof(Type)),
                Expression.Constant(null, typeof(object))
            }), formatterDelegateType);
            return expressionConvert;
        }

        private static Expression<Func<StringBuilder, T, bool>> CreateSerializePropertyLambda<T,TProp>(string serializationName, string propertyName, UnaryExpression unaryExpression)
        {
            var sb = Expression.Parameter(typeof(StringBuilder), "sb");
            var t = Expression.Parameter(typeof(T), "t");
            var o = Expression.Parameter(typeof(T), "o");
            var propertyExpression = Expression.Property(o, typeof(T).GetTypeInfo().GetDeclaredProperty(propertyName));
            var propertyLambda = Expression.Lambda<Func<T, TProp>>(propertyExpression, new[] { o });

            var serializePropertyMethod = typeof(NExpJsonSerializerTools).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerTools.SerializeProperty));
            var serializePropertyGeneric = serializePropertyMethod.MakeGenericMethod(typeof(T), typeof(TProp));
            var expr = Expression.Constant(serializationName, typeof(string));

            var serializeProperty = Expression.Call(serializePropertyGeneric,
                    new Expression[] { sb, expr, t, propertyLambda, unaryExpression }
            );
            var serializePropertyLambda = Expression.Lambda<Func<StringBuilder, T, bool>>(serializeProperty, new[] { sb, t });
            return serializePropertyLambda;
        }

        private static Expression CreateSerializePropertyLambdaReflection(Type entityType, Type propertyType, string serializationName, string propertyName, UnaryExpression unaryExpression)
        {
            var sb = Expression.Parameter(typeof(StringBuilder), "sb");
            var t = Expression.Parameter(entityType, "t");
            var o = Expression.Parameter(entityType, "o");
            var propertyExpression = Expression.Property(o, entityType.GetTypeInfo().GetDeclaredProperty(propertyName));
            var propertyLambda = Expression.Lambda(propertyExpression, new[] { o });

            var serializePropertyMethod = typeof(NExpJsonSerializerTools).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerTools.SerializeProperty));
            var serializePropertyGeneric = serializePropertyMethod.MakeGenericMethod(entityType, propertyType);
            var expr = Expression.Constant(serializationName, typeof(string));

            var serializeProperty = Expression.Call(serializePropertyGeneric,
                    new Expression[] { sb, expr, t, propertyLambda, unaryExpression }
            );
            var serializePropertyLambda = Expression.Lambda(serializeProperty, new[] { sb, t });
            return serializePropertyLambda;
        }

        private static Expression<Func<StringBuilder, T, bool>> CreateObjectFormatterLambda<T>(Expression[] serializeProperties)
        {
            var sbP = Expression.Parameter(typeof(StringBuilder), "sbP");
            var tP = Expression.Parameter(typeof(T), "tP");
            var objectFormatterMethod = typeof(NExpJsonSerializerTools).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerTools.ObjectFormatter));
            var objectFormatterGeneric = objectFormatterMethod.MakeGenericMethod(typeof(T));

            MethodCallExpression objectFormatterExpression = Expression.Call(
                objectFormatterGeneric,
                new Expression[] { sbP, tP, Expression.NewArrayInit(typeof(Func<StringBuilder, T, bool>), serializeProperties) }
            );
            var objectFormatterLambda = Expression.Lambda<Func<StringBuilder, T, bool>>(objectFormatterExpression, new[] {sbP,tP});
            return objectFormatterLambda;
        }

        private static Expression CreateObjectFormatterLambdaReflection(Type objectType, /*Type propertySerializerDelegateType,*/ Expression[] serializeProperties)
        {
            var sbP = Expression.Parameter(typeof(StringBuilder), "sbP");
            var tP = Expression.Parameter(objectType, "tP");
            var objectFormatterMethod = typeof(NExpJsonSerializerTools).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerTools.ObjectFormatter));
            var objectFormatterGeneric = objectFormatterMethod.MakeGenericMethod(objectType);

            var propertySerializerDelegateType = typeof(Func<,,>).MakeGenericType(typeof(StringBuilder), objectType, typeof(bool));
            MethodCallExpression objectFormatterExpression = Expression.Call(
                objectFormatterGeneric,
                new Expression[] { sbP, tP, Expression.NewArrayInit(propertySerializerDelegateType, serializeProperties) }
            );
            var objectFormatterLambda = Expression.Lambda(objectFormatterExpression, new[] { sbP, tP });
            return objectFormatterLambda;
        }

        public static Func<StringBuilder, Test, bool> BuildSerializer()
        {
            var serializer = ExpressionBuilder<Test>(
                    (sbP, tP) => ObjectFormatter(sbP, tP,
                        //(sb, t) => SerializeProperty(sb, "TextField1", t, o => o.TextField1, SerializeString),
                        //(sb, t) => SerializeProperty(sb, "TestField", t, o => o.TestField,
                        //    (sb2, t2) =>
                        //        ObjectFormatter(sb2, t2,
                        //           (sb3, t3) => SerializeProperty(sb3, "TextField1", t3, o => o.TextField1, SerializeString),
                        //           (sb3, t3) => SerializeProperty(sb3, "BoolField1", t3, o => o.BoolField1, WriteBool),
                        //           (sb3, t3) => SerializeProperty(sb3, "TestField",  t3, o => o.TestField,
                        //                (sb4, t4) => ObjectFormatter(sb4, t4,
                        //                   (sb5, t5) => SerializeProperty(sb5, "TextField1", t5, o => o.TextField1, SerializeString)
                        //                )
                        //           )
                        //        )
                        //),
                        //(sb, t) => SerializeProperty(sb, "BoolField1", t, o => o.BoolField1, WriteBool),
                        (sb, t) => SerializeProperty(sb, "Number", t, o => o.Number, WriteNumber)//,
                        //(sb, t) => SerializeProperty(sb, "ListItems", t, o => o.ListItems,
                        //    (sb2, t2) => ArrayFormatter(sb2, t2,
                        //        (sb3, t3) =>
                        //            ObjectFormatter(sb3, t3,
                        //                (sb4, t4) => SerializeProperty(sb4, "DateTime", t4, o => o.DateTime, NExpJsonSerializerFormatters.SerializeToIso8601WithSecUtc ),
                        //                (sb4, t4) => SerializeProperty(sb4, "RowData",  t4, o => o.RowData, (sb5,t5)=>SerializeNullHandled(sb5, t5, NExpJsonSerializerFormatters.SerializeBase64))
                        //             )
                        //      )
                        //)
                    )
                );
            return serializer;
        }

        
        public static Func<StringBuilder, T, bool> ExpressionBuilder<T>(Expression<Func<StringBuilder, T, bool>> expression)
        {
            var f = expression.Compile();
            return f;
        }

        public static bool ObjectFormatter<T>(StringBuilder stringBuilder, T t, params Func<StringBuilder, T, bool>[] funcs)
        {
            stringBuilder.Append('{');
            var val = false;
            foreach (var f in funcs)
            {
                var v = f(stringBuilder, t);
                if (v)
                {
                    if (!val)
                        val = true;
                    stringBuilder.Append(',');
                }
            };
            stringBuilder.Length--;
            if (val)
                stringBuilder.Append('}');
            return val;
        }

        public static bool ArrayFormatter<T>(StringBuilder stringBuilder, IEnumerable<T> tarray, Func<StringBuilder, T, bool> func)
        {
            stringBuilder.Append('[');
            var val = false;
            foreach (var t in tarray)
            {
                var v = func(stringBuilder, t);
                if (v)
                {
                    if (!val)
                        val = true;
                    stringBuilder.Append(',');
                }
            };
            stringBuilder.Length--;
            if (val)
                stringBuilder.Append(']');
            return val;
        }

        public static bool SerializeProperty<T,TProp>(StringBuilder stringBuilder, string propertyName, T t,
            Func<T, TProp> getter, Func<StringBuilder, TProp, bool> formatter)
        {
            stringBuilder.Append("\"").Append(propertyName).Append("\"").Append(":");
            var value = getter(t);
            var currentNotEmpty = formatter(stringBuilder, value); 
            if (!currentNotEmpty)
                stringBuilder.Length -= (propertyName.Length + 3);
            return currentNotEmpty;
        }

        public static bool WriteString(StringBuilder stringBuilder, string text)
        {
            var val = false;
            if (text == null)
                val = WriteNull(stringBuilder);
            else
                stringBuilder.Append('\"').Append(EscapeJson(text)).Append('\"');
            return val;
        }

        public static bool SerializeString(StringBuilder stringBuilder, string text)
        {
            if (text != null)
            {
                stringBuilder.Append('\"').Append(EscapeJson(text)).Append('\"');
                return true;
            }
            else
                return true;
        }

        public static bool WriteBool(StringBuilder stringBuilder, bool b)
        {
            stringBuilder.Append(b ? "true" : "false");
            return true;
        }

        public static bool SerializeBool(StringBuilder stringBuilder, bool? b)
        {
            var val = false;
            if (b != null)
                val = WriteBool(stringBuilder, b.Value);
            return val;
        }

        public static bool WriteNBool(StringBuilder stringBuilder, bool? b)
        {
            var val = false;
            if (b == null)
                val = WriteNull(stringBuilder);
            else
                val = WriteBool(stringBuilder, b.Value);
            return val;
        }

        public static bool WriteNumber<T>(StringBuilder stringBuilder, T b)
        {
            stringBuilder.Append(b.ToString());
            return true;
        }

        //public static bool WriteNumber(StringBuilder stringBuilder, int b)
        //{
        //    stringBuilder.Append(b.ToString());
        //    return true;
        //}
        public static bool SerializeNumberHandleNulls<T>(StringBuilder stringBuilder, T? b) where T : struct
        {
            if (!b.HasValue)

            stringBuilder.Append(b.ToString());
            return true;
        }

        public static bool SerializeNumberNotHandleNulls<T>(StringBuilder stringBuilder, Nullable<T> b) where T : struct
        {
            if (!b.HasValue)

                stringBuilder.Append(b.ToString());
            return true;
        }

        private static bool WriteNull(StringBuilder stringBuilder)
        {
            stringBuilder.Append("null");
            return true;
        }

        //private static bool HandleNull<TProp>(StringBuilder stringBuilder, Func<StringBuilder, TProp, bool> formatter)
        //{
        //    var val = false;
        //    if (text == null)
        //        val = WriteNull(stringBuilder);
        //    else
        //        stringBuilder.Append('\"').Append(EscapeJson(text)).Append('\"');
        //    return val;
        //    //stringBuilder.Append("null");
        //    //return true;
        //}

        private static string EscapeJson(string text)
        {
            var stringBuilder = new StringBuilder(text.Length + (text.Length / 8));
            foreach (char c in text)
            {
                switch (c)
                {
                    case '\\':
                        stringBuilder.Append("\\\\");
                        break;
                    case '\"':
                        stringBuilder.Append("\\\"");
                        break;
                    case '\n':
                        stringBuilder.Append("\\n");
                        break;
                    case '\r':
                        stringBuilder.Append("\\r");
                        break;
                    case '\t':
                        stringBuilder.Append("\\t");
                        break;
                    case '\b':
                        stringBuilder.Append("\\b");
                        break;
                    case '\f':
                        stringBuilder.Append("\\f");
                        break;
                    default:
                        stringBuilder.Append(c);
                        break;
                }
            }
            return stringBuilder.ToString();
        }

        private static bool SerializeNullHandled<T>(StringBuilder sb, T t, Func<StringBuilder, T, bool> nullHandler)
        {
            var value = true;
            if (t == null)
                sb.Append("null");
            else
                value = nullHandler(sb, t);
            return value;
        }

        private static bool SerializeNullNonHandled<T>(StringBuilder sb, T t, Func<StringBuilder, T, bool> nullHandler)
        {
            var value = false;
            if (t != null)
                value = nullHandler(sb, t);
            return value;
        }
    }
}
