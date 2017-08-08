using System;
using Microsoft.Extensions.Logging;

namespace DashboardCode.Routines.Storage.EfCore
{
    class Logger : ILogger
    {
        const string sqlGenerationCategory = "Microsoft.EntityFrameworkCore.Storage.IRelationalCommandBuilderFactory";
        readonly string categoryName;
        readonly LoggerProvider provider;
        public Logger(string categoryName, LoggerProvider provider)
        {
            this.categoryName = categoryName;
            this.provider = provider;
        }

        public IDisposable BeginScope<TState>(TState state) =>
            null;

        public bool IsEnabled(LogLevel logLevel) =>
            provider.Verbose != null;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (provider.Verbose != null)
            {
                if (!provider.loggerProviderConfiguration.CommandBuilderOnly ||
                    (provider.loggerProviderConfiguration.CommandBuilderOnly && categoryName == sqlGenerationCategory))
                {
                    var text = formatter(state, exception);
                    provider.Verbose(text);
                }
            }
        }
    }
}
