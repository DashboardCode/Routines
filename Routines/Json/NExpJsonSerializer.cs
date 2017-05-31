using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Vse.Routines.Json
{
    public class NExpJsonSerializer<T>
    {
        private readonly SerializerBaseNode root;
        private readonly NExpJsonSerializerSettings settings;

        Action<StringBuilder, SerializerBaseNode, bool, NExpJsonSerializerSettings, object> serialize;

        //public void Compile()
        //{
        //    root.Serialize();
        //}

        public NExpJsonSerializer(
              SerializerBaseNode root, 
              NExpJsonSerializerSettings settings)
        {
            this.settings = settings;
            this.root     = root;

        }

        public string Serialize(IEnumerable<T> t)
        {
            return Serialize(t, true);
        }

        public string Serialize(IEnumerable<IEnumerable<T>> t)
        {
            return Serialize(t, true);
        }

        public string Serialize(T t)
        {
            var isEnumerable = (t is IEnumerable && !(t is string)); // TODO: AND check that T  is not a simple type
            return Serialize(t, isEnumerable);
        }

        #region private methods
        private string Serialize(object o, bool isEnumerable)
        {
            var stringBuilder = new StringBuilder();
            SerializeRecursive(stringBuilder, root, isEnumerable, settings, o);
            var json = stringBuilder.ToString();
            return json;
        }

        private static bool SerializeRecursive(StringBuilder stringBuilder, SerializerBaseNode node, bool isEnumerable, NExpJsonSerializerSettings settings, object o)
        {
            var @value = false;
            if (o == null)
            {
                WriteNull(stringBuilder); // possible only in arrays
                @value = true;
            }
            else if (isEnumerable && !node.IsByteArray)
            {
                @value = SerializeEnumerable(stringBuilder, node, (IEnumerable)o, settings /*, getContract*/);
            }
            else
            {
                if (node.IsLeaf)
                {
                    if (node.IsString)
                    {
                        WriteText(stringBuilder, (string)o);
                        @value = true;
                    }
                    else if (node.IsBoolean)
                    {
                        WriteBoolean(stringBuilder, (o is bool) ? ((bool)o) : ((bool?)o).Value);
                        @value = true;
                    }
                    else if (node.IsDateTime)
                    {
                        if (settings.DateTimeFormatter!=null)
                        {
                            var dateTime = (o is DateTime) ? ((DateTime)o) : ((DateTime?)o).Value;
                            @value = settings.DateTimeFormatter(stringBuilder, dateTime);
                        } 
                    }
                    else if (node.IsByteArray)
                    {
                        if (settings.ByteArrayFormatter != null)
                        {
                            var byteArray = (byte[])o;
                            @value = settings.ByteArrayFormatter(stringBuilder, byteArray);
                        }
                    }
                    else if (node.IsPrimitive || node.IsNPrimitive)
                    {
                        WriteSymbol(stringBuilder, o.ToString());
                        @value = true;
                    }
                    else if (node.IsDecimal || node.IsNDecimal)
                    {
                        @value = WriteSymbol(stringBuilder, o.ToString());
                    }
                    else if (node.IsSimpleText)
                    {
                        @value = WriteText(stringBuilder, o.ToString());
                    }
                    else if (node.IsSimpleSymbol)
                    {
                        @value = WriteSymbol(stringBuilder, o.ToString());
                    }
                    
                }
                else
                    @value = SerializeObject(stringBuilder, node, settings, o/*, getContract*/);
            }
            return @value;
        }

        private static bool SerializeRecursive2(StringBuilder stringBuilder, SerializerBaseNode node, bool isEnumerable, NExpJsonSerializerSettings settings, object o)
        {
            var @value = false;
            if (o == null)
            {
                @value = WriteNull(stringBuilder); // possible only in arrays
            }
            else if (isEnumerable && !node.IsByteArray)
            {
                @value = SerializeEnumerable(stringBuilder, node, (IEnumerable)o, settings);
            }
            else
            {
                if (node.IsLeaf)
                {
                    if (node.IsString)
                    {
                        @value = WriteText(stringBuilder, (string)o);
                    }
                    else if (node.IsBoolean)
                    {
                        @value = WriteBoolean(stringBuilder, (bool)o);
                    }
                    else if (node.IsNBoolean)
                    {
                        @value = WriteBoolean(stringBuilder, ((bool?)o).Value);
                    }
                    else if (node.IsDateTime)
                    {
                        if (settings.DateTimeFormatter != null)
                            @value = settings.DateTimeFormatter(stringBuilder, (DateTime)o);
                    }
                    else if (node.IsNDateTime)
                    {
                        if (settings.DateTimeFormatter != null)
                            @value = settings.DateTimeFormatter(stringBuilder, ((DateTime?)o).Value);
                    }
                    else if (node.IsByteArray)
                    {
                        if (settings.ByteArrayFormatter != null)
                            @value = settings.ByteArrayFormatter(stringBuilder, (byte[])o);
                    }
                    else if (node.IsDecimal || node.IsNDecimal)
                    {
                        @value = WriteSymbol(stringBuilder, o.ToString());
                    }
                    else if (node.IsPrimitive || node.IsNPrimitive)
                    {
                        @value = WriteSymbol(stringBuilder, o.ToString());
                    }
                    else if (node.IsSimpleText)
                    {
                        @value = WriteText(stringBuilder, o.ToString());
                    }
                    else if (node.IsSimpleSymbol)
                    {
                        @value = WriteSymbol(stringBuilder, o.ToString());
                    }
                    
                }
                else
                    @value = SerializeObject(stringBuilder, node, settings, o);
            }
            return @value;
        }

        private static bool SerializeEnumerable(StringBuilder stringBuilder, SerializerBaseNode node, IEnumerable enumerable, NExpJsonSerializerSettings settings)
        {
            var @value = false;
            stringBuilder.Append('[');

            var e = enumerable.GetEnumerator();
            bool moveNext = e.MoveNext();
            while (moveNext)
            {
                var i = e.Current;
                var isEnumerable = (node.IsRoot) ? ( i is IEnumerable && !(i is string)) : (false);
                var currentNotEmpty = SerializeRecursive(stringBuilder, node, isEnumerable, settings, i);
                moveNext = e.MoveNext();
                if (moveNext)
                    stringBuilder.Append(",");
                @value = @value || currentNotEmpty;
            }
            stringBuilder.Append(']'); 
            return @value;
        }

        private static bool SerializeObject(StringBuilder stringBuilder, SerializerBaseNode node, NExpJsonSerializerSettings settings, object t)
        {
            var @value = false;
            stringBuilder.Append('{');
            foreach(var child in node.Children)
            {
                var n = child.Value;
                var o = n.func(t);
                if (o == null && !settings.NullValueHandling)
                    continue;
                if (@value)
                    stringBuilder.Append(",");
                stringBuilder.Append("\"").Append(n.PropertyName).Append("\"").Append(":");
                var currentNotEmpty = SerializeRecursive(stringBuilder, n, n.IsEnumerable, settings, o);
                if (!currentNotEmpty)
                    stringBuilder.Length -=  (n.PropertyName.Length + (@value?4:3));
                @value = @value || currentNotEmpty;
            }
            if (node.IsRoot)
                @value = true;
            if (@value)
                stringBuilder.Append('}');
            else
                stringBuilder.Length--;
            return @value;
        }

        #region Writes
        private static bool WriteNull(StringBuilder stringBuilder)
        {
            stringBuilder.Append("null");
            return true;
        }

        private static bool WriteText(StringBuilder stringBuilder, string text)
        {
            stringBuilder.Append('\"').Append(EscapeJson(text)).Append('\"');
            return true;
        }

        private static bool WriteBoolean(StringBuilder stringBuilder, bool b)
        {
            stringBuilder.Append(b ? "true" : "false");
            return true;
        }

        private static bool WriteSymbol(StringBuilder stringBuilder, string text)
        {
            stringBuilder.Append(text);
            return true;
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
        
        #endregion
        #endregion
    }
}
