using Microsoft.EntityFrameworkCore.Design;
using DashboardCode.AdminkaV1.LoggingDom.DataAccessEfCore;

namespace DashboardCode.AdminkaV1.Injected.EfCoreMigrationApp
{
    /// <summary>
    /// Used by ps Add-Migration command. Therefore dbContext shold be setuped with migration's assembly name.
    /// </summary>
    public class AdminkaDbContextFactory : IDesignTimeDbContextFactory<LoggingDomDbContext>
    {
        // TOSTU: how args can be used to configure e.g. current culture.
        public LoggingDomDbContext CreateDbContext(string[] args)
        {
            var adminkaDbContext =
                LoggingDomDataAccessEfCoreManager.CreateLoggingDomDbContext(Program.MigrationApplicationSettings.AdminkaStorageConfiguration);
            return adminkaDbContext;
        }
    }
}