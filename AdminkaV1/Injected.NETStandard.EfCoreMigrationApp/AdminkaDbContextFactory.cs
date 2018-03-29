using System.Globalization;
using Microsoft.EntityFrameworkCore.Design;

using DashboardCode.Routines;
using DashboardCode.AdminkaV1.DataAccessEfCore;
using DashboardCode.Routines.Configuration.NETStandard;
using DashboardCode.Routines.Configuration;

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
#if NETCOREAPP2_0
            var root = InjectedManager.ResolveConfigurationRoot();
            var configurationManagerLoader = new ConfigurationManagerLoader(root);
            var connectionStringMap = new ConnectionStringMap(root);
#else
            var configurationManagerLoader = new ConfigurationManagerLoader();
            var connectionStringMap = new ConnectionStringMap();
#endif
            var adminkaStorageConfiguration = InjectedManager.ResolveSqlServerAdminkaStorageConfiguration(connectionStringMap, migrationAssembly: Program.MigrationAssembly);
            var configurationFactory = new ConfigurationContainerFactory(configurationManagerLoader);
            var userContext = new UserContext("EFCoreMigrations", CultureInfo.CurrentCulture);
            var tag = new MemberTag(this);
            var adminkaDbContext =
                DataAccessEfCoreManager.CreateAdminkaDbContext(adminkaStorageConfiguration);
            return adminkaDbContext;
        }
    }
}