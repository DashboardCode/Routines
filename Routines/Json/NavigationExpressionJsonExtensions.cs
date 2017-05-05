namespace Vse.Routines.Json
{
    public static class NavigationExpressionJsonExtensions
    {
        public static NavigationExpressionJsonSerializer<T> BuildNavigationExpressionJsonSerializer<T>(this Include<T> include, NavigationExpressionJsonSerializerSettings<T> settings = null)
        {
            if (settings == null)
                settings = new NavigationExpressionJsonSerializerSettings<T>();
            var serializerNavigationExpressionParser = new SerializerNavigationExpressionParser<T>();
            var includable = new Includable<T>(serializerNavigationExpressionParser);
            if (include!=null)
            {
                include.Invoke(includable);
            }
            var navigationExpressionSerializedResult = serializerNavigationExpressionParser.Root;
            navigationExpressionSerializedResult.Append();

            var serializer = new NavigationExpressionJsonSerializer<T>(navigationExpressionSerializedResult, settings);
            return serializer;
        }

        public static string SerializeJson<T>(this Include<T> include, T t, NavigationExpressionJsonSerializer<T> serializer = null)
        {
            if (serializer == null)
                serializer = BuildNavigationExpressionJsonSerializer<T>(include);
            var json = serializer.Serialize(t);
            return json;
        }
    }
}
