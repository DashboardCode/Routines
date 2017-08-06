using System;
using System.Collections.Generic;
using System.Linq;

namespace DashboardCode.Routines.Configuration
{
    public class ConfigurationContainer 
    {
        private readonly List<IResolvableConfigurationRecord> elements;
        public ConfigurationContainer(IConfigurationManagerLoader loader, MemberTag memberTag)
        {
            var rangedRoutines = loader.RoutineResolvables.RangedRoutines(memberTag);
            elements = new List<IResolvableConfigurationRecord>();
            foreach (var pair in rangedRoutines)
            {
                var routineElement = pair.Value;
                if (routineElement.For.IsNullOrWhiteSpaceOrAsterix())
                {
                    foreach (var r in routineElement.Resolvables)
                    {
                        if (!elements.Any(e => e.Type == r.Type && e.Namespace == r.Namespace))
                            elements.Add(r);
                    }
                }
            }
        }

        public ConfigurationContainer(IConfigurationManagerLoader loader, MemberTag memberTag, string @for)
        {
            var rangedRoutines = loader.RoutineResolvables.RangedRoutines(memberTag);
            elements = new List<IResolvableConfigurationRecord>();
            foreach (var pair in rangedRoutines)
            {
                var routineElement = pair.Value;
                if (routineElement.For.IsNullOrWhiteSpaceOrAsterix() || routineElement.For == @for)
                {
                    foreach (var r in routineElement.Resolvables)
                    {
                        if (!elements.Any(e => e.Type == r.Type && e.Namespace == r.Namespace))
                            elements.Add(r);
                    }
                }
            }
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

        public T Resolve<T>() where T : IBuilder<string>, new()
        {
            var @value = new T();
            var serialized = ResolveSerialized<T>();
            if (serialized != null)
                @value.Build(serialized);
            return @value;
        }

        public T ResolveAlt<T>() where T : IProgress<string>, new()
        {
            var @value = new T();
            var serialized = ResolveSerialized<T>();
            if (serialized != null)
                @value.Report(serialized);
            return @value;
        }
    }
}
