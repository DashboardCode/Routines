using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Vse.Routines;
using Vse.Routines.Storage.EfCore;

namespace Vse.AdminkaV1.DataAccessEfCore.Services
{
    public class DbContextFactory
    {
        readonly IAdminkaOptionsFactory optionsFactory;
        readonly LoggerProvider loggerProvider;
        public DbContextFactory(IAdminkaOptionsFactory optionsFactory, RoutineState<UserContext> state)
        {
            this.optionsFactory = optionsFactory;
            var loggerProviderConfiguration = state.Resolve<LoggerProviderConfiguration>();
            if (loggerProviderConfiguration.Enabled)
            {
                var loggerProvider = new LoggerProvider(loggerProviderConfiguration);
                loggerProvider.Verbose = state.Verbose;
                this.loggerProvider = loggerProvider;
            }
        }
        public AdminkaDbContext CreateDbContext()
        {
            var dbContext = new AdminkaDbContext(optionsFactory);
            if (this.loggerProvider != null)
            {
                var loggerFactory = dbContext.GetService<ILoggerFactory>();
                loggerFactory.AddProvider(loggerProvider);
            }
            return dbContext;
        }
    }
}
