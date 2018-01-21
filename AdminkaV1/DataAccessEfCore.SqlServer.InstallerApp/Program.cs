using System.Globalization;
using Microsoft.EntityFrameworkCore;

using DashboardCode.AdminkaV1.Injected;
using DashboardCode.AdminkaV1.Injected.NETStandard;

namespace DashboardCode.AdminkaV1.DataAccessEfCore.SqlServer.InstallerApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            var userContext = new UserContext("EFCoreMigrations", CultureInfo.CurrentCulture);
            var installerApplicationFactory = new SqlServerAdmikaConfigurationFacade(migrationAssembly: "DashboardCode.AdminkaV1.DataAccessEfCore.SqlServer.InstallerApp");

            var routine = new AdminkaRoutineHandler(typeof(Program).Namespace, nameof(Program), nameof(Main), 
                userContext, 
                installerApplicationFactory, new { });

            routine.HandleDbContext(
                (closure, dbContext) => {
                     dbContext.Database.Migrate();
                });
        }
    }
}
