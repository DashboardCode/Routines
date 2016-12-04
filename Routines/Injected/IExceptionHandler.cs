using System;

namespace Vse.Routines.Injected
{
    public interface IExceptionHandler
    {
        void Handle(Action action, Action onFailure);
    }
}
