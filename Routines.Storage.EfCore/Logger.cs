using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace DashboardCode.Routines.Storage.EfCore
{
    class Logger : ILogger
    {
        readonly string categoryName;
        readonly StatefullLoggerProvider statefullLoggerProvider;
        public Logger(string categoryName, StatefullLoggerProvider statefullLoggerProvider)
        {
            this.categoryName = categoryName;
            this.statefullLoggerProvider = statefullLoggerProvider;
        }

        public IDisposable BeginScope<TState>(TState state) =>
            null;

        public bool IsEnabled(LogLevel logLevel) =>
            statefullLoggerProvider?.verbose != null;

        static readonly List<string> events = new List<string> {
                "Microsoft.EntityFrameworkCore.Database.Connection.ConnectionClosing",
                "Microsoft.EntityFrameworkCore.Database.Connection.ConnectionClosed",
                "Microsoft.EntityFrameworkCore.Database.Command.DataReaderDisposing",
                "Microsoft.EntityFrameworkCore.Database.Connection.ConnectionOpened",
                "Microsoft.EntityFrameworkCore.Database.Connection.ConnectionOpening",
                "Microsoft.EntityFrameworkCore.Infrastructure.ServiceProviderCreated",
                "Microsoft.EntityFrameworkCore.Infrastructure.ContextInitialized"
            };

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (statefullLoggerProvider?.verbose != null)
            {
                if (!statefullLoggerProvider.loggerProviderConfiguration.CommandBuilderOnly ||
                    (statefullLoggerProvider.loggerProviderConfiguration.CommandBuilderOnly && events.Contains(eventId.Name) ))
                {
                    var text = formatter(state, exception);
                    statefullLoggerProvider.verbose($"MESSAGE; categoryName={categoryName} eventId={eventId} logLevel={logLevel}" + Environment.NewLine + text);
                }
            }
        }
    }
}