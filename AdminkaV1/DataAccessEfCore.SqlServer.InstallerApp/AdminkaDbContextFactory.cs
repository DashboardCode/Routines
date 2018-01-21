using System.Globalization;
using Microsoft.EntityFrameworkCore.Design;

using DashboardCode.Routines;
using DashboardCode.AdminkaV1.Injected;
using DashboardCode.AdminkaV1.Injected.NETStandard;

namespace DashboardCode.AdminkaV1.DataAccessEfCore.SqlServer.InstallerApp
{
    public class AdminkaDbContextFactory : IDesignTimeDbContextFactory<AdminkaDbContext>
    {
        public AdminkaDbContext CreateDbContext(string[] args)
        {
            var installerApplicationFactory = new SqlServerAdmikaConfigurationFacade(/*migrationAssembly: "DashboardCode.AdminkaV1.DataAccessEfCore.SqlServer.InstallerApp"*/);
            var userContext = new UserContext("EFCoreMigrations", CultureInfo.CurrentCulture);
            var tag = new MemberTag(this);
            var adminkaDbContext = InjectedManager.CreateAdminkaDbContext(installerApplicationFactory,  tag, userContext);
            return adminkaDbContext;
        }
    }
}