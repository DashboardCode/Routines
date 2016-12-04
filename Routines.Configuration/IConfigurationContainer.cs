using System;

namespace Vse.Routines.Configuration
{
    public interface IConfigurationContainer
    {
        string ResolveSerialized<T>();
        T Resolve<T>() where T : IProgress<string>, new();
    }
}
