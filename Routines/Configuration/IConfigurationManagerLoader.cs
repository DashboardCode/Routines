using System.Collections.Generic;

namespace DashboardCode.Routines.Configuration
{
    public interface IConfigurationManagerLoader
    {
        IEnumerable<IRoutineConfigurationRecord> GetGetRoutineConfigurationRecords();
    }
}