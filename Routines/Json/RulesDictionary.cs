using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Linq.Expressions;

namespace Vse.Routines.Json
{
    public class RulesDictionary
    {
        
        public static RulesDictionary CreateDefault(string dateTimeFormat=null)
        {
            var rulesDictionary = new RulesDictionary();
            rulesDictionary.AddTypeRule(typeof(bool), JsonChainNodeTools.GetMethodInfoExpr<bool>((sb, t) => JsonValueStringBuilderExtensions.SerializeBool(sb, t)));
            rulesDictionary.AddTypeRule(typeof(string), JsonChainNodeTools.GetMethodInfoExpr<string>((sb, t) => JsonValueStringBuilderExtensions.SerializeEscapeString(sb, t)));
            //if (dateTimeFormat == null)
                rulesDictionary.AddTypeRule(typeof(DateTime), JsonChainNodeTools.GetMethodInfoExpr<DateTime>((sb, t) => JsonValueStringBuilderExtensions.SerializeToIso8601WithMs(sb, t)));
            //else
            {
                //var methodInfo = typeof(JsonValueStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonValueStringBuilderExtensions.SerializeToIso8601WithMs /*.SerializeDateTimeDotNet*/ ));
                //var sbParameterExpression = Expression.Parameter(typeof(StringBuilder), "sb");
                //var tParameterExpression = Expression.Parameter(typeof(DateTime), "t");
                ////var formatConstExpression = Expression.Constant(dateTimeFormat);
                //var callMethodExpression = Expression.Call(methodInfo, new Expression[] { sbParameterExpression, tParameterExpression /*, formatConstExpression */});


                ////var formatterDelegateType = typeof(Func<,,>).MakeGenericType(typeof(StringBuilder), typeof(DateTime), typeof(bool));
                //var lambdaExpression = Expression.Lambda(typeof(Func<StringBuilder, DateTime, bool>), callMethodExpression, new[] { sbParameterExpression, tParameterExpression });

                //var @delegate = (Func<StringBuilder, DateTime, bool>)lambdaExpression.Compile();
                //var sb = new StringBuilder();
                //var b = @delegate(sb, DateTime.Now);
                //var text = sb.ToString();

                //var curriedMethodInfo = @delegate.GetMethodInfo();

                //var genericResolvedDelegate = curriedMethodInfo.CreateDelegate(typeof(Func<StringBuilder, DateTime, bool>), null);

                //rulesDictionary.AddTypeRule(typeof(DateTime), curriedMethodInfo);
            }
            rulesDictionary.AddTypeRule(typeof(byte[]),  JsonChainNodeTools.GetMethodInfoExpr<byte[]> ((sb, t) => JsonValueStringBuilderExtensions.SerializeBase64(sb, t)));
            rulesDictionary.AddTypeRule(typeof(decimal), JsonChainNodeTools.GetMethodInfoExpr<decimal>((sb, t) => JsonValueStringBuilderExtensions.SerializePrimitive(sb, t)));
            return rulesDictionary;
        }

        Dictionary<Type, MethodInfo> dictionary = new Dictionary<Type, MethodInfo>();

        public RulesDictionary AddLeafTypeRule<T>(
            Func<StringBuilder, T, bool> func
            )
        {
            var methodInfo = func.GetMethodInfo();
            dictionary.Add(typeof(T), methodInfo);
            return this;
        }

        public RulesDictionary AddTypeRule<T>(
            Func<StringBuilder, T, bool> func,
            Func<StringBuilder, bool> nullFunc
            )
        {
            var methodInfo = func.GetMethodInfo();
            dictionary.Add(typeof(T), methodInfo);
            return this;
        }

        public RulesDictionary AddTypeRule(
            Type type,
            MethodInfo methodInfo
            )
        {
            dictionary.Add(type, methodInfo);
            return this;
        }

        public MethodInfo GetRule(Type type)
        {
            MethodInfo rule = null;
            dictionary.TryGetValue(type, out rule);
            return rule;
        }

        //public Config<T> AddTypeRule<TProp>(
        //    Func<StringBuilder, TProp, bool> serializer,
        //    Func<StringBuilder, bool> nullSerializer = null)
        //{
        //    var serializerMethodInfo = serializer.GetMethodInfo();
        //    var nullSerializerMethodInfo = (nullSerializer == null)?null: nullSerializer.GetMethodInfo();
        //    rules.Add(new Tuple<Include<T>, SerializerSet, ConfigItem>(
        //        include, 
        //        serializer.GetMethodInfo(),
        //        nullSerializer==null? null : nullSerializer.GetMethodInfo(), 
        //        ConfigItem.REGULAR));
        //    return this;
        //}


        //public Config<T> AddRule<TProp>(
        //    Func<StringBuilder, TProp, bool> serializer,
        //    Func<StringBuilder, bool> nullSerializer = null)
        //{
        //    var serializerMethodInfo = serializer.GetMethodInfo();
        //    var nullSerializerMethodInfo = (nullSerializer == null) ? null : nullSerializer.GetMethodInfo();
        //    rules.Add(new Tuple<Include<T>, MethodInfo, MethodInfo, ConfigItem>(
        //        include,
        //        serializer.GetMethodInfo(),
        //        nullSerializer == null ? null : nullSerializer.GetMethodInfo(),
        //        ConfigItem.REGULAR));
        //    return this;
        //}
    }
}
