using System;
using Microsoft.Extensions.Logging;

namespace DashboardCode.Routines.Storage.EfCore
{
    
    public sealed class LoggerProvider : ILoggerProvider
    {
        public readonly LoggerProviderConfiguration loggerProviderConfiguration;
        public LoggerProvider(LoggerProviderConfiguration loggerProviderConfiguration) =>
            this.loggerProviderConfiguration = loggerProviderConfiguration;

        public Action<string> Verbose { get; set; }

        public ILogger CreateLogger(string categoryName) =>
            new Logger(categoryName,this);

        void IDisposable.Dispose()
        {

        }
    }
}