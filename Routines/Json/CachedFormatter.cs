using System;
using System.Collections.Generic;
using System.Reflection;

namespace DashboardCode.Routines.Json
{
    public class CachedFormatter
    {
        Delegate formatter;

        public Delegate GetFormatter<T>(Include<T> include = null, Func<ChainNode, IEnumerable<MemberInfo>> leafRule = null)
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
                formatter = JsonManager.ComposeEnumerableFormatter<T>(root);
                return formatter;
            }
        }
    }
}
