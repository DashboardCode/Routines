using System;
using DashboardCode.Routines;
using DashboardCode.Routines.Storage.EfCore;

namespace DashboardCode.AdminkaV1.DataAccessEfCore.Services
{
    public class AdminkaDbContextFactory
    {
        readonly IAdminkaOptionsFactory optionsFactory;
        readonly Func<StatefullLoggerFactory> getLoggerFactory;
        readonly Action<StatefullLoggerFactory> returnLoggerFactory;

        public AdminkaDbContextFactory(
            IAdminkaOptionsFactory optionsFactory, 
            Routine<UserContext> state)
        {
            this.optionsFactory = optionsFactory;
            var loggerProviderConfiguration = state.Resolve<LoggerProviderConfiguration>();
            var verbose = state.Verbose;
            if (loggerProviderConfiguration.Enabled)
            {
                this.getLoggerFactory = () =>
                {
                    var l = StatefullLoggerFactoryPool.Instance.Get(verbose, loggerProviderConfiguration);
                    return l;
                };

                this.returnLoggerFactory = (l) =>
                {
                    StatefullLoggerFactoryPool.Instance.Return(l);
                };
            }
        }

        public AdminkaDbContext CreateAdminkaDbContext()
        {
            var dbContext = (getLoggerFactory == null)?
                new AdminkaDbContext(optionsFactory):
                new AdminkaDbContext(optionsFactory, getLoggerFactory, returnLoggerFactory);
            return dbContext;
        }
    }
}

