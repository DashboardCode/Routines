using Microsoft.EntityFrameworkCore;

namespace AdminkaV1.StorageDom.EfCore.SqlServer
{
    // Migrations in EF version 9.05 are broken
    // there are no way to create snapshot in MuigrationsApp project
    // (ignore dotnet ef migrations add ExcConnectionIsActive --project ../ConnectionsStorage.EfCore --output-dir ../ConnectionsStorage.EfCore.SqlServer.MigrationsApp/Migrations --startup-project ../ConnectionsStorage.EfCore.SqlServer.MigrationsApp)
    // Also there is no way to call funcionality of Add Migration manually: the github propose not compiling code (old version?).

    public class SqlServerExcDbContext(DbContextOptions<ExcDbContext> options) : ExcDbContext(options)
    {
    }
}
