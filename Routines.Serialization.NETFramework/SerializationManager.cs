using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.IO;
using System.Xml;
using Vse.Web;

namespace Vse.Routines.Serialization.NETFramework
{
    public static class SerializationManager
    {
        public static T DeserializeXml<T>(string xmlText, IEnumerable<Type> knownTypes = null)
        {
            using (var stringReader = new StringReader(xmlText))
            {
                using (var xmlTextReader = new XmlTextReader(stringReader))
                {
                    var serializer = new DataContractSerializer(typeof(T), knownTypes);
                    var o = serializer.ReadObject(xmlTextReader);
                    var t = (T)o;
                    return t;
                }
            }
        }
        public static string SerializeToXml(object o, Type rootType, IEnumerable<Type> knownTypes = null)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var xmlTextWriter = new XmlTextWriter(stringWriter))
                {
                    var serializer = new DataContractSerializer(rootType, knownTypes);
                    serializer.WriteObject(xmlTextWriter, o);
                    var text = stringWriter.ToString();
                    return text;
                }
            }
        }

        public static T DeserializeJson<T>(string json)
        {
            var serializer = new JavaScriptSerializer();
            var t = serializer.Deserialize<T>(json);
            return t;
        }
        public static string SerializeToJson(object o)
        {
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(o);
            return json;
        }
        public static string SerializeToJson(object o, int depth, bool ignoreDuplicates, IEnumerable<Type> types)
        {
            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new CircularJsonConverter(types, MemberExpressionExtensions.SystemTypes, 30, false) });
            var json = serializer.Serialize(o);
            return json;
        }
    }
}
