using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Vse.Routines.Json
{
    public class RulesDictionary
    {
        
        public static RulesDictionary CreateDefault(string dateTimeFormat=null, string floatingPointFormat=null)
        {
            var rulesDictionary = new RulesDictionary();
            rulesDictionary.AddTypeRule(typeof(bool), JsonChainNodeTools.GetMethodInfoExpr<bool>((sb, t) => JsonValueStringBuilderExtensions.SerializeBool(sb, t)));
            rulesDictionary.AddTypeRule(typeof(string), JsonChainNodeTools.GetMethodInfoExpr<string>((sb, t) => JsonValueStringBuilderExtensions.SerializeEscapeString(sb, t)));
            if (dateTimeFormat == null)
                rulesDictionary.AddTypeRule(typeof(DateTime), JsonChainNodeTools.GetMethodInfoExpr<DateTime>((sb, t) => JsonValueStringBuilderExtensions.SerializeToIso8601WithMs(sb, t)));
            else
                rulesDictionary.AddTypeRule<DateTime>((sb, t) => JsonValueStringBuilderExtensions.SerializeDateTimeDotNet(sb, t, dateTimeFormat));
            if (floatingPointFormat != null)
            {
                rulesDictionary.AddTypeRule<double>((sb, t) => JsonValueStringBuilderExtensions.SerializeDoubleDotNet(sb, t, floatingPointFormat));
                rulesDictionary.AddTypeRule<float> ((sb, t) => JsonValueStringBuilderExtensions.SerializeFloatDotNet(sb, t, floatingPointFormat));
                //rulesDictionary.AddTypeRule<Sinlge>((sb, t) => JsonValueStringBuilderExtensions.SerializeFloatDotNet(sb, t, floatingPointFormat));
            }
            rulesDictionary.AddTypeRule(typeof(byte[]),  JsonChainNodeTools.GetMethodInfoExpr<byte[]> ((sb, t) => JsonValueStringBuilderExtensions.SerializeBase64(sb, t)));
            rulesDictionary.AddTypeRule(typeof(decimal), JsonChainNodeTools.GetMethodInfoExpr<decimal>((sb, t) => JsonValueStringBuilderExtensions.SerializePrimitive(sb, t)));
            return rulesDictionary;
        }

        Dictionary<Type, Delegate> dictionary = new Dictionary<Type, Delegate>();

        public RulesDictionary AddLeafTypeRule<T>(
            Func<StringBuilder, T, bool> func
            )
        {
            dictionary.Add(typeof(T), func);
            return this;
        }

        public RulesDictionary AddTypeRule<T>(
            Func<StringBuilder, T, bool> func,
            Func<StringBuilder, bool> nullFunc=null
            )
        {
            dictionary.Add(typeof(T), func);
            return this;
        }

        public RulesDictionary AddTypeRule(
            Type type,
            MethodInfo methodInfo
            )
        {
            var formatterDelegateType = typeof(Func<,,>).MakeGenericType(typeof(StringBuilder), type, typeof(bool));
            var @delegate = methodInfo.CreateDelegate(formatterDelegateType);
            dictionary.Add(type, @delegate);
            return this;
        }

        public Delegate GetRule(Type type)
        {
            Delegate rule = null;
            dictionary.TryGetValue(type, out rule);
            return rule;
        }
    }
}
