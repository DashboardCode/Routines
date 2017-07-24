using System.Collections.Generic;

namespace DashboardCode.Routines.Configuration
{
    public static class RoutinesExtensions
    {
        public static SpecifiableConfigurationContainer GetConfigurationContainer(this IEnumerable<IRoutineResolvable> routines, string @namespace, string @class, string member)
        {
            var rangedRoutines = routines.RangedRoutines(@namespace, @class, member);
            var configurationContainer = new SpecifiableConfigurationContainer(rangedRoutines);
            return configurationContainer;
        }

        public static SortedDictionary<int, IRoutineResolvable> RangedRoutines(this IEnumerable<IRoutineResolvable> routines, string @namespace, string type, string member)
        {
            var rangedRoutines = new Dictionary<int, IRoutineResolvable>();
            int rA = 0, rB = 1000, rC = 2000;
            foreach (IRoutineResolvable routine in routines)
            {
                if (
                    (routine.Namespace == @namespace || routine.Namespace.IsNullOrWhiteSpaceOrAsterix())
                    && routine.Type == type
                    && routine.Member == member)
                {
                    if (!routine.For.IsNullOrWhiteSpaceOrAsterix())
                        rangedRoutines.Add(rA++, routine);
                    else
                        rangedRoutines.Add(999, routine);
                }
                else if (
                    (routine.Namespace == @namespace || routine.Namespace.IsNullOrWhiteSpaceOrAsterix())
                    && routine.Type == type
                    && routine.Member.IsNullOrWhiteSpaceOrAsterix())
                {
                    if (!routine.For.IsNullOrWhiteSpaceOrAsterix())
                        rangedRoutines.Add(rB++, routine);
                    else
                        rangedRoutines.Add(1999, routine);
                }
                else if (
                    (routine.Namespace == @namespace || routine.Namespace.IsNullOrWhiteSpaceOrAsterix())
                    && routine.Type.IsNullOrWhiteSpaceOrAsterix()
                    && routine.Member.IsNullOrWhiteSpaceOrAsterix())
                {
                    if (!routine.For.IsNullOrWhiteSpaceOrAsterix())
                        rangedRoutines.Add(rC++, routine);
                    else
                        rangedRoutines.Add(2999, routine);
                }
            }
            return new SortedDictionary<int, IRoutineResolvable>(rangedRoutines);
        }
    }
}