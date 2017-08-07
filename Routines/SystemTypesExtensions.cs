using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DashboardCode.Routines
{
    public static class SystemTypesExtensions
    {
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
            ParameterExpression eParameterExpression = Expression.Parameter(declaringType, "e");
            var propertyCallExpression = Expression.Property(
                eParameterExpression,
                propertyInfo
                );
            var propertyLambda = Expression.Lambda(propertyCallExpression, new[] { eParameterExpression });
            return propertyLambda;
        }

        public static PropertyInfo GrabDeclaredOrInheritedPoperty(this TypeInfo typeInfo, string propertyName)
        {
            foreach (var p in ListProperties(typeInfo))
                if (p.Name == propertyName)
                    return p;
            throw new Exception($"Property '{propertyName}' not found in type '{typeInfo.Name}'");
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

    }
}