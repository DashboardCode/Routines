using System;

namespace DashboardCode.Routines
{
    public static class TypeExtensions
    {
        public static bool IsAnonymousType(Type type)
        {
            var @value = type.IsClass && type.IsPublic == false && type.IsSealed && type.Namespace == null && type.Name.Contains("AnonymousType")
                 && Attribute.IsDefined(type, typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false);
            return @value;
        }

        public static bool IsTupleOrValueTuple(Type type)
        {
            var @value = false;
            if (type.IsGenericType)
            {
                var genType = type.GetGenericTypeDefinition();
                if (genType == typeof(Tuple<>)
                    || genType == typeof(Tuple<,>)
                    || genType == typeof(Tuple<,,>)
                    || genType == typeof(Tuple<,,,>)
                    || genType == typeof(Tuple<,,,,>)
                    || genType == typeof(Tuple<,,,,,>)
                    || genType == typeof(Tuple<,,,,,,>)
                    || genType == typeof(Tuple<,,,,,,,>)
                    || genType == typeof(Tuple<,,,,,,,>))
                    @value = true;
                // TODO: supportValueType means support fields (not properties) 
                //else if (genType == typeof(ValueTuple<>)
                //    || genType == typeof(ValueTuple<,>)
                //    || genType == typeof(ValueTuple<,,>)
                //    || genType == typeof(ValueTuple<,,,>)
                //    || genType == typeof(ValueTuple<,,,,>)
                //    || genType == typeof(ValueTuple<,,,,,>)
                //    || genType == typeof(ValueTuple<,,,,,,>)
                //    || genType == typeof(ValueTuple<,,,,,,,>)
                //    || genType == typeof(ValueTuple<,,,,,,,>))
                //    @value = true;
            }
            return @value;
        }
    }
}
