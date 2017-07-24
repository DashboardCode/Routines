using System.Collections.Generic;

namespace DashboardCode.Routines.Configuration.NETStandard
{
    public class RoutineResolvable : IRoutineResolvable
    {
        public string Namespace { get; set; }
        public string Type { get; set; }
        public string Member { get; set; }
        public string For { get; set; }
        public List<Resolvable> Resolvables { get; set; }
        IEnumerable<IResolvable> IRoutineResolvable.Resolvables { get { return Resolvables; } }
    }
}
