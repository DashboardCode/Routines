using System;

namespace Vse.Routines.Configuration
{
    public interface IConfigurationContainer
    {
        string ResolveSerialized<T>();
        string ResolveSerialized(string routineNamespace, string routineType);
        T Resolve<T>() where T : IProgress<string>, new();
    }
}
