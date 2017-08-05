using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DashboardCode.Routines
{
    public static class MemberExpressionExtensions
    {
        public static string GetMemberName<T1, T2>(Expression<Func<T1, T2>> expression)
        {
            var memberExpression = (MemberExpression)expression.Body;
            return memberExpression.Member.Name;
        }

        public static object GetMemberValue(this MemberExpression memberExpression, object entity)
        {
            var popertyName = memberExpression.Member.Name;
            var type = entity.GetType();
            var typeInfo = type.GetTypeInfo();
            var propertyInfo = typeInfo.GetDeclaredProperty(popertyName);
            Debug.Assert(propertyInfo!=null, "propertyInfo is null");
            Debug.Assert(propertyInfo.CanRead && propertyInfo.GetIndexParameters().Length == 0, "propertyInfo can't be read");
            var value = propertyInfo.GetValue(entity);
            return value;
        }

        public static object GetMemberValueCompiled(this MemberExpression memberExpression, object entity)
        {
            UnaryExpression objectMember = Expression.Convert(memberExpression, typeof(object));
            Expression<Func<object, object>> getterExpression = Expression.Lambda<Func<object,object>>(objectMember);
            var getter = getterExpression.Compile();
            return getter(entity);
        }

        public static Tuple<object, object> GetMemberValues(
            this MemberExpression memberExpression,
            object entity1,
            object entity2)
        {
            var popertyName = memberExpression.Member.Name;
            var type = entity1.GetType();
            var typeInfo = type.GetTypeInfo();
            var propertyInfo = typeInfo.GetDeclaredProperty(popertyName);
            Debug.Assert(propertyInfo != null, "propertyInfo is null");
            Debug.Assert(propertyInfo.CanRead && propertyInfo.GetIndexParameters().Length == 0, "propertyInfo can't be read");
            var value1 = propertyInfo.GetValue(entity1);
            var value2 = propertyInfo.GetValue(entity2);
            return new Tuple<object, object>(value1, value2);
        }

        public static Type GetMemberType(this MemberExpression memberExpression)
        {
            var type = memberExpression.Type;
            return type;
        }

        private static void SetValue(this MemberExpression memberExpression, object entity, object propertyValue)
        {
            var type = entity.GetType();
            var popertyName = memberExpression.Member.Name;
            var propertyInfo = type.GetTypeInfo().GetDeclaredProperty(popertyName);
            Debug.Assert(propertyInfo.CanWrite && propertyInfo.GetIndexParameters().Length == 0);
            propertyInfo.SetValue(entity, propertyValue);
        }
        
        public static Tuple<object, object> CopyMemberValue(
            this MemberExpression memberExpression,
            object source,
            object destination)
        {
            object copiedValue = default(object);
            var sourceValue = memberExpression.GetMemberValue(source);
            if (sourceValue != null)
            {
                var type = sourceValue.GetType();
                var typeInfo = type.GetTypeInfo();
                if (!typeInfo.IsClass || sourceValue is string)
                {
                    copiedValue = sourceValue;
                    SetValue(memberExpression, destination, copiedValue);
                }
                else
                {
                    var destinationValue = memberExpression.GetMemberValue(destination);
                    if (destinationValue == null)
                    {
                        if (type.IsArray)
                        {
                            var sourceArray = (Array)sourceValue;
                            copiedValue = Activator.CreateInstance(type, new object[] { sourceArray.Length });
                        }
                        else
                        {
                            var constructor = typeInfo.DeclaredConstructors.First(e=>e.GetParameters().Count()==0);
                            copiedValue = constructor.Invoke(null);
                        }
                    }
                    else
                    {
                        copiedValue = destinationValue;
                    }
                    SetValue(memberExpression, destination, copiedValue);
                }
            }
            return new Tuple<object, object>(sourceValue, copiedValue);
        }
        
        public static List<PropertyInfo> GetPrimitiveOrSimpleProperties(
            Type type, IReadOnlyCollection<Type> simpleTextTypes, IReadOnlyCollection<Type> simpleNumberTypes)
        {
            var properties = type.GetTypeInfo().DeclaredProperties;
            var list = new List<PropertyInfo>();
            foreach (var propertyInfo in properties)
                if (propertyInfo.CanRead && propertyInfo.CanWrite && propertyInfo.GetIndexParameters().Length == 0)
                {
                    var propertyType = propertyInfo.PropertyType;
                    var typeInfo = propertyType.GetTypeInfo();
                    if (typeInfo.IsPrimitive 
                        || propertyType==typeof(string) 
                        || simpleTextTypes.Contains(propertyInfo.PropertyType)
                        || simpleNumberTypes.Contains(propertyInfo.PropertyType))
                    {
                        list.Add(propertyInfo);
                    }
                    else
                    {
                        var baseNullableType = Nullable.GetUnderlyingType(propertyType);
                        if (baseNullableType != null && baseNullableType.GetTypeInfo().IsPrimitive)
                        {
                            list.Add(propertyInfo);
                        }
                    }
                }
            return list;
        }
    }
}