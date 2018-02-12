using System;

namespace DashboardCode.Routines.Injected
{
    public interface IDataLogger
    {
        void Input(DateTime dt, object input);
        void Output(DateTime dt, object output);
    }
}