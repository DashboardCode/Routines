using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DashboardCode.Routines
{
    public static class SystemTypesExtensions
    {
        public static bool IsAssociativeArrayType(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return (typeInfo.IsClass && !(typeof(string) == type || typeInfo.IsArray)) || (typeInfo.IsValueType && !typeInfo.IsEnum && !typeInfo.IsPrimitive);
        }

        public static readonly IReadOnlyCollection<Type> DefaultSimpleTextTypes = new HashSet<Type>
        {
             typeof(DateTime), typeof(Guid), typeof(TimeSpan), typeof(DateTime?), typeof(Guid?),  typeof(TimeSpan?), typeof(DateTimeOffset), typeof(DateTimeOffset?)
        };

        public static readonly IReadOnlyCollection<Type> DefaultSimpleSymbolTypes = new HashSet<Type>
        {
             typeof(Decimal), typeof(Decimal?)
        };

        public static readonly IReadOnlyCollection<Type> SystemTypes = new HashSet<Type>
        {
                typeof(bool),
                typeof(bool?),
                typeof(byte),
                typeof(byte?),
                typeof(char),
                typeof(char?),
                typeof(decimal),
                typeof(decimal?),
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
                typeof(ushort?),
                typeof(string),
                typeof(DateTime),
                typeof(DateTime?),
                typeof(DateTimeOffset),
                typeof(DateTimeOffset?),
                typeof(Guid),
                typeof(Guid?),
                typeof(TimeSpan),
                typeof(TimeSpan?)
        };

        public static bool Contains(Type type)
        {
            return SystemTypes.Contains(type);
        }

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


        public static PropertyInfo GrabDeclaredOrInheritedPoperty(this TypeInfo typeInfo, string propertyName)
        {
            foreach (var p in ListProperties(typeInfo))
                if (p.Name == propertyName)
                    return p;
            throw new Exception($"Property '{propertyName}' not found in type '{typeInfo.Name}'");
        }

        public static FieldInfo GrabDeclaredOrInheritedField(this TypeInfo typeInfo, string fieldName)
        {
            foreach (var p in ListFields(typeInfo))
                if (p.Name == fieldName)
                    return p;
            throw new Exception($"Field '{fieldName}' not found in type '{typeInfo.Name}'");
        }

        public static IEnumerable<PropertyInfo> ListProperties(this TypeInfo typeInfo)
        {
            while (typeInfo != null)
            {
                foreach (var p in typeInfo.DeclaredProperties)
                    yield return p;
                typeInfo = typeInfo.BaseType?.GetTypeInfo();
            }
        }

        public static IEnumerable<FieldInfo> ListFields(this TypeInfo typeInfo)
        {
            while (typeInfo != null)
            {
                foreach (var p in typeInfo.DeclaredFields)
                    yield return p;
                typeInfo = typeInfo.BaseType?.GetTypeInfo();
            }
        }
    }
}