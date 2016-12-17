using System;
using System.Collections.Generic;
using System.Linq;

namespace Vse.Routines.Configuration
{
    public class ConfigurationContainer : IConfigurationContainer
    {
        private readonly List<ResolvableElement> elements;
        public ConfigurationContainer(List<ResolvableElement> elements)
        {
            this.elements = elements;
        }

        public string ResolveSerialized<T>()
        {
            
            var type = typeof(T);
            var typeName = type.Name;
            var typeNamespace = type.Namespace;
            var @value = ResolveSerialized(typeNamespace, typeName);
            return @value;
        }

        public string ResolveSerialized(string typeNamespace, string typeName)
        {
            var @value = default(string);
            var config = elements.FirstOrDefault(e => (e.Namespace == typeNamespace || string.IsNullOrEmpty(e.Namespace)) && e.Type == typeName);
            if (config != null)
                @value = config.Value;
            return @value;
        }
        public T Resolve<T>() where T : IProgress<string>, new()
        {
            var @value = new T();
            var serialized = ResolveSerialized<T>();
            if (serialized != null)
                @value.Report(serialized);
            return @value;
        }
    }
}
