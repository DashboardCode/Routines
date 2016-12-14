using System.Globalization;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Vse.AdminkaV1.Injected;
using Vse.Routines;

namespace Vse.AdminkaV1.DataAccessEfCore.SqlServer.Installer
{
    public class AdminkaDbContextFactory : IDbContextFactory<AdminkaDbContext>
    {
        public AdminkaDbContext Create(DbContextFactoryOptions factoryOptions)
        {
            var userContext = new UserContext("EFCoreMigrations", CultureInfo.CurrentCulture);
            var routine = new AdminkaRoutine(new RoutineTag(this), userContext, new { });
            return routine.Handle(
                (container, dataAccessServcies) => {
                    var dbContext = dataAccessServcies.CreateAdminkaDbContext();
                    return dbContext;
                });
        }
    }
}