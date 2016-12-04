using System;
using System.Collections.Generic;
using System.Configuration;

namespace Vse.Routines.Configuration
{
    public class RoutinesConfigurationSection : ConfigurationSection
    {
        private static readonly ConfigurationProperty routinesCollectionProperty 
            = new ConfigurationProperty("",typeof(RoutineElementCollection),null,ConfigurationPropertyOptions.IsDefaultCollection);
        private static readonly ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection{routinesCollectionProperty};
        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                return properties;
            }
        }
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public RoutineElementCollection Routines
        {
            get
            {
                return (RoutineElementCollection)base[routinesCollectionProperty];
            }
        }
        public SortedDictionary<int, RoutineElement> RangedRoutines(string @namespace, string @class, string member/*, string @for*/)
        {
                var rangedRoutines = new Dictionary<int, RoutineElement>();
                int rA = 0, rB=1000, rC=2000;
                foreach (RoutineElement routine in this.Routines)
                {
                    if (
                        (routine.Namespace == @namespace || string.IsNullOrEmpty(routine.Namespace))
                        && routine.Class == @class
                        && routine.Member == member)
                    {
                        if (!string.IsNullOrEmpty(routine.For))
                            rangedRoutines.Add(rA++, routine);
                        else 
                            rangedRoutines.Add(999, routine);
                    }

                    else if (
                        (routine.Namespace == @namespace || string.IsNullOrEmpty(routine.Namespace))
                        && routine.Class == @class
                        && string.IsNullOrEmpty(routine.Member))
                    {
                        if (!string.IsNullOrEmpty(routine.For))
                            rangedRoutines.Add(rB++, routine);
                        else
                            rangedRoutines.Add(1999, routine);
                    }

                    else if (
                        (routine.Namespace == @namespace || string.IsNullOrEmpty(routine.Namespace))
                        && string.IsNullOrEmpty(routine.Class)
                        && string.IsNullOrEmpty(routine.Member))
                    {
                        if (!string.IsNullOrEmpty(routine.For))
                            rangedRoutines.Add(rC++, routine);
                        else
                            rangedRoutines.Add(2999, routine);
                    }
                }
            return new SortedDictionary<int, RoutineElement>(rangedRoutines);
        }

        #region Debug
        public static DateTime StaticConstructedAt { get; } = DateTime.Now;
        public DateTime ConstructedAt { get; } = DateTime.Now;

        public override string ToString()
        {
            var text = $"{ConstructedAt}/{StaticConstructedAt} " +  base.ToString();
            return text;
        }
        #endregion
    }
}
