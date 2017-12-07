using System.Collections.Generic;

namespace DashboardCode.Routines.Configuration
{
    public interface IConfigurationManagerLoader
    {
        IEnumerable<IRoutineConfigurationRecord> RoutineResolvables { get; }
        string GetConnectionString(string name);
    }
}