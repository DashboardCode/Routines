using System.Collections.Generic;

namespace DashboardCode.Routines.Configuration
{
    public interface IConfigurationManagerLoader
    {
        IEnumerable<IRoutineConfigurationRecord> RoutineResolvables { get; }
    }

    public interface IConnectionStringAccess
    {
        string GetConnectionString(string name);
    }
}