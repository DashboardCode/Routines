using System.Globalization;
using Microsoft.EntityFrameworkCore;

using DashboardCode.Routines;
using DashboardCode.Routines.Configuration.NETStandard;
using DashboardCode.Routines.Configuration;

namespace DashboardCode.AdminkaV1.Injected.NETStandard.EfCoreMigrationApp
{
    public class Program
    {
        public static readonly string MigrationAssembly = typeof(Program).Assembly.GetName().Name;
        static void Main(string[] args)
        {
            var userContext = new UserContext("EFCoreMigrations", CultureInfo.CurrentCulture);
#if NETCOREAPP2_0
            var root = InjectedManager.ResolveConfigurationRoot();
            var configurationManagerLoader = new ConfigurationManagerLoader(root);
            var connectionStringMap = new ConnectionStringMap(root);
#else
            var configurationManagerLoader = new ConfigurationManagerLoader();
            var connectionStringMap = new ConnectionStringMap();
#endif

            // This application should execute all migrations, therefore dbContext should be setuped with migrations assembly
            var adminkaStorageConfiguration = InjectedManager.ResolveSqlServerAdminkaStorageConfiguration(connectionStringMap, migrationAssembly: MigrationAssembly);
            var configurationFactory = new ConfigurationContainerFactory(configurationManagerLoader);
            var routine = new AdminkaRoutineHandler(
                adminkaStorageConfiguration,
                configurationFactory,
                new MemberTag(typeof(Program).Namespace, nameof(Program), nameof(Main)),
                userContext, 
                new { });

            routine.HandleDbContext(
                dbContext => {
                    dbContext.Database.Migrate();
                });
        }
    }
}