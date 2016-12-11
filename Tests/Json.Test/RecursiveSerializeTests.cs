using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Vse.Json.Test
{
    [TestClass]
    public class RecursiveSerializeTests
    {
        [TestMethod]
        public void RecursiveJavaScriptSerializer()
        {
            //var x = Microsoft.AspNetCore.Mvc.Formatters.Json.
            var item1 = new Item { Name="a", Number=1};
            var item2 = new Item { Name = "b", Number = 2 };
            item1.Child = item2; // circular reference 
            var item3 = new Item { Name = "c", Number = 3 };
            item2.Child = item3;
            item3.Child = item1;
            var jss = new JavaScriptSerializer();
            jss.RegisterConverters(new[] { new CircularScriptConverter(new[] { typeof(Item) }, 30, false) });
            // ef types
            //types.AddRange(Assembly.GetAssembly(typeof(DbContext)).GetTypes());
            // model types
            // types.AddRange(Assembly.GetAssembly(typeof(BaseViewModel)).GetTypes());

            var json = jss.Serialize(item1);
        }

        public class Item
        {
            public int Number { get; set; }
            public string Name { get; set; }
            public Item Child { get; set; }
        }

        public class CircularScriptConverter : JavaScriptConverter
        {
            private readonly int recursionDepth = 1;
            private readonly int currentRecursionDepth = 1;
            private readonly bool ignoreDuplicates;
            private readonly List<object> history = new List<object>();
            private readonly Type[] standardTypes = new[]
            {
                typeof(bool),
                typeof(bool?),
                typeof(byte),
                typeof(byte?),
                typeof(char),
                typeof(char?),
                typeof(decimal),
                typeof(decimal?),
                typeof(double),
                typeof(double?),
                typeof(float),
                typeof(float?),
                typeof(int),
                typeof(int?),
                typeof(long),
                typeof(long?),
                typeof(sbyte),
                typeof(sbyte?),
                typeof(short),
                typeof(short?),
                typeof(uint),
                typeof(uint?),
                typeof(ulong),
                typeof(ulong?),
                typeof(ushort),
                typeof(ushort?),
                typeof(string),
                typeof(DateTime),
                typeof(DateTime?),
                typeof(DateTimeOffset),
                typeof(DateTimeOffset?),
                typeof(Guid),
                typeof(Guid?),
                typeof(TimeSpan),
                typeof(TimeSpan?)
            };

            readonly IEnumerable<Type> supportedTypes;

            public CircularScriptConverter(IEnumerable<Type> supportedTypes, int recursionDepth = 1, bool ignoreDuplicates = false)
            {
                this.recursionDepth = recursionDepth;
                this.supportedTypes = supportedTypes;
                this.ignoreDuplicates = ignoreDuplicates;
            }

            private CircularScriptConverter(IEnumerable<Type> supportedTypes, int maxDepth, bool ignoreDuplicates, int currentRecursionDepth, List<object> history)
            {
                this.recursionDepth = maxDepth;
                this.ignoreDuplicates = ignoreDuplicates;
                this.supportedTypes = supportedTypes;
                this.currentRecursionDepth = currentRecursionDepth;
                this.history = history;
            }

            public override IDictionary<string, object> Serialize(object o, JavaScriptSerializer serializer)
            {
                history.Add(o);
                var type = o.GetType();
                var standardTypesValues = new Dictionary<string,object>();
                var properties = o.GetType().GetProperties();
                foreach (var propertyInfo in properties)
                {
                    if (propertyInfo.CanRead && propertyInfo.GetIndexParameters().Length == 0)
                    {
                        if (standardTypes.Contains(propertyInfo.PropertyType))
                        {
                            string propertyName = propertyInfo.Name;
                            var value = propertyInfo.GetValue(o, null);
                            standardTypesValues.Add(propertyName, value);
                        }
                        else
                        {
                            if (currentRecursionDepth <= recursionDepth)
                            {
                                string propertyName = propertyInfo.Name;
                                var value = propertyInfo.GetValue(o, null);
                                if (value != null)
                                {
                                    if (!ignoreDuplicates)
                                    {
                                        var dictionaryProperties = Up(propertyName, value);
                                        standardTypesValues.Add(propertyName, dictionaryProperties);
                                    }
                                    else if (!history.Contains(value))
                                    {
                                        var dictionaryProperties = Up(propertyName, value);
                                        standardTypesValues.Add(propertyName, dictionaryProperties);
                                        
                                    }
                                    
                                }
                            }
                        }
                    }
                }
                return standardTypesValues;
            }

            private IDictionary<string, object> Up(string propertyName, object value)
            {
                var js = new CircularScriptConverter(supportedTypes, recursionDepth - currentRecursionDepth, ignoreDuplicates, currentRecursionDepth, history);
                var jss = new JavaScriptSerializer();
                jss.RegisterConverters(new[] { new CircularScriptConverter(supportedTypes) });
                var dictionary = js.Serialize(value, jss);
                return dictionary;
            }

            public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
            {
                throw new NotImplementedException("This json serializer is used only for serialization");
            }

            public override IEnumerable<Type> SupportedTypes
            {
                get
                {
                    return supportedTypes;
                }
            }
        }
    }
}
