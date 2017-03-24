using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Vse.AdminkaV1.Injected;

namespace Vse.AdminkaV1.DataAccessEfCore.SqlServer.InstallerApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            var userContext = new UserContext("EFCoreMigrations", CultureInfo.CurrentCulture);
            var installerConfiguration = new InstallerConfiguration();
            var routine = new AdminkaRoutine(typeof(Program).Namespace, nameof(Program), nameof(Main), userContext, installerConfiguration, new { });

            routine.Handle(
                (state, dataAccessServcies) => {
                    using (var dbContext = dataAccessServcies.CreateAdminkaDbContext())
                    {
                        dbContext.Database.Migrate();
                    }
                });
        }
    }
}
