using System;
using System.Text;

namespace Vse.Routines.Json
{
    public delegate bool TypeHandler<T>(StringBuilder s, T t);
    public class JsonSerializerSettings
    {
        public TypeHandler<DateTime> DateTimeFormatter  { get; set; } = JsonValueStringBuilderExtensions.SerializeToIso8601WithSec;
        public TypeHandler<byte[]>   ByteArrayFormatter { get; set; } = JsonValueStringBuilderExtensions.SerializeBase64;
        public bool NullValueHandling { get; set; } = true;
        //public Dictionary<Type, Func<object, string>> TextTypeFormatters   { get; set; } = new Dictionary<Type, Func<object, string>>();
        //public Dictionary<Type, Func<object, string>> SymbolTypeFormatters { get; set; } = new Dictionary<Type, Func<object, string>>();
    }

    public class JsonSerializerSettings2//<T>
    {
        //public Dictionary<Include<T>, Func<object, string>> TextFormatters { get; set; } = new Dictionary<Include<T>, Func<object, string>>();
        //public Dictionary<Include<T>, Func<object, string>> NumberFormatters { get; set; } = new Dictionary<Include<T>, Func<object, string>>();

        public void AddFormatter<T, TLeaf>(Include<T> include, TypeHandler<TLeaf> formatter)
        {

        }
    }
}
