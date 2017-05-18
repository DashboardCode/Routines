using System.Collections.Generic;

namespace Vse.Routines.Json
{
    public static class NExpJsonExtensions
    {
        //public static NExpJsonSerializer<T> BuildNExpJsonSerializer<T>(this Include<IEnumerable<T>> include, NExpJsonSerializerSettings settings = null)
        //{
        //    if (settings == null)
        //        settings = new NExpJsonSerializerSettings();
        //    var serializerNavigationExpressionParser = new SerializerNExpParser<T>();
        //    var includable = new Includable<T>(serializerNavigationExpressionParser);
        //    if (include != null)
        //        include.Invoke(includable);
        //    var serializerNode = serializerNavigationExpressionParser.Root;
        //    serializerNode.AppendLeafs();

        //    var serializer = new NExpJsonSerializer<T>(serializerNode, settings);
        //    return serializer;
        //}

        public static NExpJsonSerializer<T> BuildNExpJsonSerializer<T>(this Include<T> include, NExpJsonSerializerSettings settings = null)
        {
            if (settings == null)
                settings = new NExpJsonSerializerSettings();
            var parser = new SerializerNExpParser<T>();
            var includable = new Includable<T>(parser);
            if (include!=null)
                include.Invoke(includable);
            var serializerNode = parser.Root;
            serializerNode.AppendLeafs();


            var serializer = new NExpJsonSerializer<T>(serializerNode, settings);
            return serializer;
        }

        public static string SerializeJson<T>(this Include<T> include, T t, NExpJsonSerializer<T> serializer = null)
        {
            if (serializer == null)
                serializer = BuildNExpJsonSerializer(include);
            var json = serializer.Serialize(t);
            return json;
        }

        //public static string SerializeJsonAll<T>(this Include<IEnumerable<T>> include, IEnumerable<T> t, NExpJsonSerializer<T> serializer = null)
        //{
        //    //if (serializer == null)
        //    //    serializer = BuildNExpJsonSerializer(include);
        //    //var json = serializer.Serialize(t);
        //    return null; // json;
        //}

        public static string SerializeJson<T>(this Include<T> include, T t, NExpJsonSerializerSettings settings)
        {
            var serializer = BuildNExpJsonSerializer(include, settings);
            var json = serializer.Serialize(t);
            return json;
        }

        public static string SerializeJson<T>(this Include<T> include, IEnumerable<T> t, NExpJsonSerializerSettings settings)
        {
            var serializer = BuildNExpJsonSerializer(include, settings);
            var json = serializer.Serialize(t);
            return json;
        }

        public static string SerializeJson<T>(this Include<T> include, IEnumerable<IEnumerable<T>> t, NExpJsonSerializerSettings settings)
        {
            var serializer = BuildNExpJsonSerializer(include, settings);
            var json = serializer.Serialize(t);
            return json;
        }
    }
}