using Microsoft.EntityFrameworkCore;

namespace DashboardCode.AdminkaV1.Injected.EfCoreMigrationApp
{

    public class Program
    {
        public readonly static ApplicationSettings ApplicationSettings = InjectedManager.CreateApplicationSettingsStandard(); // used for seeding
        public readonly static ApplicationSettings MigrationApplicationSettings = InjectedManager.CreateApplicationSettingsStandard(migrationAssembly: typeof(Program).Assembly.GetName().Name);
        static void Main()
        {
            var routine = new AdminkaAnonymousRoutineHandler(
                MigrationApplicationSettings,
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