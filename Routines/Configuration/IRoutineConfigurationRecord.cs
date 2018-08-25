using System.Collections.Generic;

namespace DashboardCode.Routines.Configuration
{
    /// <summary>
    /// Design compromise. I'll be mo happy to have there pure struct/ref types then interfaces, but this way configuration can be easy integrated with .NET Frameworsk System.Configuration API.
    /// </summary>
    public interface IRoutineConfigurationRecord<TSerialized>
    {
        string Namespace { get; }
        string Type      { get; }
        string Member    { get; }
        string For       { get; }
        IEnumerable<IResolvableConfigurationRecord<TSerialized>> Resolvables { get; }
    }
}