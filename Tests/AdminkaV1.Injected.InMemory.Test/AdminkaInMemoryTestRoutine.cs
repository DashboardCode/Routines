using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.Injected.InMemory.Test
{
    public class AdminkaInMemoryTestRoutine : AdminkaRoutine
    {
        public AdminkaInMemoryTestRoutine(RoutineGuid routineGuid, object input, string name = "adminka")
            : base(routineGuid, new UserContext("UnitTest"), ZoningSharedSourceManager.GetConfiguration(name), input)
        {
        }
    }
}
