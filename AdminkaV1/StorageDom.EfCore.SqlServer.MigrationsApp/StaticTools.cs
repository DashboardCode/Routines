using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AdminkaV1.StorageDom.EfCore.SqlServer.MigrationsApp
{
    public static class StaticTools
    {
        public static IConfiguration GetConfiguration()
        {
            string pathToSqlServerConfig = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.sqlserver.json");

            // EF tools e.g. "dotnet ef migrations add $MigrationName" do not support linked appsettings.sqlserver.json file
            // so we need to read it manually
            if (!File.Exists(pathToSqlServerConfig))
            {
                pathToSqlServerConfig = Path.Combine(Directory.GetCurrentDirectory(), "..\\ConnectionsStorageWebApi\\appsettings.sqlserver.json");
                if (!File.Exists(pathToSqlServerConfig))
                {
                    throw new FileNotFoundException("appsettings.sqlserver.json not found");
                }
            }
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(pathToSqlServerConfig, optional: false, reloadOnChange: true)
                .AddUserSecrets("ConnectionsStorage.EfCore.SqlServer"); // override with personally configured connection string; see AddSecrets.ps1 script
            var builderConfiguration = builder.Build();
            return builderConfiguration;
        }

        public static SqlServerExcDbContext CreateDbContext()
        {
            var connectionString = GetConfiguration().GetConnectionString("DshbXConnection");
            var optionsBuilder =new DbContextOptionsBuilder<ExcDbContext>()
                .UseSqlServer(connectionString);
            var excDbContext = new SqlServerExcDbContext(optionsBuilder.Options);
            return excDbContext;
        }
    }
}
