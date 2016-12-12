using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vse.Web;

namespace Vse.Json.Test
{
    [TestClass]
    public class RecursiveSerializeTests
    {
        /// <summary>
        /// Note: it is still much more quicker then serialization using History (even when it produces huge json output and uses deeper recursion)
        /// </summary>
        [TestMethod]
        public void RecursiveJavaScriptSerializer()
        {
            var item = Item.CreateSample();
            //var x = Microsoft.AspNetCore.Mvc.Formatters.Json.

            var jss1 = new JavaScriptSerializer();
            //jss.RegisterConverters(new[] { new CircularScriptConverter(new[] { typeof(Item) }, 30, false) });
            jss1.RegisterConverters(new[] { new CircularJsonConverter(new[] { typeof(Item) }, CircularJsonConverter.StandardTypes,  50, false) });
            // ef types
            //types.AddRange(Assembly.GetAssembly(typeof(DbContext)).GetTypes());
            // model types
            // types.AddRange(Assembly.GetAssembly(typeof(BaseViewModel)).GetTypes());

            var json1 = jss1.Serialize(item);
            if (json1.Length<1000)
                throw new ApplicationException("History doesn't work. Case 0");

            var jss2 = new JavaScriptSerializer();
            jss2.RegisterConverters(new[] { new CircularJsonConverter(new[] { typeof(Item) }, CircularJsonConverter.StandardTypes, 50, true) });
            var json2 = jss2.Serialize(item);
            if (json2 != @"{""Number"":1,""Name"":""a"",""Child"":{""Number"":2,""Name"":""b"",""Child"":{""Number"":3,""Name"":""c""}}}")
                throw new ApplicationException("History doesn't work. Case 1");
        }

        [TestMethod]
        public void RecursiveJavaScriptSerializerWithHistory()
        {
            var item = Item.CreateSample();
            var jss2 = new JavaScriptSerializer();
            jss2.RegisterConverters(new[] { new CircularJsonConverter(new[] { typeof(Item) }, CircularJsonConverter.StandardTypes, 50, true) });
            var json2 = jss2.Serialize(item);
            if (json2 != @"{""Number"":1,""Name"":""a"",""Child"":{""Number"":2,""Name"":""b"",""Child"":{""Number"":3,""Name"":""c""}}}")
                throw new ApplicationException("History doesn't work. Case 1");
        }

        class Item
        {
            public static Item CreateSample()
            {
                var item1 = new Item { Name = "a", Number = 1 };
                var item2 = new Item { Name = "b", Number = 2 };
                item1.Child = item2; // circular reference 
                var item3 = new Item { Name = "c", Number = 3 };
                item2.Child = item3;
                item3.Child = item1;
                return item1;
            }
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
            

            readonly IEnumerable<Type> supportedTypes;

            public CircularScriptConverter(IEnumerable<Type> supportedTypes, int recursionDepth = 1, bool ignoreDuplicates = false)
            {
                this.recursionDepth = recursionDepth;
                this.supportedTypes = supportedTypes;
                this.ignoreDuplicates = ignoreDuplicates;
            }

            private CircularScriptConverter(IEnumerable<Type> supportedTypes, int recursionDepth, bool ignoreDuplicates, int currentRecursionDepth, List<object> history)
            {
                this.recursionDepth = recursionDepth;
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
                var properties = type.GetProperties();

                foreach (var propertyInfo in properties)
                {
                    if (propertyInfo.CanRead && propertyInfo.GetIndexParameters().Length == 0)
                    {
                        if (CircularJsonConverter.StandardTypes.Contains(propertyInfo.PropertyType))
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
                                        var dictionaryProperties = LayerUp(propertyName, value);
                                        standardTypesValues.Add(propertyName, dictionaryProperties);
                                    }
                                    else if (!history.Contains(value))
                                    {
                                        var dictionaryProperties = LayerUp(propertyName, value);
                                        standardTypesValues.Add(propertyName, dictionaryProperties);
                                        
                                    }
                                    
                                }
                            }
                        }
                    }
                }
                return standardTypesValues;
            }

            private IDictionary<string, object> LayerUp(string propertyName, object value)
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
