using System.Globalization;
using Microsoft.EntityFrameworkCore.Infrastructure;
using DashboardCode.AdminkaV1.Injected;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.DataAccessEfCore.SqlServer.InstallerApp
{
    public class AdminkaDbContextFactory : IDbContextFactory<AdminkaDbContext>
    {
        public AdminkaDbContext Create(DbContextFactoryOptions factoryOptions)
        {
            var userContext = new UserContext("EFCoreMigrations", CultureInfo.CurrentCulture);
            var installerApplicationFactory = new InstallerApplicationFactory();
            var routine = new AdminkaRoutineHandler(new MemberTag(this), userContext, installerApplicationFactory, new { });
            return routine.Handle(
                (container, dataAccessServcies) => {
                    var dbContext = dataAccessServcies.CreateAdminkaDbContext();
                    return dbContext;
                });
        }
    }
}