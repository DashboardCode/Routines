using Microsoft.Extensions.Logging;
using System;

namespace Vse.Routines.Storage.EfCore
{
    
    public sealed class LoggerProvider : ILoggerProvider
    {
        public readonly LoggerProviderConfiguration loggerProviderConfiguration;
        public LoggerProvider(LoggerProviderConfiguration loggerProviderConfiguration)
        {
            this.loggerProviderConfiguration = loggerProviderConfiguration;
        }

        public Action<string> Verbose { get; set; }

        public ILogger CreateLogger(string categoryName)
        {
            return new Logger(categoryName,this);
        }

        public void Dispose()
        {

        }
    }

    class Logger : ILogger
    {
        readonly string categoryName;
        readonly LoggerProvider provider;
        public Logger(string categoryName, LoggerProvider provider)
        {
            this.categoryName = categoryName;
            this.provider = provider;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return provider.Verbose != null;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (provider.Verbose != null)
            {
                if (!provider.loggerProviderConfiguration.CommandBuilderOnly
                    ||
                    (provider.loggerProviderConfiguration.CommandBuilderOnly && categoryName == "Microsoft.EntityFrameworkCore.Storage.IRelationalCommandBuilderFactory"))
                {
                    var text = formatter(state, exception);
                    provider.Verbose(text);
                }
            }
        }
    }
}
