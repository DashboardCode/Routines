using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Vse.Routines.Json
{
    public class NavigationExpressionJsonSerializer<T>
    {
        private readonly SerializerNode root;
        private readonly NavigationExpressionJsonSerializerSettings<T> settings;
        public NavigationExpressionJsonSerializer(
              SerializerNode root, 
              NavigationExpressionJsonSerializerSettings<T> settings)
        {
            this.settings = settings;
            this.root = root;
        }

        public string Serialize(IEnumerable<T> t)
        {
            return Serialize(t, true);
        }

        public string Serialize(T t)
        {
            return Serialize(t, false);
        }

        private string Serialize(object o, bool isEnumerable)
        {
            var stringBuilder = new StringBuilder();
            var nodes = root.Children;
            SerializeRecursive(stringBuilder, root, isEnumerable, o);
            var json = stringBuilder.ToString();
            return json;
        }
        private static bool SerializeRecursive(StringBuilder stringBuilder, SerializerNode node, bool isEnumerable, object o)
        {
            var @value = false;
            if (o == null)
            {
                WriteNull(stringBuilder); // possible only in arrays
                @value = true;
            }
            else if (isEnumerable)
            {
                @value = SerializeEnumerable(stringBuilder, node, (IEnumerable)o/*, getContract*/);
            }
            else
            {
                if (node.IsLeaf)
                {
                    if (node.IsString)
                        WriteText(stringBuilder, (string)o);
                    else if (node.IsBoolean)
                        WriteBoolean(stringBuilder, (o is bool)?((bool)o): ((bool?)o).Value);
                    else if (node.IsDateTime)
                        WriteDateTime(stringBuilder, (o is DateTime) ? ((DateTime)o) : ((DateTime?)o).Value);
                    else if (node.IsPrimitive)
                        WriteSymbol(stringBuilder, o.ToString());
                    else if (node.IsSimple)
                        WriteText(stringBuilder, o.ToString());
                    @value = true;
                }
                else
                    @value = SerializeObject(stringBuilder, node, o/*, getContract*/);
            }
            return @value;
        }

        #region Writes
        private static void WriteNull(StringBuilder stringBuilder)
        {
            stringBuilder.Append("null");
        }

        private static void WriteDateTime(StringBuilder stringBuilder, DateTime dateTime)
        {
            stringBuilder.Append('\"').Append(SerializeDateTimeWithMs(dateTime)).Append('\"');
        }

        private static void WriteText(StringBuilder stringBuilder, string text)
        {
            stringBuilder.Append('\"').Append(EscapeJson(text)).Append('\"');
        }

        private static void WriteBoolean(StringBuilder stringBuilder, bool b)
        {
            stringBuilder.Append(b ? "true" : "false");
        }

        private static void WriteSymbol(StringBuilder stringBuilder, string text)
        {
            stringBuilder.Append(text);
        }

        private static string EscapeJson(string text)
        {
            var stringBuilder = new StringBuilder(text.Length + (text.Length / 8));
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
            return stringBuilder.ToString();
        }
        /// <summary>
        /// ISO 8601 without "second fractions"
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private static string SerializeDateTimeUtcWithSec(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssK");
        }
        
        /// <summary>
        /// ISO 8601 with "second fractions", there with milliseconds
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private static string SerializeDateTimeUtcWithMs(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffK");
        }

        /// <summary>
        /// ISO 8601 without "second fractions"
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private static string SerializeDateTimeWithSec(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ssK");
        }

        /// <summary>
        /// ISO 8601 with "second fractions", there with milliseconds
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private static string SerializeDateTimeWithMs(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK");
        }
        #endregion

        private static bool SerializeEnumerable(StringBuilder stringBuilder, SerializerNode node, IEnumerable enumerable) 
        {
            var @value = false;
            stringBuilder.Append('[');
            bool first = true;
            foreach (var i in enumerable)
            {
                if (!first)
                    stringBuilder.Append(',');
                else
                    first = false;
                var notEmpty = SerializeRecursive(stringBuilder, node, false, i); 
                @value = @value || notEmpty;
            }
            stringBuilder.Append(']');
            return @value;
        }

        private static bool SerializeObject(StringBuilder stringBuilder, SerializerNode node, object t)
        {
            var @value = false;

            stringBuilder.Append('{');
            bool first = true;
            foreach (var child in node.Children)
            {
                var n = child.Value;
                var o = n.func(t);
                if (o != null)
                {
                    if (first)
                        first = false;
                    else
                        stringBuilder.Append(',');
                    stringBuilder.Append("\"").Append(n.PropertyName).Append("\"").Append(":");
                    var notEmpty = SerializeRecursive(stringBuilder, n, n.IsEnumerable, o);
                    @value = @value || notEmpty;
                }
            }
            stringBuilder.Append('}');
            return @value;
        }
    }
}
