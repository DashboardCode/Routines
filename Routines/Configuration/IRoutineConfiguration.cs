using System.Collections.Generic;

namespace DashboardCode.Routines.Configuration
{
    public interface IRoutineResolvable
    {
        string Namespace { get; }
        string Class     { get; }
        string Member    { get; }
        string For       { get; }
        IEnumerable<IResolvable> Resolvables { get; }
    }
}
