using System.Globalization;
using DashboardCode.AdminkaV1.Injected;
using DashboardCode.Routines;
using Microsoft.EntityFrameworkCore.Design;

namespace DashboardCode.AdminkaV1.DataAccessEfCore.SqlServer.InstallerApp
{
    public class AdminkaDbContextFactory : IDesignTimeDbContextFactory<AdminkaDbContext>
    {
        public AdminkaDbContext CreateDbContext(string[] args)
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