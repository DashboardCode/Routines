using System;
using DashboardCode.Routines;
using DashboardCode.Routines.Storage.EfCore;

namespace DashboardCode.AdminkaV1.DataAccessEfCore.Services
{
    public class AdminkaDbContextFactory
    {
        readonly IDbContextOptionsBuilder optionsFactory;
        readonly Action<string> verbose;
        public AdminkaDbContextFactory(
            IDbContextOptionsBuilder optionsFactory, 
            Routine<UserContext> state)
        {
            this.optionsFactory = optionsFactory;
            var loggerProviderConfiguration = state.Resolve<LoggerProviderConfiguration>();
            if (loggerProviderConfiguration.Enabled)
                verbose = state.Verbose;
        }

        public AdminkaDbContext CreateAdminkaDbContext()
        {
            var dbContext = new AdminkaDbContext((b)=>optionsFactory.Build(b), verbose);
            return dbContext;
        }
    }
}