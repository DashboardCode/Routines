using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace Vse.AdminkaV1.Injected
{
    public class CircularJsonConverter : JavaScriptConverter
    {
        private readonly int recursionDepth;
        private readonly int currentRecursionDepth;
        private readonly bool ignoreDuplicates;
        private readonly List<object> history;
        private readonly IEnumerable<Type> customTypes;
        private readonly IEnumerable<Type> systemTypes;

        public CircularJsonConverter(IEnumerable<Type> systemTypes, IEnumerable<Type> customTypes, int recursionDepth = 1, bool ignoreDuplicates = false):
            this(systemTypes, customTypes, recursionDepth, ignoreDuplicates, 1 , new List<object>())
        {
        }

        private CircularJsonConverter(IEnumerable<Type> systemTypes, IEnumerable<Type> customTypes, int recursionDepth, bool ignoreDuplicates, int currentRecursionDepth, List<object> history)
        {
            this.recursionDepth = recursionDepth;
            this.ignoreDuplicates = ignoreDuplicates;
            this.systemTypes = systemTypes;
            this.customTypes = customTypes;
            this.currentRecursionDepth = currentRecursionDepth;
            this.history = history;
        }

        public override IDictionary<string, object> Serialize(object o, JavaScriptSerializer serializer)
        {
            history.Add(o);
            var standardTypesValues = new Dictionary<string, object>();
            var type = o.GetType();
            var properties = type.GetProperties();
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.CanRead && propertyInfo.GetIndexParameters().Length == 0)
                {
                    if (systemTypes.Contains(propertyInfo.PropertyType))
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
            var js = new CircularJsonConverter(systemTypes, customTypes, recursionDepth - currentRecursionDepth, ignoreDuplicates, currentRecursionDepth, history);
            var jss = new JavaScriptSerializer();
            jss.RegisterConverters(new[] { new CircularJsonConverter(systemTypes, customTypes) });
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
                return customTypes;
            }
        }
    }
}
