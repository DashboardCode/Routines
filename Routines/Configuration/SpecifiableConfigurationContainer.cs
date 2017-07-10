using System;
using System.Linq;
using System.Collections.Generic;

namespace Vse.Routines.Configuration
{
    public class SpecifiableConfigurationContainer : IConfigurationContainer
    {
        private readonly SortedDictionary<int, IRoutineResolvable> rangedRoutines;
        private readonly ConfigurationContainer configurationContainer;
        public SpecifiableConfigurationContainer (SortedDictionary<int, IRoutineResolvable> rangedRoutines)
        {
            this.rangedRoutines = rangedRoutines;
            var elements = new List<IResolvable>();
            foreach (var pair in rangedRoutines)
            {
                var routineElement = pair.Value;
                if (routineElement.For.IsNullOrWhiteSpaceOrAsterix())
                {
                    foreach (IResolvable r in routineElement.Resolvables)
                    {
                        if (!elements.Any(e => e.Type == r.Type && e.Namespace == r.Namespace))
                            elements.Add(r);
                    }
                }
            }
            configurationContainer = new ConfigurationContainer(elements);
        }

        public string ResolveSerialized<T>()
        {
            return configurationContainer.ResolveSerialized<T>();
        }

        public string ResolveSerialized(string routineNamespace, string routineType)
        {
            return configurationContainer.ResolveSerialized(routineNamespace, routineType);
        }

        public T Resolve<T>() where T : IProgress<string>, new()
        {
            return configurationContainer.Resolve<T>();
        }

        public IConfigurationContainer Specify(string @for)
        {
            var elements = new List<IResolvable>();
            foreach (var pair in rangedRoutines)
            {
                var routineElement = pair.Value;
                if (routineElement.For.IsNullOrWhiteSpaceOrAsterix() || routineElement.For== @for)
                {
                    foreach (IResolvable r in routineElement.Resolvables)
                    {
                        if (!elements.Any(e => e.Type == r.Type && e.Namespace == r.Namespace))
                            elements.Add(r);
                    }
                }
            }
            return new ConfigurationContainer(elements);
        }
    }
}