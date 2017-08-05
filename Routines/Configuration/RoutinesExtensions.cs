using System.Collections.Generic;

namespace DashboardCode.Routines.Configuration
{
    public static class RoutinesExtensions
    {
        public static SpecifiableConfigurationContainer GetConfigurationContainer(this IEnumerable<IRoutineResolvable> routines, MemberTag memberTag)
        {
            var rangedRoutines = routines.RangedRoutines(memberTag);
            var configurationContainer = new SpecifiableConfigurationContainer(rangedRoutines);
            return configurationContainer;
        }

        public static SortedDictionary<int, IRoutineResolvable> RangedRoutines(this IEnumerable<IRoutineResolvable> routines, MemberTag memberTag)
        {
            var rangedRoutines = new Dictionary<int, IRoutineResolvable>();
            int rA = 0, rB = 1000, rC = 2000;
            foreach (IRoutineResolvable routine in routines)
            {
                if (
                    (routine.Namespace == memberTag.Namespace || routine.Namespace.IsNullOrWhiteSpaceOrAsterix())
                    && routine.Type == memberTag.Type
                    && routine.Member == memberTag.Member)
                {
                    if (!routine.For.IsNullOrWhiteSpaceOrAsterix())
                        rangedRoutines.Add(rA++, routine);
                    else
                        rangedRoutines.Add(999, routine);
                }
                else if (
                    (routine.Namespace == memberTag.Namespace || routine.Namespace.IsNullOrWhiteSpaceOrAsterix())
                    && routine.Type == memberTag.Type
                    && routine.Member.IsNullOrWhiteSpaceOrAsterix())
                {
                    if (!routine.For.IsNullOrWhiteSpaceOrAsterix())
                        rangedRoutines.Add(rB++, routine);
                    else
                        rangedRoutines.Add(1999, routine);
                }
                else if (
                    (routine.Namespace == memberTag.Namespace || routine.Namespace.IsNullOrWhiteSpaceOrAsterix())
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