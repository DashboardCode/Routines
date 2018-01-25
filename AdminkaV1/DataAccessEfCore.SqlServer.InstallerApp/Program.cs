using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace DashboardCode.AdminkaV1.Injected.NETStandard.EfCoreMigrationApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            var userContext = new UserContext("EFCoreMigrations", CultureInfo.CurrentCulture);
            var installerApplicationFactory = new SqlServerAdmikaConfigurationFacade(migrationAssembly: "DashboardCode.AdminkaV1.Injected.NETStandard.EfCoreMigrationApp");

            var routine = new AdminkaRoutineHandler(typeof(Program).Namespace, nameof(Program), nameof(Main), 
                userContext, 
                installerApplicationFactory, new { });

            routine.HandleDbContext(
                (dbContext) => {
                     dbContext.Database.Migrate();
                });
        }
    }
}
