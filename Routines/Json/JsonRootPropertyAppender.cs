using System;
using System.Collections.Generic;
using System.Text;

namespace DashboardCode.Routines.Json
{
    public interface IJsonRootPropertyAppender
    {
        IJsonRootPropertyAppender AddStringProperty(string name, string value);
        IJsonRootPropertyAppender AddNumberProperty(string name, int value);
        //IJsonRootPropertyAppender AddNumberProperty(string name, int value);
        IJsonRootPropertyAppender AddNumberProperty(string name, bool value);
        IJsonRootPropertyAppender AddNumberProperty(string name, double value);
        IJsonRootPropertyAppender AddJsonProperty(string name, string value);
    }

    class JsonRootPropertyAppender : IJsonRootPropertyAppender
    {

        Dictionary<string, Action<StringBuilder>> properties = new Dictionary<string, Action<StringBuilder>>();
        public IJsonRootPropertyAppender AddJsonProperty(string name, string value)
        {
            properties.Add(name, (sb) => JsonValueStringBuilderExtensions.SerializeStringAsJsonLiteral(sb, value));
            return this;
        }

        public IJsonRootPropertyAppender AddNumberProperty(string name, int value)
        {
            properties.Add(name, (sb) => JsonValueStringBuilderExtensions.SerializeValueToString(sb, value));
            return this;
        }

        public IJsonRootPropertyAppender AddNumberProperty(string name, bool value)
        {
            properties.Add(name, (sb) => JsonValueStringBuilderExtensions.SerializeBool(sb, value));
            return this;
        }

        public IJsonRootPropertyAppender AddNumberProperty(string name, double value)
        {
            properties.Add(name, (sb) => JsonValueStringBuilderExtensions.SerializeValueToString(sb, value));
            return this;
        }

        public IJsonRootPropertyAppender AddStringProperty(string name, string value)
        {
            properties.Add(name, (sb) => JsonValueStringBuilderExtensions.SerializeEscapeString(sb, value));
            return this;
        }

        public void Build(StringBuilder stringBuilder)
        {
            foreach (var p in properties)
            {
                stringBuilder.Append('"').Append(p.Key).Append("\":");
                p.Value(stringBuilder);
                stringBuilder.Append(',');
            }
            if (properties.Count > 0)
                stringBuilder.Length -= 1;
        }
    }
}
