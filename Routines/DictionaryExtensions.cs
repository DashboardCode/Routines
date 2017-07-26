using System;
using System.Collections.Generic;
using System.Text;

namespace DashboardCode.Routines
{
    public static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey,TValue>(this Dictionary<TKey,TValue> dictionary, TKey key)
        {
            dictionary.TryGetValue(key, out TValue child);
            return child;
        }
    }
}
