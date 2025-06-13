using Microsoft.EntityFrameworkCore;

namespace AdminkaV1.StorageDom.EfCore.SqlServer.MigrationsApp
{
    internal class Program
    {
        static void Main()
        {
            var excDbContext = StaticTools.CreateDbContext();
            var wasCreated = excDbContext.Database.EnsureCreated();
            if (wasCreated)
            {
                var store = new ExcConnectionsStore(StaticTools.CreateDbContext);
                store.CreateData();
            }
            else {
                // strange but EnsureCreated broke the migrations: doesn't create __EFMigrationsHistory table
                // So here is a workaround: we create __EFMigrationsHistory manually if it doesn't exist
                excDbContext.Database.ExecuteSqlRaw(
@"IF OBJECT_ID('__EFMigrationsHistory', 'U') IS NULL
BEGIN
CREATE TABLE[dbo].[__EFMigrationsHistory](
    [MigrationId][nvarchar](150) NOT NULL,
    [ProductVersion][nvarchar](32) NOT NULL,
 CONSTRAINT[PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED
(
    [MigrationId] ASC
) ON[PRIMARY]
) ON[PRIMARY]
END;");
                excDbContext.Database.ExecuteSqlRaw(
@"IF (NOT EXISTS(SELECT * FROM __EFMigrationsHistory))
BEGIN
	INSERT INTO [dbo].[__EFMigrationsHistory] (MigrationId, ProductVersion)
	VALUES ('20250526170949_Initial', '9.0.5')
END;", "20250526170949_Initial"); // latest migration id

                excDbContext.Database.Migrate();
            }
        }
    }
}
