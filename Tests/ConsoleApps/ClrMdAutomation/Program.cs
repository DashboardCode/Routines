using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace ClrMdAutomation
{
    struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    class Program
    {
        public static bool SerializeAssociativeArray<T>(StringBuilder stringBuilder, T t, params Func<StringBuilder, T, bool>[] propertySerializers)
        {
            var @value = false;
            stringBuilder.Append('{');
            foreach (var propertySerializer in propertySerializers)
            {
                var notEmpty = propertySerializer(stringBuilder, t);
                if (notEmpty)
                {
                    if (!@value)
                        @value = true;
                    stringBuilder.Append(',');
                }
            };
            stringBuilder.Length--;
            if (@value)
                stringBuilder.Append('}');
            return @value;
        }

        public static bool SerializeValueProperty<T, TProp>(StringBuilder stringBuilder, T t, string propertyName,
            Func<T, TProp> getter, Func<StringBuilder, TProp, bool> serializer) where TProp : struct
        {
            stringBuilder.Append('"').Append(propertyName).Append('"').Append(':');
            var value = getter(t);
            var notEmpty = serializer(stringBuilder, value);
            if (!notEmpty)
                stringBuilder.Length -= (propertyName.Length + 3);
            return notEmpty;
        }

        public static bool SerializeValueToString<T>(StringBuilder stringBuilder, T t) where T : struct
        {
            stringBuilder.Append(t);
            return true;
        }

        static void Main(string[] args)
        {
            //var serializerExpression = StaticCompose();
            var serializerExpression = DynamicCompose<Point, int>(p => p.X, p => p.Y, "X", "Y");
            var serializer = serializerExpression.Compile();

            string formatter(Point p)
            {
                var stringBuilder = new StringBuilder();
                serializer(stringBuilder, p);
                var txt = stringBuilder.ToString();
                return txt;
            }
            var point = new Point { X = -1, Y = 1 };
            var json = formatter(point);

            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName("dynamicAssembly"),
                AssemblyBuilderAccess.Save);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("dynamicModule", "dynamic.dll");
            var typeBuilder = moduleBuilder.DefineType("DynamicManager");
            var methodBuilder = typeBuilder.DefineMethod("Serialize", MethodAttributes.Public | MethodAttributes.Static);
            serializerExpression.CompileToMethod(methodBuilder);
            Type type = typeBuilder.CreateType();
            assemblyBuilder.Save("dynamic.dll");

            // imoprtant to know, but hard to explain 
            // create delegate to just created method
            var @delegate = (Func<StringBuilder, Point, bool>)(type.GetMethod("Serialize").CreateDelegate((typeof(Func<StringBuilder, Point, bool>))));
            
            var sb = new StringBuilder();
            var json2 = @delegate(sb, point); // surprise System.TypeAccessException

        }

        public static Expression<Func<StringBuilder, Point, bool>> StaticCompose()
        {
            Expression<Func<StringBuilder, Point, bool>> serializerExp =
                    (sb, t) => SerializeAssociativeArray(sb, t,
                        (sb1, t1) => SerializeValueProperty(sb1, t1, "X", o => o.X, (sb1i, v1) => SerializeValueToString(sb1i, v1)),
                        (sb2, t2) => SerializeValueProperty(sb2, t2, "Y", o => o.Y, (sb1i, v1) => SerializeValueToString(sb1i, v1))
                    );
            return serializerExp;
        }

        public static Expression<Func<StringBuilder, T, bool>> DynamicCompose<T, TProp>(
            Expression<Func<T, TProp>> getter1, 
            Expression<Func<T, TProp>> getter2, 
            string propertyName1, 
            string propertyName2)
        {
            //Expression<Func<StringBuilder, Point, bool>> serializerExp =
            //        (sb, t) => SerializeAssociativeArray(sb, t,
            //            (sb1, t1) => SerializeValueProperty(sb1, t1, "X", o1 => o1.X, SerializeValueToString),
            //            (sb2, t2) => SerializeValueProperty(sb2, t2, "Y", o2 => o2.Y, SerializeValueToString)
            //        );

            var typeOfAssociativeArray = typeof(T);
            var typeOfProperty = typeof(TProp);

            var sbParameterExpression = Expression.Parameter(typeof(StringBuilder), "sb");
            var tParameterExpression = Expression.Parameter(typeOfAssociativeArray, "t");
            var sb1ParameterExpression = Expression.Parameter(typeof(StringBuilder), "sb1");
            var t1ParameterExpression = Expression.Parameter(typeOfAssociativeArray, "t1");
            var sb2ParameterExpression = Expression.Parameter(typeof(StringBuilder), "sb2");
            var t2ParameterExpression = Expression.Parameter(typeOfAssociativeArray, "t2");

            var sb1iParameterExpression = Expression.Parameter(typeof(StringBuilder), "sb1i");
            var v1ParameterExpression = Expression.Parameter(typeOfProperty, "v1");
            var sb2iParameterExpression = Expression.Parameter(typeof(StringBuilder), "sb2i");
            var v2ParameterExpression = Expression.Parameter(typeOfProperty, "v2");

            var o1ParameterExpression = Expression.Parameter(typeOfAssociativeArray, "o1");
            var o2ParameterExpression = Expression.Parameter(typeOfAssociativeArray, "o2");

            var serializeValuePropertyMethodInfoGeneric = typeof(Program)
                .GetTypeInfo().GetDeclaredMethod(nameof(Program.SerializeValueProperty));
            var serializeValuePropertyMethodInfo = serializeValuePropertyMethodInfoGeneric.MakeGenericMethod(typeof(Point), typeof(int));

            var serializeValueToStringMethodInfoGeneric = typeof(Program)
                .GetTypeInfo().GetDeclaredMethod(nameof(Program.SerializeValueToString));
            var serializeValueToStringMethodInfo = serializeValueToStringMethodInfoGeneric.MakeGenericMethod(typeof(int));

            var getterDelegateExpression1 = getter1;

            // alternative (but it doesn't work for CompileToMethod compilation)
            //var getterDelegate1 = getter1.Compile();
            //var getterDelegateExpression1 = Expression.Constant(getterDelegate1, getterDelegate1.GetType());

            var serializeValueCall1 = Expression.Call(serializeValueToStringMethodInfo, new[] { sb1iParameterExpression, v1ParameterExpression });
            var serializeValueLambda1 = Expression.Lambda(serializeValueCall1, new[] { sb1iParameterExpression, v1ParameterExpression });

            var serializeValueDelegateExpression1 = serializeValueLambda1;
            // alternative (but it doesn't work for CompileToMethod compilation)
            //var serializeValueDelegate1 = serializeValueLambda1.Compile();
            //var serializeValueDelegateExpression1 = Expression.Constant(serializeValueDelegate1, serializeValueDelegate1.GetType());


            var callSerializeValueProperty1 = Expression.Call(
                serializeValuePropertyMethodInfo,
                new Expression[] { sb1ParameterExpression, t1ParameterExpression, Expression.Constant(propertyName1, typeof(string))
                    , getterDelegateExpression1, serializeValueDelegateExpression1 });
            var lambda1 = Expression.Lambda(callSerializeValueProperty1, new[] { sb1ParameterExpression, t1ParameterExpression });

            #region property 2
            var getterDelegateExpression2 = getter2;
            // alternative (but it doesn't work for CompileToMethod compilation)
            //var getterDelegate2 = getter2.Compile();
            //var getterDelegateExpression2 = Expression.Constant(getterDelegate2, getterDelegate2.GetType());

            var serializeValueCall2 = Expression.Call(serializeValueToStringMethodInfo, new[] { sb2iParameterExpression, v2ParameterExpression });
            var serializeValueLambda2 = Expression.Lambda(serializeValueCall2, new[] { sb2iParameterExpression, v2ParameterExpression });

            var serializeValueDelegateExpression2 = serializeValueLambda2;
            // alternative (but it doesn't work for CompileToMethod compilation)
            // var serializeValueDelegate2 = serializeValueLambda2.Compile();
            // var serializeValueDelegateExpression2 = Expression.Constant(serializeValueDelegate2, serializeValueDelegate2.GetType());

            var callSerializeValueProperty2 = Expression.Call(
                serializeValuePropertyMethodInfo,
                new Expression[] { sb2ParameterExpression, t2ParameterExpression, Expression.Constant(propertyName2, typeof(string))
                    , getterDelegateExpression2, serializeValueDelegateExpression2 });
            var lambda2 = Expression.Lambda(callSerializeValueProperty2, new[] { sb2ParameterExpression, t2ParameterExpression });
            #endregion

            var serializeAssociativeArrayMethodInfoGeneric = typeof(Program)
                .GetTypeInfo().GetDeclaredMethod(nameof(Program.SerializeAssociativeArray));

            var serializeObjectGenericMethodInfo = serializeAssociativeArrayMethodInfoGeneric.MakeGenericMethod(typeOfAssociativeArray);

            var serializePropertyFuncDelegateType = typeof(Func<,,>).MakeGenericType(typeof(StringBuilder), typeOfAssociativeArray, typeof(bool));

            Expression[] serializeProperties = new[] { lambda1, lambda2 };
            var arrayParameterExpression = Expression.NewArrayInit(serializePropertyFuncDelegateType, serializeProperties);
            var callSerializeAssociativeArray = Expression.Call(
                serializeObjectGenericMethodInfo, 
                new Expression[] { sbParameterExpression, tParameterExpression, arrayParameterExpression });

            var serializeLambda = (Expression<Func<StringBuilder, T, bool>>)Expression.Lambda(callSerializeAssociativeArray, new[] { sbParameterExpression, tParameterExpression });
            return serializeLambda;
            //var @value = ((Expression<Func<StringBuilder, T, bool>>)serializeLambda).Compile();

            //return @value;
        }
    }
}
