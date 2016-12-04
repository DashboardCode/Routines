using System;
using System.Linq;
using System.Collections.Generic;

namespace Vse.Routines.Configuration
{
    public class SpecifiableConfigurationContainer : IConfigurationContainer
    {
        private readonly SortedDictionary<int, RoutineElement> rangedRoutines;
        private readonly ConfigurationContainer configurationContainer;
        public SpecifiableConfigurationContainer (SortedDictionary<int, RoutineElement> rangedRoutines)
        {
            this.rangedRoutines = rangedRoutines;
            var elements = new List<ResolvableElement>();
            foreach (var pair in rangedRoutines)
            {
                var routineElement = pair.Value;
                if (string.IsNullOrWhiteSpace(routineElement.For))
                {
                    foreach (ResolvableElement c in routineElement.Configs)
                    {
                        if (!elements.Any(e => e.Type == c.Type && e.Namespace == c.Namespace))
                            elements.Add(c);
                    }
                }
            }
            configurationContainer = new ConfigurationContainer(elements);
        }
        public string ResolveSerialized<T>()
        {
            return configurationContainer.ResolveSerialized<T>();
        }
        public T Resolve<T>() where T : IProgress<string>, new()
        {
            return configurationContainer.Resolve<T>();
        }

        public IConfigurationContainer Specify(string @for)
        {
            var elements = new List<ResolvableElement>();
            foreach (var pair in rangedRoutines)
            {
                var routineElement = pair.Value;
                if (string.IsNullOrWhiteSpace(routineElement.For) || routineElement.For== @for)
                {
                    foreach (ResolvableElement c in routineElement.Configs)
                    {
                        if (!elements.Any(e => e.Type == c.Type && e.Namespace == c.Namespace))
                            elements.Add(c);
                    }
                }
            }
            return new ConfigurationContainer(elements);
        }
    }
}