using System;

namespace DashboardCode.Routines.Logging
{
    public interface ITraceDocumentBuilder
    {
        void AddProperty(DateTime dateTime, string message);
        void AddVerbose(DateTime dateTime, string message);
        void AddInput(DateTime dateTime, string message);
        void AddOutput(DateTime dateTime, string message);
        void AddException(DateTime dateTime, string message);
    }
}
