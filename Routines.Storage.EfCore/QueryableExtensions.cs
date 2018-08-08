using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using DashboardCode.Routines.Json;

namespace DashboardCode.Routines.Storage.EfCore
{
    public static class QueryableExtensions
    {
        public static string ToJson<T>(this IQueryable<T> queryable, 
            CachedFormatter cache, 
            Include<T> include = null,
            Func<ChainNode, IEnumerable<MemberInfo>> leafRule = null)
        {
            var theDelegate = cache.GetFormatter(include, leafRule);
            if (!(theDelegate is Func<IEnumerable<T>, string> formatter))
                throw new NotImplementedException("It seems you reuse CachedFormatter. It is forbidden. Use one CachedFormatter for one Include");
            var enumerable = (IEnumerable<T>)queryable;
            var json = formatter(enumerable);
            return json;
        }
    }
}