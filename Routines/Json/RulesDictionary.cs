using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Vse.Routines.Json
{
    public class RulesDictionary<TEntity>
    {
        public readonly bool useToString ;
        public readonly Dictionary<string, Dictionary<Type, SerializerOptions>> dictionary = new Dictionary<string, Dictionary<Type, SerializerOptions>>();
        Include<TEntity> currentInclude = null;

        public readonly Func<StringBuilder, bool> nullSerializer;
        public readonly bool handleNullProperty;
        public readonly InternalNodeOptions internalNodeOptions;
        public RulesDictionary(bool useToString, string dateTimeFormat, string floatingPointFormat, Func<StringBuilder, bool> nullSerializer, bool handleNullProperty, InternalNodeOptions internalNodeOptions)
        {
            this.useToString = useToString;
            this.nullSerializer = nullSerializer;
            this.handleNullProperty = handleNullProperty;
            this.internalNodeOptions = internalNodeOptions;

            AddTypeRuleOptimized<bool>((sb, t) => JsonValueStringBuilderExtensions.SerializeBool(sb, t));
            AddTypeRuleOptimized<string>((sb, t) => JsonValueStringBuilderExtensions.SerializeEscapeString(sb, t));
            if (dateTimeFormat == null)
                AddTypeRuleOptimized<DateTime>((sb, t) => JsonValueStringBuilderExtensions.SerializeToIso8601WithMs(sb, t));
            else
                AddTypeRule<DateTime>((sb, t) => JsonValueStringBuilderExtensions.SerializeDateTimeDotNet(sb, t, dateTimeFormat));
            if (floatingPointFormat != null)
            {
                AddTypeRule<double>((sb, t) => JsonValueStringBuilderExtensions.SerializeDoubleDotNet(sb, t, floatingPointFormat));
                AddTypeRule<float>((sb, t) => JsonValueStringBuilderExtensions.SerializeFloatDotNet(sb, t, floatingPointFormat));
            }
            AddTypeRuleOptimized<byte[]>((sb, t) => JsonValueStringBuilderExtensions.SerializeBase64(sb, t));
            AddTypeRuleOptimized<decimal>((sb, t) => JsonValueStringBuilderExtensions.SerializePrimitive(sb, t));
        }

        public RulesDictionary<TEntity> SubInclude(
            Include<TEntity> include,
            Action<RulesDictionary<TEntity>> config
        ){
            
            config(this);
            return this;
        }

        private RulesDictionary<TEntity> AddTypeRuleOptimized<T>(
            Expression<Func<StringBuilder, T, bool>> funcExpression
            )
        {
            var methodInfo = JsonChainTools.GetMethodInfoExpr(funcExpression);
            var type = typeof(T);

            var formatterDelegateType = typeof(Func<,,>).MakeGenericType(typeof(StringBuilder), type, typeof(bool));
            var @delegate = methodInfo.CreateDelegate(formatterDelegateType);
            AddTypeRuleForCurrentInclude<T>(@delegate, nullSerializer, handleNullProperty, internalNodeOptions);
            return this;
        }

        public RulesDictionary<TEntity> AddTypeRule<T>(
            Func<StringBuilder, T, bool> func =null,
            Func<StringBuilder, bool> nullSerializer = null,
            bool? handleNullProperty = true,
            InternalNodeOptions internalNodeOptions = null
        ) //where T : class
        {
            AddTypeRuleForCurrentInclude<T>(func, nullSerializer??this.nullSerializer, handleNullProperty ?? this.handleNullProperty, internalNodeOptions ?? this.internalNodeOptions);
            return this;
        }

        //public RulesDictionary<TEntity> AddTypeRuleRef<T>(
        //    Func<StringBuilder, T, bool> func,
        //    InternalNodeOptions internalNodeOptions = null
        //) where T : struct
        //{
        //    AddTypeRuleForCurrentInclude<T>(func, nullSerializer, handleNullProperty, internalNodeOptions);
        //    return this;
        //}

        private void AddTypeRuleForCurrentInclude<T>(
            Delegate @delegate,
            Func<StringBuilder, bool> nullSerializer,
            bool handleNullProperty,
            InternalNodeOptions internalNodeOptions
        )
        {
            var paths = currentInclude.GetXPaths();
            foreach (var p in paths)
                AddSerailizer(p, typeof(T), new SerializerOptions(@delegate, nullSerializer, handleNullProperty,internalNodeOptions));
        }

        public SerializerOptions GetLeafSerializerOptions(ChainNode node)
        {
            var path = "/";
            var serializationType = Nullable.GetUnderlyingType(node.Type) ?? node.Type;
            var rule = default(SerializerOptions);
            if (dictionary.TryGetValue(path, out Dictionary<Type, SerializerOptions> typeDictionary))
                if (!typeDictionary.TryGetValue(serializationType, out rule))
                {
                    var @delegate = CreateGeneralSerializer(serializationType, useToString);
                    rule = new SerializerOptions(@delegate, nullSerializer, handleNullProperty, internalNodeOptions);
                }

            if (rule?.Serializer == null)
                throw new NotConfiguredException($"Node '{node.GetXPathOfNode()}' included as leaf but serializer for its type '{serializationType.FullName}' is not configured");
            return rule;
        }

        public InternalNodeOptions GeInternalNodeOptions(ChainNode node, bool isEnumerable)
        {
            var path = "/";
            var serializationType = Nullable.GetUnderlyingType(node.Type) ?? node.Type;
            var rule = default(SerializerOptions);
            var options = default(InternalNodeOptions);
            if (dictionary.TryGetValue(path, out Dictionary<Type, SerializerOptions> typeDictionary))
                if (typeDictionary.TryGetValue(serializationType, out rule))
                    options = rule.InternalNodeOptions;
            return options??internalNodeOptions;
        }

        public void AddSerailizer(string path, Type type, SerializerOptions serializer)
        {
            var typeDictionary= default(Dictionary<Type, SerializerOptions>);
            if (!dictionary.TryGetValue(path, out typeDictionary))
            {
                typeDictionary = new Dictionary<Type, SerializerOptions>();
                dictionary.Add(path, typeDictionary);
            }
            typeDictionary[type] = serializer;
        }
        
        public static Delegate CreateGeneralSerializer(Type serializationType, bool useToString)
        {
            Delegate @delegate;
            bool isPrimitive = serializationType.GetTypeInfo().IsPrimitive;
           
            if (isPrimitive)
            {
                var genericMethodInfo = typeof(JsonValueStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonValueStringBuilderExtensions.SerializePrimitive));
                var methodInfo = genericMethodInfo.MakeGenericMethod(serializationType);
                var formatterDelegateType = typeof(Func<,,>).MakeGenericType(typeof(StringBuilder), serializationType, typeof(bool));
                @delegate = methodInfo.CreateDelegate(formatterDelegateType);
            }
            else
            {
                if (useToString)
                {
                    MethodInfo genericMethodInfo = null;
                    if (serializationType.GetTypeInfo().IsValueType)
                        genericMethodInfo = typeof(JsonValueStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonValueStringBuilderExtensions.SerializeEscapingTextVal));
                    else
                        genericMethodInfo = typeof(JsonValueStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonValueStringBuilderExtensions.SerializeEscapingTextRef));
                    var methodInfo = genericMethodInfo.MakeGenericMethod(serializationType);
                    var formatterDelegateType = typeof(Func<,,>).MakeGenericType(typeof(StringBuilder), serializationType, typeof(bool));
                    @delegate = methodInfo.CreateDelegate(formatterDelegateType);
                }
                else
                {
                    @delegate = null;
                }
            }
            return  @delegate;
        }
    }
}
