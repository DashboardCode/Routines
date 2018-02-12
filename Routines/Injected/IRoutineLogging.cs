using System;

namespace DashboardCode.Routines.Injected
{
    public interface IRoutineLogging
    {
        (Action<object>, Action) LogStart(object input);
    }
}
