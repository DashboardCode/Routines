using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace DashboardCode.AdminkaV1.Injected
{
    public static class SerializationManager
    {
        #region xml
        public static T DeserializeXml<T>(string xmlText, IEnumerable<Type> knownTypes = null)
        {
            var stringReader = new StringReader(xmlText);
            using (var xmlTextReader = XmlReader.Create(stringReader))
            {
                var serializer = new DataContractSerializer(typeof(T), knownTypes);
                var o = serializer.ReadObject(xmlTextReader);
                var t = (T)o;
                return t;
            }
        }
        public static string SerializeToXml(object o, Type rootType, IEnumerable<Type> knownTypes = null)
        {
            var stringWriter = new StringWriter();
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                var serializer = new DataContractSerializer(rootType, knownTypes);
                serializer.WriteObject(xmlTextWriter, o);

            }
            var text = stringWriter.ToString();
            return text;
        }
        #endregion

        #region Json
        // TODO: try  System.Runtime.Serialization.Json
        public static T DeserializeJson<T>(string json)
        {
            var t = JsonConvert.DeserializeObject<T>(json);
            return t;
        }
        public static string SerializeToJson(object o)
        {
            var json = JsonConvert.SerializeObject(o);
            return json;
        }
        public static string SerializeToJson(object o, int depth, bool ignoreDuplicates, IEnumerable<Type> types)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };

            var json = JsonConvert.SerializeObject(o, jsonSerializerSettings);
            return json;
        }
        #endregion
    }
}
