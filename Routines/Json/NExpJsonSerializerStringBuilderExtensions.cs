using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Vse.Routines.Json
{
    public static class NExpJsonSerializerStringBuilderExtensions
    {
        #region Object Serializers
        public static bool SerializeObject<T>(StringBuilder stringBuilder, T t, params Func<StringBuilder, T, bool>[] propertySerializers)
        {
            var @value = false;
            stringBuilder.Append('{');
            foreach (var propertySerializer in propertySerializers)
            {
                var notEmpty = propertySerializer(stringBuilder, t);
                if (notEmpty)
                {
                    if (!@value)
                        @value = true;
                    stringBuilder.Append(',');
                }
            };
            stringBuilder.Length--;
            if (@value)
                stringBuilder.Append('}');
            return @value;
        }

        public static bool SerializeObjectHandleEmpty<T>(StringBuilder stringBuilder, T t, params Func<StringBuilder, T, bool>[] propertySerializers)
        {
            var @value = false;
            stringBuilder.Append('{');
            foreach (var propertySerializer in propertySerializers)
            {
                var notEmpty = propertySerializer(stringBuilder, t);
                if (notEmpty)
                {
                    if (!@value)
                        @value = true;
                    stringBuilder.Append(',');
                }
            };
            if (!@value)
                stringBuilder.Length--;
            stringBuilder.Append('}');
            return @value;
        }
        #endregion

        #region Array Serializers
        public static bool SerializeRefArray<T>(StringBuilder stringBuilder, IEnumerable<T> enumerable, Func<StringBuilder, T, bool> itemSerializer, Func<StringBuilder, bool> nullSerializer) where T: class
        {
            var @value = false;
            stringBuilder.Append('[');
            foreach (var item in enumerable)
            {
                bool notEmpty;
                if (item!=null)
                    notEmpty = itemSerializer(stringBuilder, item);
                else
                    notEmpty = nullSerializer(stringBuilder); 
                if (notEmpty)
                {
                    if (!@value)
                        @value = true;
                    stringBuilder.Append(',');
                }
            };
            stringBuilder.Length--;
            if (@value)
                stringBuilder.Append(']');
            return @value;
        }

        public static bool SerializeRefArrayHandleEmpty<T>(StringBuilder stringBuilder, IEnumerable<T> enumerable, Func<StringBuilder, T, bool> itemSerializer, Func<StringBuilder, bool> nullSerializer) where T : class
        {
            var @value = false;
            stringBuilder.Append('[');
            foreach (var item in enumerable)
            {
                bool notEmpty;
                if (item != null)
                    notEmpty = itemSerializer(stringBuilder, item); 
                else
                    notEmpty = nullSerializer(stringBuilder);
                if (notEmpty)
                {
                    if (!@value)
                        @value = true;
                    stringBuilder.Append(',');
                }
            };
            if (@value)
                stringBuilder.Length--;
            stringBuilder.Append(']');
            return @value;
        }

        public static bool SerializeStructArray<T>(StringBuilder stringBuilder, IEnumerable<T> enumerable, Func<StringBuilder, T, bool> itemSerializer) where T : struct
        {
            var @value = false;
            stringBuilder.Append('[');
            foreach (var item in enumerable)
            {
                bool notEmpty = itemSerializer(stringBuilder, item);
                if (notEmpty)
                {
                    if (!@value)
                        @value = true;
                    stringBuilder.Append(',');
                }
            };
            stringBuilder.Length--;
            if (@value)
                stringBuilder.Append(']');
            return @value;
        }

        public static bool SerializeStructArrayHandleEmpty<T>(StringBuilder stringBuilder, IEnumerable<T> enumerable, Func<StringBuilder, T, bool> itemSerializer) where T : struct
        {
            var @value = false;
            stringBuilder.Append('[');
            foreach (var item in enumerable)
            {
                bool notEmpty = itemSerializer(stringBuilder, item);
                if (notEmpty)
                {
                    if (!@value)
                        @value = true;
                    stringBuilder.Append(',');
                }
            };
            if (@value)
                stringBuilder.Length--;
            stringBuilder.Append(']');
            return @value;
        }

        public static bool SerializeNStructArray<T>(StringBuilder stringBuilder, IEnumerable<T?> enumerable, Func<StringBuilder, T, bool> itemSerializer, Func<StringBuilder, bool> nullSerializer) where T : struct
        {
            var @value = false;
            stringBuilder.Append('[');
            foreach (var item in enumerable)
            {
                bool notEmpty;
                if (item.HasValue)
                    notEmpty = itemSerializer(stringBuilder, item.Value); 
                else
                    notEmpty = nullSerializer(stringBuilder);
                if (notEmpty)
                {
                    if (!@value)
                        @value = true;
                    stringBuilder.Append(',');
                }
            };
            stringBuilder.Length--;
            if (@value)
                stringBuilder.Append(']');
            return @value;
        }

        public static bool SerializeNStructArrayHandleEmpty<T>(StringBuilder stringBuilder, IEnumerable<T?> enumerable, Func<StringBuilder, T, bool> itemSerializer, Func<StringBuilder, bool> nullSerializer) where T : struct
        {
            var @value = false;
            stringBuilder.Append('[');
            foreach (var item in enumerable)
            {
                bool notEmpty;
                if (item.HasValue)
                    notEmpty = itemSerializer(stringBuilder, item.Value); 
                else
                    notEmpty = nullSerializer(stringBuilder);
                if (notEmpty)
                {
                    if (!@value)
                        @value = true;
                    stringBuilder.Append(',');
                }
            };
            if (@value)
                stringBuilder.Length--;
            stringBuilder.Append(']');
            return @value;
        }
        #endregion 

        #region Serialize Struct Property
        /* TODO: change to tree expression
          
         string filterString = "ruby";

         var valueParam = Expression.Parameter(typeof(bool), "value");
         var notEmptyParam = Expression.Parameter(typeof(bool), "notEmpty");
        
         var block = Expression.Block(
            new[] { valueParam, notEmptyParam }, // Add a local variable.
            Expression.Assign(notEmpty, Expression.Constant(bool, typeof(bool))), // Assign a constant to the local variable: filterStringParam = filterString
            ...
        );
        
         var @delegate = Expression.Lambda<Func<StringBuilder, T, bool>>(block, stringParam).Compile();
         */
        public static bool SerializeStructProperty<T, TProp>(StringBuilder stringBuilder,  T t, string propertyName,
            Func<T, TProp> getter, Func<StringBuilder, TProp, bool> serializer) where TProp : struct
        {
            stringBuilder.Append('"').Append(propertyName).Append('"').Append(':');
            var value = getter(t);
            var notEmpty = serializer(stringBuilder, value);
            if (!notEmpty)
                stringBuilder.Length -= (propertyName.Length + 3);
            return notEmpty;
        }

        public static bool SerializeNStructPropertyHandleNull<T, TProp>(StringBuilder stringBuilder, T t, string propertyName,
            Func<T, TProp?> getter, Func<StringBuilder, TProp, bool> serializer, Func<StringBuilder, bool> nullSerializer) where TProp : struct
        {
            stringBuilder.Append('"').Append(propertyName).Append('"').Append(':');
            var nullableValue = getter(t);
            var notEmpty = (nullableValue.HasValue) ? serializer(stringBuilder, nullableValue.Value) : nullSerializer(stringBuilder);
            if (!notEmpty)
                stringBuilder.Length -= (propertyName.Length + 3);
            return notEmpty;
        }

        public static bool SerializeNStructProperty<T, TProp>(StringBuilder stringBuilder, T t, string propertyName,
            Func<T, TProp?> getter, Func<StringBuilder, TProp, bool> serializer) where TProp : struct
        {
            var notEmpty = false;
            var nullableValue = getter(t);
            if (nullableValue.HasValue)
            {
                stringBuilder.Append('"').Append(propertyName).Append('"').Append(':');
                notEmpty = serializer(stringBuilder, nullableValue.Value);
                if (!notEmpty)
                    stringBuilder.Length -= (propertyName.Length + 3);
            }
            return notEmpty;
        }
        #endregion

        #region Serialize Ref Property
        public static bool SerializeRefPropertyHandleNull<T, TProp>(StringBuilder stringBuilder, T t, string propertyName,
            Func<T, TProp> getter, Func<StringBuilder, TProp, bool> formatter, Func<StringBuilder, bool> nullFormatter) where TProp : class
        {
            stringBuilder.Append('"').Append(propertyName).Append('"').Append(':');
            var value = getter(t);
            var notEmpty = (value == null)? nullFormatter(stringBuilder): formatter(stringBuilder, value);
            if (!notEmpty)
                stringBuilder.Length -= (propertyName.Length + 3);
            return notEmpty;
        }

        public static bool SerializeRefProperty<T, TProp>(StringBuilder stringBuilder, T t, string propertyName,
            Func<T, TProp> getter, Func<StringBuilder, TProp, bool> formatter) where TProp : class
        {
            var notEmpty = false;
            var value = getter(t);
            if (value != null)
            {
                stringBuilder.Append('"').Append(propertyName).Append('"').Append(':');
                notEmpty = formatter(stringBuilder, value);
                if (!notEmpty)
                    stringBuilder.Length -= (propertyName.Length + 3);
            }
            return notEmpty;
        }
        #endregion

        #region Formatters: String, Bool, Struct, Ref, Text
        public static bool SerializeStringValue(StringBuilder stringBuilder, string text)
        {
            stringBuilder.Append('\"').Append(text).Append('\"');
            return true;
        }
        public static bool SerializeEscapeString(StringBuilder stringBuilder, string text)
        {
            stringBuilder.Append('\"').AppendJsonEscaped(text).Append('\"');
            return true;
        }
        public static bool SerializeBool(StringBuilder stringBuilder, bool b)
        {
            stringBuilder.Append(b ? "true" : "false");
            return true;
        }
        public static bool SerializeStruct<T>(StringBuilder stringBuilder, T t) where T : struct
        {
            stringBuilder.Append(t);
            return true;
        }
        public static bool SerializePrimitive<T>(StringBuilder stringBuilder, T t) where T : struct
        {
            stringBuilder.Append(Convert.ToString(t, CultureInfo.InvariantCulture));
            return true;
        }
        public static bool SerializeRefValue<T>(StringBuilder stringBuilder, T t) where T : class
        {
            stringBuilder.Append(t);
            return true;
        }
        public static bool SerializeEscapeTextVal<T>(StringBuilder stringBuilder, T t) where T : struct
        {
            stringBuilder.Append('"').AppendJsonEscaped(t.ToString()).Append('"');
            return true;
        }
        public static bool SerializeEscapingTextRef<T>(StringBuilder stringBuilder, T t) where T : class
        {
            stringBuilder.Append('"').AppendJsonEscaped(t.ToString()).Append('"');
            return true;
        }
        public static bool SerializeTextStructValue<T>(StringBuilder stringBuilder, T t) where T : struct
        {
            stringBuilder.Append('"').Append(t).Append('"');
            return true;
        }
        public static bool SerializeTextRefValue<T>(StringBuilder stringBuilder, T t) where T : class
        {
            stringBuilder.Append('"').Append(t).Append('"');
            return true;
        }
        #endregion

        public static bool SerializeNull(StringBuilder stringBuilder)
        {
            stringBuilder.Append("null");
            return true;
        }

        private static StringBuilder AppendJsonEscaped(this StringBuilder stringBuilder, string text)
        {
            foreach (char c in text)
            {
                switch (c)
                {
                    case '\\':
                        stringBuilder.Append("\\\\");
                        break;
                    case '\"':
                        stringBuilder.Append("\\\"");
                        break;
                    case '\n':
                        stringBuilder.Append("\\n");
                        break;
                    case '\r':
                        stringBuilder.Append("\\r");
                        break;
                    case '\t':
                        stringBuilder.Append("\\t");
                        break;
                    case '\b':
                        stringBuilder.Append("\\b");
                        break;
                    case '\f':
                        stringBuilder.Append("\\f");
                        break;
                    default:
                        stringBuilder.Append(c);
                        break;
                }
            }
            return stringBuilder;
        }
    }
}
