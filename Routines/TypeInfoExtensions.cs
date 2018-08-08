using System;
using System.Reflection;
using System.Collections.Generic;

namespace DashboardCode.Routines
{
    public static class TypeInfoExtensions
    {
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