using System;
using System.Reflection;
using System.Collections.Generic;

using System.Text;

namespace DashboardCode.Routines.Json
{
    public static class GenericExtensions
    {
        public static string ToJsonAll<T>(this IEnumerable<T> enumerable,  
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
            , string rootAsProperty = null
            , Action<IJsonRootPropertyAppender> rootPropertyAppender = null
            , bool rootHandleNull = true
            , bool rootHandleEmptyLiteral = true
            , int stringBuilderCapacity = 16
            )
        {
            var theDelegate = cache.GetEnumerableFormatter(include: include,
                leafRule: leafRule, config: config, useToString: useToString,
                dateTimeFormat: dateTimeFormat, floatingPointFormat: floatingPointFormat,
                objectAsArray: objectAsArray, handleEmptyObjectLiteral: handleEmptyObjectLiteral,
                handleEmptyArrayLiteral: handleEmptyArrayLiteral, 
                nullSerializer: nullSerializer, handleNullProperty: handleNullProperty, 
                nullArraySerializer: nullArraySerializer, 
                handleNullArrayProperty: handleNullArrayProperty, 
                rootAsProperty: rootAsProperty, rootPropertyAppender: rootPropertyAppender,
                rootHandleNull: rootHandleNull,
                rootHandleEmptyLiteral: rootHandleEmptyLiteral, stringBuilderCapacity: stringBuilderCapacity
                );
            if (!(theDelegate is Func<IEnumerable<T>, string> formatter))
                throw new NotImplementedException("It seems you reuse CachedFormatter. It is forbidden. Use one CachedFormatter for one Include");
            var json = formatter(enumerable);
            return json;
        }

        public static string ToJson<T>(this T entity,
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
            string rootAsProperty = null,
            Action<IJsonRootPropertyAppender> rootPropertyAppender = null,
            bool rootHandleNull = true, 
            bool rootHandleEmptyLiteral = true,
            int stringBuilderCapacity = 16

            )
        {
            var theDelegate = cache.GetFormatter(
                include: include, 
                leafRule: leafRule, config: config, useToString: useToString, 
                dateTimeFormat: dateTimeFormat, floatingPointFormat: floatingPointFormat, 
                objectAsArray: objectAsArray, handleEmptyObjectLiteral: handleEmptyObjectLiteral, handleEmptyArrayLiteral: handleEmptyArrayLiteral,
                nullSerializer: nullSerializer, handleNullProperty: handleNullProperty, nullArraySerializer: nullArraySerializer,
                handleNullArrayProperty: handleNullArrayProperty, 
                rootAsProperty: rootAsProperty, rootPropertyAppender: rootPropertyAppender,
                rootHandleNull: rootHandleNull, rootHandleEmptyLiteral: rootHandleEmptyLiteral, 
                stringBuilderCapacity: stringBuilderCapacity
                );
            if (!(theDelegate is Func<T, string> formatter))
                throw new NotImplementedException("It seems you reuse CachedFormatter. It is forbidden. Use one CachedFormatter for one Include");
            var json = formatter(entity);
            return json;
        }

        public static string ToJsonAll<T, TParameters>(this IEnumerable<T> enumerable,
            TParameters parameters,
            CachedFormatter cache,
            // TODO:
            // Include<T,TParameters> include = null,
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
            , string rootAsProperty = null
            , Action<IJsonRootPropertyAppender, TParameters> rootPropertyAppender = null
            , bool rootHandleNull = true
            , bool rootHandleEmptyLiteral = true
            , int stringBuilderCapacity = 16
            )
        {
            var theDelegate = cache.GetEnumerableFormatter(include: include,
                leafRule: leafRule, config: config, useToString: useToString,
                dateTimeFormat: dateTimeFormat, floatingPointFormat: floatingPointFormat,
                objectAsArray: objectAsArray, handleEmptyObjectLiteral: handleEmptyObjectLiteral,
                handleEmptyArrayLiteral: handleEmptyArrayLiteral,
                nullSerializer: nullSerializer, handleNullProperty: handleNullProperty,
                nullArraySerializer: nullArraySerializer,
                handleNullArrayProperty: handleNullArrayProperty,
                rootAsProperty: rootAsProperty, rootPropertyAppender: rootPropertyAppender,
                rootHandleNull: rootHandleNull,
                rootHandleEmptyLiteral: rootHandleEmptyLiteral, stringBuilderCapacity: stringBuilderCapacity
                );
            if (!(theDelegate is Func<IEnumerable<T>, TParameters, string> formatter))
                throw new NotImplementedException("It seems you reuse CachedFormatter. It is forbidden. Use one CachedFormatter for one Include");
            var json = formatter(enumerable, parameters);
            return json;
        }

        public static string ToJson<T,TParameters>(this T entity,
            TParameters parameters,
            CachedFormatter cache,
            // TODO:
            // Include<T,TParameters> include = null,
            Include<T> include = null,
            Func<ChainNode, IEnumerable<MemberInfo>> leafRule = null,
             Action<RulesDictionary<T>> config = null,
            bool useToString = false,
            string dateTimeFormat = null,
            string floatingPointFormat = null,
            bool objectAsArray = false,
            bool handleEmptyObjectLiteral = true,
            bool handleEmptyArrayLiteral = true,
            Func<StringBuilder, bool> nullSerializer = null,
            bool handleNullProperty = true,
            Func<StringBuilder, bool> nullArraySerializer = null,
            bool handleNullArrayProperty = true,
            string rootAsProperty = null,
            Action<IJsonRootPropertyAppender> rootPropertyAppender = null,
            bool rootHandleNull = true,
            bool rootHandleEmptyLiteral = true,
            int stringBuilderCapacity = 16

            )
        {
            var theDelegate = cache.GetFormatter(
                include: include,
                leafRule: leafRule, config: config, useToString: useToString,
                dateTimeFormat: dateTimeFormat, floatingPointFormat: floatingPointFormat,
                objectAsArray: objectAsArray, handleEmptyObjectLiteral: handleEmptyObjectLiteral, handleEmptyArrayLiteral: handleEmptyArrayLiteral,
                nullSerializer: nullSerializer, handleNullProperty: handleNullProperty, nullArraySerializer: nullArraySerializer,
                handleNullArrayProperty: handleNullArrayProperty,
                rootAsProperty: rootAsProperty, rootPropertyAppender: rootPropertyAppender,
                rootHandleNull: rootHandleNull, rootHandleEmptyLiteral: rootHandleEmptyLiteral,
                stringBuilderCapacity: stringBuilderCapacity
                );
            if (!(theDelegate is Func<T, TParameters, string> formatter))
                throw new NotImplementedException("It seems you reuse CachedFormatter. It is forbidden. Use one CachedFormatter for one Include");
            var json = formatter(entity, parameters);
            return json;
        }
    }
}