using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace DashboardCode.Routines.Storage.EfCore
{
    class Logger : ILogger
    {
        readonly string categoryName;
        readonly Action<string> verbose;
        readonly LoggerProviderConfiguration loggerProviderConfiguration;
        public Logger(string categoryName, Action<string> verbose, LoggerProviderConfiguration loggerProviderConfiguration)
        {
            this.categoryName = categoryName;
            this.verbose = verbose;
            this.loggerProviderConfiguration = loggerProviderConfiguration;
        }

        public IDisposable BeginScope<TState>(TState state) =>
            null;

        public bool IsEnabled(LogLevel logLevel) =>
            verbose != null;

        static readonly List<string> events = new List<string> { "Microsoft.EntityFrameworkCore.Database.Connection.ConnectionClosing",
                "Microsoft.EntityFrameworkCore.Database.Connection.ConnectionClosed",
                "Microsoft.EntityFrameworkCore.Database.Command.DataReaderDisposing",
                "Microsoft.EntityFrameworkCore.Database.Connection.ConnectionOpened",
                "Microsoft.EntityFrameworkCore.Database.Connection.ConnectionOpening",
                "Microsoft.EntityFrameworkCore.Infrastructure.ServiceProviderCreated",
                "Microsoft.EntityFrameworkCore.Infrastructure.ContextInitialized"
            };

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (verbose != null)
            {
                if (!loggerProviderConfiguration.CommandBuilderOnly ||
                    (loggerProviderConfiguration.CommandBuilderOnly && events.Contains(eventId.Name) ))
                {
                    var text = formatter(state, exception);
                    verbose($"MESSAGE; categoryName={categoryName} eventId={eventId} logLevel={logLevel}" + Environment.NewLine + text);
                }
            }
        }
    }
}