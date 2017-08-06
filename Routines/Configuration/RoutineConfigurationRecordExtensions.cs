using System.Collections.Generic;

namespace DashboardCode.Routines.Configuration
{
    public static class RoutineConfigurationRecordExtensions
    {
        public static SortedDictionary<int, IRoutineConfigurationRecord> RangedRoutines(this IEnumerable<IRoutineConfigurationRecord> routines, MemberTag memberTag)
        {
            var rangedRoutines = new Dictionary<int, IRoutineConfigurationRecord>();
            int rA = 0, rB = 1000, rC = 2000;
            foreach (IRoutineConfigurationRecord routine in routines)
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
            return new SortedDictionary<int, IRoutineConfigurationRecord>(rangedRoutines);
        }
    }
}