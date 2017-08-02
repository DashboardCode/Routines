using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using DashboardCode.Routines;
using DashboardCode.Routines.Storage.EfCore;

namespace DashboardCode.AdminkaV1.DataAccessEfCore.Services
{
    public class AdminkaDbContextFactory
    {
        readonly IAdminkaOptionsFactory optionsFactory;
        readonly LoggerProvider loggerProvider;
        public AdminkaDbContextFactory(IAdminkaOptionsFactory optionsFactory, Routine<UserContext> state)
        {
            this.optionsFactory = optionsFactory;
            var loggerProviderConfiguration = state.Resolve<LoggerProviderConfiguration>();
            if (loggerProviderConfiguration.Enabled)
                loggerProvider = new LoggerProvider(loggerProviderConfiguration) { Verbose = state.Verbose };
        }
        public AdminkaDbContext CreateAdminkaDbContext()
        {
            var dbContext = new AdminkaDbContext(optionsFactory);
            if (loggerProvider != null)
            {
                var loggerFactory = dbContext.GetService<ILoggerFactory>();
                loggerFactory.AddProvider(loggerProvider);
            }
            return dbContext;
        }
    }
}
