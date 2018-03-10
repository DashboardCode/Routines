using System.Globalization;
using Microsoft.EntityFrameworkCore;

using DashboardCode.Routines;
using DashboardCode.Routines.Configuration.NETStandard;

namespace DashboardCode.AdminkaV1.Injected.NETStandard.EfCoreMigrationApp
{
    public class Program
    {
        public static readonly string MigrationAssembly = typeof(Program).Assembly.GetName().Name;
        static void Main(string[] args)
        {
            var userContext = new UserContext("EFCoreMigrations", CultureInfo.CurrentCulture);
            var configurationManagerLoader = new ConfigurationManagerLoader();
            
            // This application should execute all migrations, therefore dbContext should be setuped with migrations assembly
            var sqlServerAdmikaConfigurationFacade = new SqlServerAdmikaConfigurationFacade(
                configurationManagerLoader,
                migrationAssembly: MigrationAssembly
                );
            var configurationFactory = new ConfigurationContainerFactory(configurationManagerLoader);
            var routine = new AdminkaRoutineHandler(
                sqlServerAdmikaConfigurationFacade.ResolveAdminkaStorageConfiguration(),
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