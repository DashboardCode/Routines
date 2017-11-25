using Microsoft.Extensions.Logging;

namespace DashboardCode.Routines.Storage.EfCore
{
    public class StatefullLoggerFactory : LoggerFactory
    {
        public readonly StatefullLoggerProvider LoggerProvider;
        internal StatefullLoggerFactory() : this(new StatefullLoggerProvider()){}

        private StatefullLoggerFactory(StatefullLoggerProvider loggerProvider) : base(new[] { loggerProvider }) =>
            LoggerProvider = loggerProvider;
    }
}