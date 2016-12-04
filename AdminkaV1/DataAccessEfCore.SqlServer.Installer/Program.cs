using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Vse.AdminkaV1.Injected;

namespace Vse.AdminkaV1.DataAccessEfCore.SqlServer.Installer
{
    public class Program
    {
        static void Main(string[] args)
        {
            var userContext = new UserContext("EFCoreMigrations", CultureInfo.CurrentCulture);
            var routine = new AdminkaRoutine(typeof(Program).Namespace, nameof(Program), nameof(Main), userContext, new { });

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
