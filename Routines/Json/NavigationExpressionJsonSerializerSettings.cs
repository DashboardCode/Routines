using System;
using System.Collections.Generic;
using System.Text;

namespace Vse.Routines.Json
{
    public class NavigationExpressionJsonSerializerSettings<T>
    {
        public string DateTimeFormat { get; set; }
        public string FloatFormat { get; set; }
        public Dictionary<Type, Func<object, string>> TextTypeFormatters { get; set; }
        public Dictionary<Type, Func<object, long>> NumberTypeFormatters { get; set; }
        //public Dictionary<Include<T>, Func<object, string>> TextFormatters       { get; set; }
        //public Dictionary<Include<T>, Func<object, long>>   NumberFormatters     { get; set; }
    }
}
