using System.Linq;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using DashboardCode.AdminkaV1.Injected;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using DashboardCode.Routines.Storage;

namespace DashboardCode.AdminkaV1.DataAccessEfCore.SqlServer.InstallerApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            var userContext = new UserContext("EFCoreMigrations", CultureInfo.CurrentCulture);
            var installerApplicationFactory = new InstallerApplicationFactory();

            var routine = new AdminkaRoutineHandler(typeof(Program).Namespace, nameof(Program), nameof(Main), 
                userContext, 
                installerApplicationFactory, new { });

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
