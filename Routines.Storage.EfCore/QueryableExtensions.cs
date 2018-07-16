using System;
using System.Linq;
using System.Collections.Generic;

using DashboardCode.Routines.Json;

namespace DashboardCode.Routines.Storage.EfCore
{
    public class CachedFormatter
    {
        Delegate formatter;

        internal Delegate GetFormatter<T>(Include<T> include = null)
        {
            if (formatter != null)
                return formatter;
            else
            {
                if (include == null)
                {
                    include = IncludeExtensions.CreateDefaultEfCoreInclude<T>();
                }
                else
                {
                    include = IncludeExtensions.AppendLeafs(include, new LeafRulesDictionaryBase(LeafRulesDictionaryBase.IncludeLeafsEfCore));
                }
                formatter = JsonManager.ComposeEnumerableFormatter(include);
                return formatter;
            }
        }
    }

    public static class QueryableExtensions
    {
        public static string ToJson<T>(this IQueryable<T> queryable, CachedFormatter cache, Include<T> include = null)
        {
            var theDelegate = cache.GetFormatter(include);
            var formatter = (Func<IEnumerable<T>, string>)theDelegate;
            var enumerable = (IEnumerable<T>)queryable;
            var json = formatter(enumerable);
            return json;
        }
    }
}