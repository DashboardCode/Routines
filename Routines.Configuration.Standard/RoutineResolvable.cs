using System.Collections.Generic;

namespace DashboardCode.Routines.Configuration.Standard
{

    public class RoutineResolvable : IRoutineConfigurationRecord
    {
        public string Namespace { get; set; }
        public string Type { get; set; }
        public string Member { get; set; }
        public string For { get; set; }
        public List<Resolvable> Resolvables { get; set; }

        IEnumerable<IResolvableConfigurationRecord> IRoutineConfigurationRecord.Resolvables
        {
            get {
                return Resolvables;
            }
        }
    }
}
