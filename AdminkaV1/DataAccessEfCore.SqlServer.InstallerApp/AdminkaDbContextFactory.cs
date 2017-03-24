using System.Globalization;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Vse.AdminkaV1.Injected;
using Vse.Routines;
using Vse.Routines.Configuration;

namespace Vse.AdminkaV1.DataAccessEfCore.SqlServer.InstallerApp
{
    public class AdminkaDbContextFactory : IDbContextFactory<AdminkaDbContext>
    {
        public AdminkaDbContext Create(DbContextFactoryOptions factoryOptions)
        {
            var userContext = new UserContext("EFCoreMigrations", CultureInfo.CurrentCulture);
            var installerConfiguration = new InstallerConfiguration();
            var routine = new AdminkaRoutine(new RoutineTag(this), userContext, installerConfiguration, new { });
            return routine.Handle(
                (container, dataAccessServcies) => {
                    var dbContext = dataAccessServcies.CreateAdminkaDbContext();
                    return dbContext;
                });
        }
    }
}