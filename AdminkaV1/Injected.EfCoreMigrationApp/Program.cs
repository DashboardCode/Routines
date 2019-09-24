
using Microsoft.EntityFrameworkCore;

namespace DashboardCode.AdminkaV1.Injected.EfCoreMigrationApp
{

    public class Program
    {
        public readonly static ApplicationSettings ApplicationSettings = InjectedManager.CreateApplicationSettingsStandard();
        public static readonly string MigrationAssembly = typeof(Program).Assembly.GetName().Name;
        static void Main()
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

            routine.Handle((container, closure) => container.ResolveLoggingDomDbContextHandler().HandleDbContext(
                dbContext => {
                    dbContext.Database.Migrate();
                }));
        }
    }
}