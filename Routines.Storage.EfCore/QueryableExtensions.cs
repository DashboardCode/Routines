using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using DashboardCode.Routines.Json;
using System.Text;

namespace DashboardCode.Routines.Storage.EfCore
{
    public static class QueryableExtensions
    {
        public static string ToJsonAll<T>(this IQueryable<T> queryable, 
            CachedFormatter cache, 
            Include<T> include = null
            , Func<ChainNode, IEnumerable<MemberInfo>> leafRule = null
            , Action<RulesDictionary<T>> config = null
            , bool useToString = false
            , string dateTimeFormat = null
            , string floatingPointFormat = null
            , bool objectAsArray = false
            , bool handleEmptyObjectLiteral = true
            , bool handleEmptyArrayLiteral = true
            , Func<StringBuilder, bool> nullSerializer = null
            , bool handleNullProperty = true
            , Func<StringBuilder, bool> nullArraySerializer = null
            , bool handleNullArrayProperty = true
            , bool rootHandleNullArray = true
            , bool rootHandleEmptyArrayLiteral = true
            , int stringBuilderCapacity = 16
            )
        {
            var theDelegate = cache.GetEnumerableFormatter(include, leafRule, config, useToString, dateTimeFormat, floatingPointFormat,
                objectAsArray, handleEmptyObjectLiteral,
                handleEmptyArrayLiteral, nullSerializer, handleNullProperty, nullArraySerializer, handleNullArrayProperty, rootHandleNullArray,
                rootHandleEmptyArrayLiteral, stringBuilderCapacity
                );
            if (!(theDelegate is Func<IEnumerable<T>, string> formatter))
                throw new NotImplementedException("It seems you reuse CachedFormatter. It is forbidden. Use one CachedFormatter for one Include");
            var enumerable = (IEnumerable<T>)queryable;
            var json = formatter(enumerable);
            return json;
        }

        public static string ToJson<T>(this IQueryable<T> queryable,
            CachedFormatter cache,
            Include<T> include = null,
            Func<ChainNode, IEnumerable<MemberInfo>> leafRule = null,
             Action<RulesDictionary<T>> config = null,
            bool useToString = false,
            string dateTimeFormat = null,
            string floatingPointFormat = null,
            bool objectAsArray=false,
            bool handleEmptyObjectLiteral = true,
            bool handleEmptyArrayLiteral = true,
            Func<StringBuilder, bool> nullSerializer = null,
            bool handleNullProperty = true,
            Func<StringBuilder, bool> nullArraySerializer = null,
            bool handleNullArrayProperty = true,
            bool rootHandleNullArray = true, bool
            rootHandleEmptyArrayLiteral = true,
            int stringBuilderCapacity = 16

            )
        {
            var theDelegate = cache.GetFormatter(
                include, 
                leafRule, config, useToString, dateTimeFormat, floatingPointFormat, objectAsArray, handleEmptyObjectLiteral, handleEmptyArrayLiteral,
                nullSerializer, handleNullProperty, nullArraySerializer, handleNullArrayProperty, rootHandleNullArray, rootHandleEmptyArrayLiteral, stringBuilderCapacity
                );
            if (!(theDelegate is Func<T, string> formatter))
                throw new NotImplementedException("It seems you reuse CachedFormatter. It is forbidden. Use one CachedFormatter for one Include");
            var enumerable = (T)queryable;
            var json = formatter(enumerable);
            return json;
        }
    }
}