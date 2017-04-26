using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using static Vse.Routines.MemberExpressionExtensions;

namespace Vse.Routines.Text
{

    public class SerializerSetting<T>
    {
        public string DateTimeFormat { get; set; }
        public string FloatFormat { get; set; }
        public Dictionary<Type, Func<object, string>> TextTypeFormatters   { get; set; }
        public Dictionary<Type, Func<object, long>>   NumberTypeFormatters { get; set; }
        //public Dictionary<Include<T>, Func<object, string>> TextFormatters       { get; set; }
        //public Dictionary<Include<T>, Func<object, long>>   NumberFormatters     { get; set; }
    }

    public static class RoutineSerializer
    {
        //public interface IContract
        //{
        //    Action<object, StringBuilder> Serialize { get; set; }
        //}

        //public class PrimitiveContract : IContract
        //{
        //    public PropertyInfo PropertyInfo { get; set; }
        //    public Action<object, StringBuilder> Serialize { get; set; }
        //}

        //public class ObjectContract : IContract
        //{
        //    public TypeInfo                      TypeInfo   { get; set;}
        //    public List<PrimitiveContract>       Properties { get; set;}
        //    public Action<object, StringBuilder> Serialize  { get; set;}

            
        //}

        //public class ArrayContract : IContract
        //{
        //    public ObjectContract ObjectContract { get; set; }
        //    public Action<object, StringBuilder> Serialize { get; set; }
        //}

        public static string ToJson<T>(T t, Include<T> include, SerializerSetting<T> settings=null)
        {
            if (settings == null)
                settings = new SerializerSetting<T>();

            var serializingIncluding = new SerializingIncluding<T>();
            var includable = new Includable<T>(serializingIncluding);
            include.Invoke(includable);
            var nodes = serializingIncluding.Root;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("{");
            bool first = true;
            foreach (var n in nodes)
            {
                var p = n.func(t);
                if(p!=null)
                {
                    if (first)
                        first = false;
                    else
                        stringBuilder.Append(",");
                    stringBuilder.Append("\"").Append(n.PropertyName).Append("\"").Append(":");
                    SerializeRecursive(stringBuilder, n, n.IsEnumerable, p);
                }
            }
            stringBuilder.Append("}");

            //var contracts = new Dictionary<Type, ObjectContract>();
            //Func<Type, ObjectContract> getContract = (contractType) =>
            //{
            //    var contract = default(ObjectContract);
            //    if (!contracts.TryGetValue(contractType, out contract))
            //    {
            //        contract = new ObjectContract();
            //        contract.Properties = new List<PropertyInfo>();

            //        var properties = contractType.GetTypeInfo().DeclaredProperties;
            //        foreach (var propertyInfo in properties)
            //        {
            //            if (propertyInfo.CanRead && propertyInfo.CanWrite && propertyInfo.GetIndexParameters().Length == 0)
            //            {
            //                propertyInfo.PropertyType.GetTypeInfo().IsPrimitive
            //                if (systemTypes.Contains(propertyInfo.PropertyType))
            //                {
            //                    //var value = propertyInfo.GetValue(source, null);
            //                    //propertyInfo.SetValue(destination, value);
            //                }
            //                contract.Properties.Add(propertyInfo);
            //            }
            //        }
            //        contracts.Add(contractType, contract);
            //    }
            //    return contract;
            //};
            //var stringBuilder = new StringBuilder();
            //SerializeRecursive(stringBuilder, t, getContract);
            var text = stringBuilder.ToString();
            return text;
        }

        public static void SerializeRecursive(StringBuilder stringBuilder, SerializerNode node, bool enumerable, object o/*, Func<Type, ObjectContract> getContract*/)
        {
            if (o == null)
                stringBuilder.Append("null"); // possible only in arrays
            else if (enumerable)
            {
                SerializeEnumerable(stringBuilder, node, (IEnumerable)o/*, getContract*/);
            }
            //if (o is Array)
            //{
            //    SerializeArray(stringBuilder, (Array)o, getContract);
            //}
            //else if (o is IList)
            //{
            //    SerializeList(stringBuilder, (IList)o, getContract);
            //}
            //// note, string is IEnumerable, so may be "is not string is enough"
            //else if (o is IEnumerable && o.GetType().GetTypeInfo().ImplementedInterfaces.Any(t =>
            //        t.GetTypeInfo().IsGenericType &&
            //        t.GetGenericTypeDefinition() == typeof(ISet<>)))
            //{
            //    SerializeSet(stringBuilder, (IEnumerable)o, getContract);
            //}
            else
            {
                if (o is string)
                    WriteText(stringBuilder, EscapeJson((string)o));
                else if (o.GetType().GetTypeInfo().IsPrimitive)
                    WriteSymbol(stringBuilder, o.ToString());
                else
                {
                    SerializeObject(stringBuilder, node, o/*, getContract*/);
                }
            }
        }

        #region Writes
        public static void WriteText(StringBuilder stringBuilder, string text)
        {
            stringBuilder.Append('\"').Append(EscapeJson(text)).Append('\"');
        }

        public static void WriteSymbol(StringBuilder stringBuilder, string text)
        {
            stringBuilder.Append(text);
        }

        public static string EscapeJson(string text)
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

        #region lists
        private static void SerializeEnumerable(StringBuilder stringBuilder, SerializerNode node, IEnumerable enumerable/*, Func<Type, ObjectContract> getContract*/) // Set<> means HashSet<>
        {
            stringBuilder.Append('[');
            bool first = true;
            foreach (var i in enumerable)
            {
                if (!first)
                    stringBuilder.Append(',');
                else
                    first = false;
                if (i is null)
                    continue;
                SerializeRecursive(stringBuilder, node, false, i/*, getContract*/);
            }
            stringBuilder.Append(']');
        }

        //private static void SerializeSet(StringBuilder stringBuilder, IEnumerable enumerable, Func<Type, ObjectContract> getContract) // Set<> means HashSet<>
        //{
        //    stringBuilder.Append('[');
        //    bool first = true;
        //    foreach (var i in enumerable)
        //    {
        //        if (!first)
        //            stringBuilder.Append(',');
        //        else
        //            first = false;
        //        if (i is null)
        //            continue;
        //        SerializeRecursive(stringBuilder, i, getContract);
        //    }
        //    stringBuilder.Append(']');
        //}

        //private static void SerializeList(StringBuilder stringBuilder, IList list, Func<Type, ObjectContract> getContract)
        //{
        //    stringBuilder.Append('[');
        //    bool first = true;
        //    foreach (var i in list)
        //    {
        //        if (!first)
        //            stringBuilder.Append(',');
        //        else
        //            first = false;
        //        if (i is null)
        //            continue;
        //        SerializeRecursive(stringBuilder, i, getContract);
        //    }
        //    stringBuilder.Append(']');
        //}

        //private static void SerializeArray(StringBuilder stringBuilder, Array array, Func<Type, ObjectContract> getContract)
        //{
        //    stringBuilder.Append('[');
        //    bool first = true;
        //    foreach (var i in array)
        //    {
        //        if (!first)
        //            stringBuilder.Append(',');
        //        else
        //            first = false;
        //        if (i is null)
        //            continue;
        //        //if (i is string || i is char || i is char?)
        //        //    ;
        //        //if (i is int || i is int? || i is byte || i is byte? || i is long || i is long? || i is sbyte || i is sbyte? || i is short || i is short? || i is uint || i is uint? || i is ulong || i is ulong? || i is ushort || i is ushort?)
        //        //    ;
        //        //if (i is bool && i is bool?)
        //        //    ;
        //        //if (i is double || i is double? || i is float || i is float?)
        //        //    ;
        //        //if (i is decimal || i is decimal?)
        //        //    ;
        //        //if (i is decimal || i is decimal?)
        //        //    ;
        //        SerializeRecursive(stringBuilder, i, getContract);
        //    }
        //    stringBuilder.Append(']');
        //}
        #endregion

        public static void SerializeObject(StringBuilder stringBuilder, SerializerNode node, object t /*, Func<Type, ObjectContract> getContract*/)
        {
            stringBuilder.Append('{');
            bool first = true;
            foreach(var n in node.Children)
            {
                var p = n.func(t);
                if (p != null)
                {
                    if (first)
                        first = false;
                    else
                        stringBuilder.Append(',');
                    stringBuilder.Append("\"").Append(n.PropertyName).Append("\"").Append(":");
                    SerializeRecursive(stringBuilder, n, n.IsEnumerable, p/*, getContract*/);
                }
            }
            stringBuilder.Append('}');
        }
    }
}
