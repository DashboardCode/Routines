using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DashboardCode.Routines.Json
{
    public class RulesDictionaryBase<TEntity>
    {
        internal readonly bool useToString;
        protected readonly Func<StringBuilder, bool> nullSerializer;
        protected readonly bool handleNullProperty;
        protected readonly InternalNodeOptions internalNodeOptions;
        internal readonly Dictionary<Type, SerializerOptions> dictionary = new Dictionary<Type, SerializerOptions>();
        protected readonly string dateTimeFormat;
        protected readonly string floatingPointFormat;

        public RulesDictionaryBase(
            bool useToString,
            string dateTimeFormat,
            string floatingPointFormat,
            bool stringAsJsonLiteral,
            bool stringJsonEscape,
            Func<StringBuilder, bool> nullSerializer, bool handleNullProperty, InternalNodeOptions internalNodeOptions
        )
        {
            this.useToString = useToString;
            this.nullSerializer = nullSerializer;
            this.handleNullProperty = handleNullProperty;
            this.internalNodeOptions = internalNodeOptions;
            this.dateTimeFormat = dateTimeFormat;
            this.floatingPointFormat = floatingPointFormat;
        }

        protected void AddTypeRuleForCurrentInclude<T>(
            Delegate @delegate,
            Func<StringBuilder, bool> nullSerializer,
            bool handleNullProperty,
            InternalNodeOptions internalNodeOptions
        )
        {
            AddSerailizer(typeof(T), new SerializerOptions(@delegate, nullSerializer, handleNullProperty, internalNodeOptions));
        }

        protected void AddTypeRuleOptimized<T>(
              Expression<Func<StringBuilder, T, bool>> funcExpression
        )
        {
            var methodInfo = JsonChainTools.GetMethodInfoExpr(funcExpression);
            var type = typeof(T);

            var formatterDelegateType = typeof(Func<,,>).MakeGenericType(typeof(StringBuilder), type, typeof(bool));
            var @delegate = methodInfo.CreateDelegate(formatterDelegateType);
            AddTypeRuleForCurrentInclude<T>(@delegate, nullSerializer, handleNullProperty, internalNodeOptions);
        }

        protected void AddSerailizer(Type type, SerializerOptions serializer)
        {
            dictionary[type] = serializer;
        }

        protected void AddRule<T>(
            Func<StringBuilder, T, bool> func = null,
            Func<StringBuilder, bool> nullSerializer = null,
            bool? handleNullProperty = null,
            InternalNodeOptions internalNodeOptions = null
        ) 
        {
            AddTypeRuleForCurrentInclude<T>(func, nullSerializer ?? this.nullSerializer, handleNullProperty ?? this.handleNullProperty, internalNodeOptions ?? this.internalNodeOptions);
        }
    }



    public class RulesSubDictionary<TEntity>: RulesDictionaryBase<TEntity>
    {
        internal readonly ChainNode root;

        public RulesSubDictionary(
                ChainNode root,
                bool useToString,
                string dateTimeFormat,
                string floatingPointFormat,
                bool stringAsJsonLiteral,
                bool stringJsonEscape,
                Func<StringBuilder, bool> nullSerializer, bool handleNullProperty, InternalNodeOptions internalNodeOptions
            ):base(useToString, dateTimeFormat, floatingPointFormat, stringAsJsonLiteral, stringJsonEscape, nullSerializer, handleNullProperty, internalNodeOptions)
        {
            this.root = root;

            AddTypeRuleOptimized<bool>((sb, t) => JsonValueStringBuilderExtensions.SerializeBool(sb, t));
            if (stringAsJsonLiteral)
                AddTypeRuleOptimized<string>((sb, t) => JsonValueStringBuilderExtensions.SerializeStringAsJsonLiteral(sb, t));
            else if (!stringJsonEscape)
                AddTypeRuleOptimized<string>((sb, t) => JsonValueStringBuilderExtensions.SerializeString(sb, t));
            else
                AddTypeRuleOptimized<string>((sb, t) => JsonValueStringBuilderExtensions.SerializeEscapeString(sb, t));
            if (dateTimeFormat == null)
                AddTypeRuleOptimized<DateTime>((sb, t) => JsonValueStringBuilderExtensions.SerializeToIso8601WithMs(sb, t));
            else
                AddRule<DateTime>((sb, t) => JsonValueStringBuilderExtensions.SerializeDateTimeDotNet(sb, t, dateTimeFormat));
            if (floatingPointFormat != null)
            {
                AddRule<double>((sb, t) => JsonValueStringBuilderExtensions.SerializeDoubleDotNet(sb, t, floatingPointFormat));
                AddRule<float>((sb, t) => JsonValueStringBuilderExtensions.SerializeFloatDotNet(sb, t, floatingPointFormat));
            }
            AddTypeRuleOptimized<byte[]>((sb, t) => JsonValueStringBuilderExtensions.SerializeBase64(sb, t));
            AddTypeRuleOptimized<decimal>((sb, t) => JsonValueStringBuilderExtensions.SerializePrimitive(sb, t));
        }

        public new RulesSubDictionary<TEntity> AddRule<T>(
            Func<StringBuilder, T, bool> func = null,
            Func<StringBuilder, bool> nullSerializer = null,
            bool? handleNullProperty = null,
            InternalNodeOptions internalNodeOptions = null
        )
        {
            base.AddRule(func, nullSerializer, handleNullProperty, internalNodeOptions);
            return this;
        }
    }

    public class RulesDictionary<TEntity>: RulesDictionaryBase<TEntity>
    {
        List<RulesSubDictionary<TEntity>> subsets = new List<RulesSubDictionary<TEntity>>();

        public RulesDictionary(
            bool useToString, 
            string dateTimeFormat, 
            string floatingPointFormat, 
            bool stringAsJsonLiteral,
            bool stringJsonEscape,
            Func<StringBuilder, bool> nullSerializer, bool handleNullProperty, InternalNodeOptions internalNodeOptions)
            : base(useToString, dateTimeFormat, floatingPointFormat, stringAsJsonLiteral, stringJsonEscape, nullSerializer, handleNullProperty, internalNodeOptions)
        {

            AddTypeRuleOptimized<bool>((sb, t) => JsonValueStringBuilderExtensions.SerializeBool(sb, t));
            if (stringAsJsonLiteral)
                AddTypeRuleOptimized<string>((sb, t) => JsonValueStringBuilderExtensions.SerializeStringAsJsonLiteral(sb, t));
            else if (!stringJsonEscape)
                AddTypeRuleOptimized<string>((sb, t) => JsonValueStringBuilderExtensions.SerializeString(sb, t));
            else
                AddTypeRuleOptimized<string>((sb, t) => JsonValueStringBuilderExtensions.SerializeEscapeString(sb, t));
            if (dateTimeFormat == null)
                AddTypeRuleOptimized<DateTime>((sb, t) => JsonValueStringBuilderExtensions.SerializeToIso8601WithMs(sb, t));
            else
                AddRule<DateTime>((sb, t) => JsonValueStringBuilderExtensions.SerializeDateTimeDotNet(sb, t, dateTimeFormat));
            if (floatingPointFormat != null)
            {
                AddRule<double>((sb, t) => JsonValueStringBuilderExtensions.SerializeDoubleDotNet(sb, t, floatingPointFormat));
                AddRule<float>((sb, t) => JsonValueStringBuilderExtensions.SerializeFloatDotNet(sb, t, floatingPointFormat));
            }
            AddTypeRuleOptimized<byte[]>((sb, t) => JsonValueStringBuilderExtensions.SerializeBase64(sb, t));
            AddTypeRuleOptimized<decimal>((sb, t) => JsonValueStringBuilderExtensions.SerializePrimitive(sb, t));
        }

        public new RulesDictionary<TEntity> AddRule<T>(
            Func<StringBuilder, T, bool> func = null,
            Func<StringBuilder, bool> nullSerializer = null,
            bool? handleNullProperty = null,
            InternalNodeOptions internalNodeOptions = null
        )
        {
            base.AddRule(func, nullSerializer, handleNullProperty, internalNodeOptions);
            return this;
        }

        public RulesDictionary<TEntity> Subset(
            Include<TEntity> include,
            Action<RulesSubDictionary<TEntity>> config=null,
            bool? useToString = null,
            string dateTimeFormat = null,
            string floatingPointFormat = null,
            bool? stringAsJsonLiteral = null,
            bool? stringJsonEscape = null,
            Func<StringBuilder, bool> nullSerializer = null,
            bool? handleNullProperty = null,
            InternalNodeOptions internalNodeOptions = null
        )
        {
            if (include == null)
                throw new ArgumentNullException(nameof(include), "Serialization for subset can't be configured if subset is defined for the root only include (include is null)");

            var subDictionary = new RulesSubDictionary<TEntity>(
                include.GetChainNode(),
                useToString         ?? this.useToString,
                dateTimeFormat      ?? this.dateTimeFormat, 
                floatingPointFormat ?? this.floatingPointFormat,
                stringAsJsonLiteral ?? false, 
                stringJsonEscape    ?? true, 
                nullSerializer      ?? this.nullSerializer, 
                handleNullProperty  ?? this.handleNullProperty, 
                internalNodeOptions ?? this.internalNodeOptions);
            config?.Invoke(subDictionary);

            subsets.Add(subDictionary);

            return this;
        }

        #region AddSerailizer, GetLeafSerializerOptions, GeInternalNodeOptions
        private Dictionary<Type, SerializerOptions> GetDictionary(ChainNode node)
        {
            var theDictionary = default(Dictionary<Type, SerializerOptions>);
            if (node is ChainPropertyNode chainPropertyNode)
            {
                var path = ChainNodeTree.FindLinkedRootPath(chainPropertyNode);
                for (int i = subsets.Count - 1; i >= 0; i--)
                {
                    var subset = subsets[i];
                    if (ChainNodeTree.IsSubsetOf(path, subset.root))
                    {
                        theDictionary = subset.dictionary;
                        break;
                    }
                }
            }
            if (theDictionary == default(Dictionary<Type, SerializerOptions>))
                theDictionary = dictionary;
            return theDictionary;
        }

        internal SerializerOptions GetLeafSerializerOptions(ChainNode node)
        {
            var theDictionary = GetDictionary(node);
            var serializationType = Nullable.GetUnderlyingType(node.Type) ?? node.Type;
            if (!theDictionary.TryGetValue(serializationType, out SerializerOptions rule))
            {
                var @delegate = CreateGeneralSerializer(serializationType, useToString);
                rule = new SerializerOptions(@delegate, nullSerializer, handleNullProperty, internalNodeOptions);
            }

            if (rule?.Serializer == null)
                throw new NotConfiguredException($"Node '{node.FindLinkedRootXPath()}' included as leaf but serializer for its type '{serializationType.FullName}' is not configured");
            return rule;
        }

        internal InternalNodeOptions GeInternalNodeOptions(ChainNode node, bool isEnumerable)
        {
            var theDictionary = GetDictionary(node);
            var serializationType = Nullable.GetUnderlyingType(node.Type) ?? node.Type;
            var options = default(InternalNodeOptions);
            if (theDictionary.TryGetValue(serializationType, out SerializerOptions rule))
                options = rule.InternalNodeOptions;
            return options??internalNodeOptions;
        }
        #endregion

        internal static Delegate CreateGeneralSerializer(Type serializationType, bool useToString)
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
