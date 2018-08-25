using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using System.Linq.Expressions;

namespace DashboardCode.Routines.Json
{
    internal class CachedDelegates<TIn, TOut>
    {
        private static readonly ConcurrentDictionary<Expression<Func<TIn,TOut>>, Func<TIn,TOut>> Cache = new ConcurrentDictionary<Expression<Func<TIn, TOut>>, Func<TIn, TOut>>();
        public static Func<TIn, TOut> AsFunc(Expression<Func<TIn, TOut>> expr) =>
            Cache.GetOrAdd(expr, 
                k=>
                k.Compile()
                );

    }

    public class CachedFormatter
    {
        Delegate formatter;

        public Delegate GetEnumerableFormatter<T>(
            Include<T> include = null, 
            Func<ChainNode, IEnumerable<MemberInfo>> leafRule = null
            , Action<RulesDictionary<T>> config = null
            , bool useToString = false
            , string dateTimeFormat = null
            , string floatingPointFormat = null
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
            if (formatter != null)
                return formatter;
            else
            {
                ChainNode root = IncludeExtensions.CreateChainNode(include);
                if (include == null)
                {
                    var type = typeof(T);
                    if (type.IsAssociativeArrayType())
                        root.AppendLeafs(leafRule ?? LeafRuleManager.DefaultEfCore);
                }
                else
                {
                    if (leafRule != null)
                        root.AppendLeafs(leafRule);
                }
                formatter = JsonManager.ComposeEnumerableFormatter(root, config, useToString, dateTimeFormat, floatingPointFormat, handleEmptyObjectLiteral,
                    handleEmptyArrayLiteral, nullSerializer, handleNullProperty, nullArraySerializer, handleNullArrayProperty, rootHandleNullArray, rootHandleEmptyArrayLiteral, stringBuilderCapacity
                    );
                return formatter;
            }
        }

        public Delegate GetFormatter<T>(
            Include<T> include = null, 
            Func<ChainNode, IEnumerable<MemberInfo>> leafRule = null,
             Action<RulesDictionary<T>> config = null,
            bool useToString = false,
            string dateTimeFormat = null,
            string floatingPointFormat = null,
            bool handleEmptyObjectLiteral = true,
            bool handleEmptyArrayLiteral = true,
            Func<StringBuilder, bool> nullSerializer = null,
            bool handleNullProperty = true,
            Func<StringBuilder, bool> nullArraySerializer = null,
            bool handleNullArrayProperty = true,
            bool rootHandleNullArray = true, 
            bool rootHandleEmptyArrayLiteral = true,
            int stringBuilderCapacity = 16)
        {
            if (formatter != null)
                return formatter;
            else
            {
                ChainNode root = IncludeExtensions.CreateChainNode(include);
                if (include == null)
                {
                    var type = typeof(T);
                    if (type.IsAssociativeArrayType())
                        root.AppendLeafs(leafRule??LeafRuleManager.DefaultEfCore);
                }
                else
                {
                    if (leafRule!=null)
                        root.AppendLeafs(leafRule);
                }
                formatter = JsonManager.ComposeFormatter(root, config, useToString, dateTimeFormat, floatingPointFormat, handleEmptyObjectLiteral,
                    handleEmptyArrayLiteral, nullSerializer, handleNullProperty, nullArraySerializer, handleNullArrayProperty, rootHandleNullArray, rootHandleEmptyArrayLiteral, stringBuilderCapacity);
                return formatter;
            }
        }
    }
}
