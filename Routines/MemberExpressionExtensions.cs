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
        public static string GetMemberName<T1, T2>(this Expression<Func<T1, T2>> expression)
        {
            var memberExpression = (MemberExpression)expression.Body;
            return memberExpression.Member.Name;
        }

        public static string GetMemberName<T1, T2>(this Expression<Func<T1, ICollection<T2>>> expression)
        {
            var memberExpression = (MemberExpression)expression.Body;
            return memberExpression.Member.Name;
            //var member = (PropertyInfo)memberExpression.Member;
            //var name = member.Name;
            //return name;
        }

        public static string GetMemberName<T1, T2>(this Expression<Func<T1, IEnumerable<T2>>> expression)
        {
            var memberExpression = (MemberExpression)expression.Body;
            return memberExpression.Member.Name;
            //var member = (PropertyInfo)memberExpression.Member;
            //var name = member.Name;
            //return name;
        }


        public static object GetMemberValue(this MemberExpression memberExpression, object entity)
        {
            var popertyName = memberExpression.Member.Name;
            var type = entity.GetType();
            var typeInfo = type.GetTypeInfo();
            var propertyInfo = typeInfo.GetProperty(popertyName);
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
            var propertyInfo = typeInfo.GetProperty(popertyName);
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
            var propertyInfo = type.GetTypeInfo().GetProperty(popertyName);
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
        
        // Contravariance enables you to use a LESS derived type (means base) than that specified by the generic parameter. 
        public static Expression<Func<TEntity, IEnumerable<TRelationEntity>>> ContravarianceToIEnumerable<TEntity, TRelationEntity>(
            this Expression<Func<TEntity, ICollection<TRelationEntity>>> getRelation)
        {
            var memberExpression = (MemberExpression)getRelation.Body;
            var member = (PropertyInfo)memberExpression.Member;
            var tParameterExpression = Expression.Parameter(typeof(TEntity), "t");
            var newMemberExpression  = Expression.Property(tParameterExpression, member);

            Expression<Func<TEntity, IEnumerable<TRelationEntity>>> getRelationAsEnumerable =
                Expression.Lambda<Func<TEntity, IEnumerable<TRelationEntity>>>(newMemberExpression, new[] { tParameterExpression });
            return getRelationAsEnumerable;
        }

        public static Action<TEntity, TPropertyValue> CompileSetProperty<TEntity, TPropertyValue>(this Expression<Func<TEntity, TPropertyValue>> expression)
        {
            var memberExpression = (MemberExpression)expression.Body;
            return CompileSetProperty<TEntity, TPropertyValue>(memberExpression);
        }

        public static Action<TEntity, TPropertyValue> CompileSetProperty<TEntity, TPropertyValue>(this MemberExpression memberExpression)
        {
            ParameterExpression eParameter = (ParameterExpression)memberExpression.Expression;
            ParameterExpression vParameter = Expression.Parameter(typeof(TPropertyValue), "v");
            var assign = Expression.Assign(memberExpression, vParameter);
            var lambda = Expression.Lambda<Action<TEntity, TPropertyValue>>(assign, eParameter, vParameter);
            var setter = lambda.Compile();
            return setter;
        }

        public static Action<TEntity, TPropertyValue> CompileSetPropertyCovariance<TEntity, TPropertyValue>(this MemberExpression memberExpression)
        {
            ParameterExpression eParameter = (ParameterExpression)memberExpression.Expression;
            ParameterExpression vParameter = Expression.Parameter(typeof(TPropertyValue), "v");
            var assign = Expression.Assign(memberExpression, Expression.Convert(vParameter, memberExpression.Type));
            var lambda = Expression.Lambda<Action<TEntity, TPropertyValue>>(assign, eParameter, vParameter);
            var setter = lambda.Compile();
            return setter;
        }

        public static Func<TEntity, Action<TPropertyValue>> CompileFunctionalSetter<TEntity, TPropertyValue>(this MemberExpression memberExpression)
        {
            Action<TEntity, TPropertyValue> setter = memberExpression.CompileSetProperty<TEntity, TPropertyValue>();
            Func<TEntity, Action<TPropertyValue>> functionalSetter = e => v => setter(e,v);
            return functionalSetter;
        }
    }
}