using Microsoft.EntityFrameworkCore.Design;

namespace AdminkaV1.StorageDom.EfCore.SqlServer.MigrationsApp
{
    // This factory is used by EF Core tools to create the DbContext at design time
    public class ExcDbContextFactory : IDesignTimeDbContextFactory<SqlServerExcDbContext>
    {
        public SqlServerExcDbContext CreateDbContext(string[] args)
        {
            return StaticTools.CreateDbContext();
        }
    }
}
