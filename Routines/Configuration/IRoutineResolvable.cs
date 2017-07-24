using System.Collections.Generic;

namespace DashboardCode.Routines.Configuration
{
    public interface IRoutineResolvable
    {
        string Namespace { get; }
        string Type      { get; }
        string Member    { get; }
        string For       { get; }
        IEnumerable<IResolvable> Resolvables { get; }
    }

    //public interface IRoutineResolvableRecord
    //{
    //    RoutineResolvableRecord GetRoutineResolvableRecord();
    //}

    //public struct RoutineResolvableRecord
    //{
    //    public string Namespace;
    //    public string Type;
    //    public string Member;
    //    public string For;
    //    public IResolvable[] Resolvables;
    //}
}
