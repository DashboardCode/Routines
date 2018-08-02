using System.Globalization;
using Microsoft.EntityFrameworkCore;

using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.Injected.NETStandard.EfCoreMigrationApp
{
    public class AdminkaDbInstallGroup
    {
        public string Name { get; set; }
        public string[] Priveleges { get; set; }
    }

    public class Program
    {
        public readonly static ApplicationSettingsStandard ApplicationSettings = new ApplicationSettingsStandard();
        public static readonly string MigrationAssembly = typeof(Program).Assembly.GetName().Name;
        static void Main(string[] args)
        {
            var userContext = new UserContext("EFCoreMigrations", CultureInfo.CurrentCulture);

            // This application should execute all migrations, therefore dbContext should be setuped with migrations assembly
            var storageConfiguration = ApplicationSettings.CreateMigrationAdminkaStorageConfiguration(MigrationAssembly);
            var routine = new AdminkaRoutineHandler(
                storageConfiguration,
                ApplicationSettings.PerformanceCounters,
                ApplicationSettings.AuthenticationLogging,
                ApplicationSettings.ConfigurationContainerFactory,
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