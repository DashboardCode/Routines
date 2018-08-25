using System.Collections.Generic;

namespace DashboardCode.Routines.Configuration
{
    public interface IConfigurationManagerLoader<TSerialized>
    {
        IEnumerable<IRoutineConfigurationRecord<TSerialized>> GetGetRoutineConfigurationRecords();
    }
}