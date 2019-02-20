using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

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
            , bool objectAsArray = false
            , bool handleEmptyObjectLiteral = true
            , bool handleEmptyArrayLiteral = true
            , Func<StringBuilder, bool> nullSerializer = null
            , bool handleNullProperty = true
            , Func<StringBuilder, bool> nullArraySerializer = null
            , bool handleNullArrayProperty = true
            , Action<IJsonRootPropertyAppender> rootPropertyAppender = null
            , string rootAsProperty = null
            , bool rootHandleNull = true
            , bool rootHandleEmptyLiteral = true
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
                formatter = JsonManager.ComposeEnumerableFormatter(
                    root: root, config: config, useToString: useToString, dateTimeFormat: dateTimeFormat, 
                    floatingPointFormat: floatingPointFormat, objectAsArray: objectAsArray,
                    handleEmptyObjectLiteral: handleEmptyObjectLiteral,
                    handleEmptyArrayLiteral: handleEmptyArrayLiteral, nullSerializer: nullSerializer, 
                    handleNullProperty: handleNullProperty, nullArraySerializer: nullArraySerializer, handleNullArrayProperty: handleNullArrayProperty,
                    rootAsProperty : rootAsProperty, rootPropertyAppender: rootPropertyAppender,
                    rootHandleNull: rootHandleNull, rootHandleLiteral: rootHandleEmptyLiteral, stringBuilderCapacity: stringBuilderCapacity
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
                formatter = JsonManager.ComposeFormatter(root: root, 
                    config: config, useToString: useToString, dateTimeFormat: dateTimeFormat, 
                    floatingPointFormat: floatingPointFormat, objectAsArray: objectAsArray, 
                    handleEmptyObjectLiteral: handleEmptyObjectLiteral,
                    handleEmptyArrayLiteral: handleEmptyArrayLiteral, 
                    nullSerializer: nullSerializer, handleNullProperty: handleNullProperty, 
                    nullArraySerializer: nullArraySerializer, handleNullArrayProperty: handleNullArrayProperty,
                    rootAsProperty: rootAsProperty,
                    rootPropertyAppender: rootPropertyAppender,
                    rootHandleNull: rootHandleNull, 
                    rootHandleEmptyLiteral: rootHandleEmptyLiteral, 
                    stringBuilderCapacity);
                return formatter;
            }
        }

        public Delegate GetEnumerableFormatter<T, TParameters>(
            // TODO:
            // Include<T,TParameters> include = null,
            Include<T> include = null,
            Func<ChainNode, IEnumerable<MemberInfo>> leafRule = null
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
            , Action<IJsonRootPropertyAppender, TParameters> rootPropertyAppender = null
            , string rootAsProperty = null
            , bool rootHandleNull = true
            , bool rootHandleEmptyLiteral = true
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
                formatter = JsonManager.ComposeEnumerableFormatter<T, TParameters>(
                    root: root, config: config, useToString: useToString, dateTimeFormat: dateTimeFormat,
                    floatingPointFormat: floatingPointFormat, objectAsArray: objectAsArray,
                    handleEmptyObjectLiteral: handleEmptyObjectLiteral,
                    handleEmptyArrayLiteral: handleEmptyArrayLiteral, nullSerializer: nullSerializer,
                    handleNullProperty: handleNullProperty, nullArraySerializer: nullArraySerializer, handleNullArrayProperty: handleNullArrayProperty,
                    rootAsProperty: rootAsProperty, rootPropertyAppender: rootPropertyAppender,
                    rootHandleNull: rootHandleNull, rootHandleLiteral: rootHandleEmptyLiteral, stringBuilderCapacity: stringBuilderCapacity
                    );
                return formatter;
            }
        }

        public Delegate GetFormatter<T, TParameters>(
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
            Action<IJsonRootPropertyAppender, TParameters> rootPropertyAppender = null,
            bool rootHandleNull = true,
            bool rootHandleEmptyLiteral = true,
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
                        root.AppendLeafs(leafRule ?? LeafRuleManager.DefaultEfCore);
                }
                else
                {
                    if (leafRule != null)
                        root.AppendLeafs(leafRule);
                }
                formatter = JsonManager.ComposeFormatter<T,TParameters>(root: root,
                    config: config, useToString: useToString, dateTimeFormat: dateTimeFormat,
                    floatingPointFormat: floatingPointFormat, objectAsArray: objectAsArray,
                    handleEmptyObjectLiteral: handleEmptyObjectLiteral,
                    handleEmptyArrayLiteral: handleEmptyArrayLiteral,
                    nullSerializer: nullSerializer, handleNullProperty: handleNullProperty,
                    nullArraySerializer: nullArraySerializer, handleNullArrayProperty: handleNullArrayProperty,
                    rootAsProperty: rootAsProperty,
                    rootPropertyAppender: rootPropertyAppender,
                    rootHandleNull: rootHandleNull,
                    rootHandleEmptyLiteral: rootHandleEmptyLiteral,
                    stringBuilderCapacity);
                return formatter;
            }
        }
    }
}
