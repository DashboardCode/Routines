using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace Vse.Web
{
    public class CircularJsonConverter : JavaScriptConverter
    {
        #region StandardTypes
        public static readonly IReadOnlyCollection<Type> StandardTypes = new[]
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
        #endregion

        private readonly int recursionDepth = 1;
        private readonly int currentRecursionDepth =1;
        private readonly bool ignoreDuplicates;
        private readonly List<object> history;
        private readonly IEnumerable<Type> supportedTypes;
        private readonly IEnumerable<Type> simpleTypes;
        

        public CircularJsonConverter(IEnumerable<Type> supportedTypes, IEnumerable<Type> simpleTypes, int recursionDepth = 1, bool ignoreDuplicates = false):
            this(supportedTypes, simpleTypes, recursionDepth, ignoreDuplicates, 1 , new List<object>())
        {
        }

        private CircularJsonConverter(IEnumerable<Type> supportedTypes, IEnumerable<Type> simpleTypes, int recursionDepth, bool ignoreDuplicates, int currentRecursionDepth, List<object> history)
        {
            this.recursionDepth = recursionDepth;
            this.ignoreDuplicates = ignoreDuplicates;
            this.supportedTypes = supportedTypes;
            this.simpleTypes = simpleTypes;
            this.currentRecursionDepth = currentRecursionDepth;
            this.history = history;
        }

        public override IDictionary<string, object> Serialize(object o, JavaScriptSerializer serializer)
        {
            history.Add(o);
            var type = o.GetType();
            var standardTypesValues = new Dictionary<string, object>();
            var properties = type.GetProperties();

            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.CanRead && propertyInfo.GetIndexParameters().Length == 0)
                {
                    if (simpleTypes.Contains(propertyInfo.PropertyType))
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

            //foreach (var propertyInfo in properties)
            //{
            //    if (propertyInfo.CanRead && propertyInfo.GetIndexParameters().Length == 0)
            //    {
            //        if (supportedTypes.Contains(propertyInfo.PropertyType))
            //        {
            //            string propertyName = propertyInfo.Name;
            //            var value = propertyInfo.GetValue(o, null);
            //            standardTypesValues.Add(propertyName, value);
            //        }
            //        else
            //        {
            //            if (currentRecursionDepth <= recursionDepth)
            //            {
            //                string propertyName = propertyInfo.Name;
            //                var value = propertyInfo.GetValue(o, null);
            //                if (value != null)
            //                {
            //                    if (!ignoreDuplicates)
            //                    {
            //                        var dictionaryProperties = LayerUp(propertyName, value);
            //                        standardTypesValues.Add(propertyName, dictionaryProperties);
            //                    }
            //                    else if (!history.Contains(value))
            //                    {
            //                        var dictionaryProperties = LayerUp(propertyName, value);
            //                        standardTypesValues.Add(propertyName, dictionaryProperties);

            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            return standardTypesValues;
        }

        private IDictionary<string, object> LayerUp(string propertyName, object value)
        {
            var js = new CircularJsonConverter(supportedTypes, simpleTypes, recursionDepth - currentRecursionDepth, ignoreDuplicates, currentRecursionDepth, history);
            var jss = new JavaScriptSerializer();
            jss.RegisterConverters(new[] { new CircularJsonConverter(supportedTypes, simpleTypes) });
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
