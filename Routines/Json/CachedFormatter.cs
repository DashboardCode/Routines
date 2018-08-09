using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DashboardCode.Routines.Json
{
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
