using System;

namespace DashboardCode.Routines.Configuration
{
    public interface IConfigurationContainer
    {
        string ResolveSerialized<T>();
        string ResolveSerialized(string routineNamespace, string routineType);
        T Resolve<T>() where T : IProgress<string>, new();
    }

    public interface ISpecifiableConfigurationContainer : IConfigurationContainer
    {
        IConfigurationContainer Specify(string @for);
    }
}
