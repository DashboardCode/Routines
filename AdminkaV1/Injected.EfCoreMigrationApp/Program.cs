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
        public readonly static ApplicationSettings ApplicationSettings = InjectedManager.CreateApplicationSettingsStandard();
        public static readonly string MigrationAssembly = typeof(Program).Assembly.GetName().Name;
        static void Main(string[] args)
        {
            var migrationApplicationSettings = ApplicationSettings.CreateMigrationApplicationSettings(MigrationAssembly);

            var routine = new AdminkaAnonymousRoutineHandler(
                migrationApplicationSettings,
                "EFCoreMigrations",
                new { },
                correlationToken: System.Guid.NewGuid(),
                documentBuilder: null,
                controllerNamespace: typeof(Program).Namespace,
                controllerName: nameof(Program)
                );

            routine.Handle((container, closure) => container.ResolveAdminkaDbContextHandler().HandleDbContext(
                dbContext => {
                    dbContext.Database.Migrate();
                }));
        }
    }
}