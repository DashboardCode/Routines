using System;

namespace DashboardCode.Routines.Injected
{
    public interface IExceptionHandler
    {
        void Handle(Action action, Action onFailure);
    }
}
