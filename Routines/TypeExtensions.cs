using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DashboardCode.Routines
{
    public static class TypeExtensions
    {
        public static bool IsAssociativeArrayType(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return (typeInfo.IsClass && !(typeof(string) == type || typeInfo.IsArray)) || (typeInfo.IsValueType && !typeInfo.IsEnum && !typeInfo.IsPrimitive);
        }

        public static readonly IReadOnlyCollection<Type> DefaultSimpleTextyTypes = new HashSet<Type>
        {
             typeof(DateTime), typeof(DateTime?), typeof(Guid), typeof(Guid?), typeof(TimeSpan),  typeof(TimeSpan?), typeof(DateTimeOffset), typeof(DateTimeOffset?)
        };

        public static readonly IReadOnlyCollection<Type> DefaultSimpleSymbolTypes = new HashSet<Type>
        {
             typeof(Decimal), typeof(Decimal?)
        };

        public static readonly IReadOnlyCollection<Type> PrimitiveTypes = new HashSet<Type>
        {
             typeof(bool),
             typeof(bool?),
             typeof(byte),
             typeof(byte?),
             typeof(char),
             typeof(char?),
             typeof(double),
             typeof(double?),
             typeof(float),
             typeof(float?),
             typeof(int),
             typeof(int?),
             typeof(long),
             typeof(long?),
             typeof(sbyte),
             typeof(sbyte?),
             typeof(short),
             typeof(short?),
             typeof(uint),
             typeof(uint?),
             typeof(ulong),
             typeof(ulong?),
             typeof(ushort),
             typeof(ushort?)
        };

        public static readonly IReadOnlyCollection<Type> SystemTypes = new HashSet<Type>(
            DefaultSimpleTextyTypes
                .Union(DefaultSimpleSymbolTypes)
                .Union(PrimitiveTypes)
                .Union(new[] { typeof(string) }));

        public static LambdaExpression CreatePropertyLambda(this Type declaringType, PropertyInfo propertyInfo)
        {
            ParameterExpression expression = Expression.Parameter(declaringType, "e");
            var propertyCallExpression = Expression.Property(
                expression,
                propertyInfo
                );
            var propertyLambda = Expression.Lambda(propertyCallExpression, new[] { expression });
            return propertyLambda;
        }

        public static LambdaExpression CreateFieldLambda(this Type declaringType, FieldInfo fieldInfo)
        {
            ParameterExpression expression = Expression.Parameter(declaringType, "e");
            var propertyCallExpression = Expression.Field(
                expression,
                fieldInfo
                );
            var fieldLambda = Expression.Lambda(propertyCallExpression, new[] { expression });
            return fieldLambda;
        }

        public static LambdaExpression CreatePropertyLambda(this Type declaringType, string propertyName)
        {
            var propertyInfo = declaringType.GetProperty(propertyName);
            var propertyLambda = CreatePropertyLambda(declaringType, propertyInfo);
            return propertyLambda;
        }

        public static LambdaExpression CreateFieldLambda(this Type declaringType, string fieldName)
        {
            var fieldInfo = declaringType.GetField(fieldName);
            var fieldLambda = CreateFieldLambda(declaringType, fieldInfo);
            return fieldLambda;
        }

        public static bool IsAnonymousType(this Type type)
        {
            var @value = type.IsClass && type.IsPublic == false && type.IsSealed && type.Namespace == null && type.Name.Contains("AnonymousType")
                 && Attribute.IsDefined(type, typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false);
            return @value;
        }

        public static bool IsTuple(this Type type)
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
            }
            return @value;
        }

        public static bool IsValueTuple(this Type type)
        {
            var @value = false;
            if (type.IsGenericType)
            {
                var genType = type.GetGenericTypeDefinition();
                if (genType == typeof(ValueTuple<>)
                    || genType == typeof(ValueTuple<,>)
                    || genType == typeof(ValueTuple<,,>)
                    || genType == typeof(ValueTuple<,,,>)
                    || genType == typeof(ValueTuple<,,,,>)
                    || genType == typeof(ValueTuple<,,,,,>)
                    || genType == typeof(ValueTuple<,,,,,,>)
                    || genType == typeof(ValueTuple<,,,,,,,>)
                    || genType == typeof(ValueTuple<,,,,,,,>))
                    @value = true;
            }
            return @value;
        }

        /// TODO: what is most effective IsSystemTypePredefined or IsSystemType
        public static bool IsSystemTypePredefined(Type type)
        {
            return SystemTypes.Contains(type);
        }
        /// <summary>
        /// The same result as for IsSystemType but different way
        /// </summary>
        /// <param name="memberType"></param>
        /// <returns></returns>
        public static bool IsSystemType(this Type memberType)
        {
            if (memberType.GetTypeInfo().IsPrimitive
                || memberType == typeof(string)
                || DefaultSimpleTextyTypes.Contains(memberType)
                || DefaultSimpleSymbolTypes.Contains(memberType))
            {
                return true;
            }
            else
            {
                var baseNullableType = Nullable.GetUnderlyingType(memberType);
                if (baseNullableType != null && baseNullableType.GetTypeInfo().IsPrimitive)
                    return true;
            }
            return false;
        }
    }
}
