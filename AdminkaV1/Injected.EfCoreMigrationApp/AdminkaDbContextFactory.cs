using System.Globalization;
using Microsoft.EntityFrameworkCore.Design;

using DashboardCode.Routines;
using DashboardCode.AdminkaV1.DataAccessEfCore;

namespace DashboardCode.AdminkaV1.Injected.NETStandard.EfCoreMigrationApp
{
    /// <summary>
    /// Used by ps Add-Migration command. Therefore dbContext shold be setuped with migration's assembly name.
    /// </summary>
    public class AdminkaDbContextFactory : IDesignTimeDbContextFactory<AdminkaDbContext>
    {
        // TOSTU: how args can be used to configure e.g. current culture.
        public AdminkaDbContext CreateDbContext(string[] args)
        {
            var adminkaStorageConfiguration = Program.ApplicationSettings.CreateMigrationAdminkaStorageConfiguration(Program.MigrationAssembly);
            var adminkaDbContext =
                DataAccessEfCoreManager.CreateAdminkaDbContext(adminkaStorageConfiguration);
            return adminkaDbContext;
        }
    }
}