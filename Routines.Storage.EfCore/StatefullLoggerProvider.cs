using System;
using Microsoft.Extensions.Logging;

namespace DashboardCode.Routines.Storage.EfCore
{
    public class StatefullLoggerProvider : ILoggerProvider
    {
        internal LoggerProviderConfiguration loggerProviderConfiguration;
        internal Action<string> verbose;
        internal StatefullLoggerProvider() {}

        internal void Set(Action<string> verbose, LoggerProviderConfiguration loggerProviderConfiguration)
        {
            this.verbose = verbose;
            this.loggerProviderConfiguration = loggerProviderConfiguration;
        }

        public ILogger CreateLogger(string categoryName) =>
            new Logger(categoryName, this);

        void IDisposable.Dispose(){}
    }
}