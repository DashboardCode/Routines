using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DashboardCode.Routines.Configuration.Standard
{
    public class RoutineResolvable : IRoutineConfigurationRecord<IConfigurationSection>
    {
        public string Namespace { get; set; }
        public string Type { get; set; }
        public string Member { get; set; }
        public string For { get; set; }
        public List<Resolvable> Resolvables { get; set; }

        IEnumerable<IResolvableConfigurationRecord<IConfigurationSection>> IRoutineConfigurationRecord<IConfigurationSection>.Resolvables
        {
            get {
                return Resolvables;
            }
        }
    }
}