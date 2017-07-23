using System;

namespace DashboardCode.Routines.Injected
{
    public interface IVerboseLogging
    {
        void Input(DateTime dt, object input);
        void Output(DateTime dt, object output);
    }
}
