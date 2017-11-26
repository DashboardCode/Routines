using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DashboardCode.AdminkaV1.DataAccessEfCore.SqlServer
{
    // TODO: support sql express
    // optionsBuilder.UseSqlite("Filename=./blog.db");
    public class SqlServerAdminkaOptionsFactory: IAdminkaOptionsFactory
    {
        readonly string connectionString;
        readonly string migrationAssembly;
        public SqlServerAdminkaOptionsFactory(string connectionString, string migrationAssembly)
        {
            this.connectionString = connectionString;
            this.migrationAssembly = migrationAssembly;
        }

        public DbContextOptions BuildOptions(DbContextOptionsBuilder optionsBuilder) 
        {
            if (migrationAssembly != null)
                optionsBuilder.UseSqlServer(connectionString, sqlServerDbContextOptionsBuilder => sqlServerDbContextOptionsBuilder.MigrationsAssembly(migrationAssembly));
            else
                optionsBuilder.UseSqlServer(connectionString);

            var relationalOptions = RelationalOptionsExtension.Extract(optionsBuilder.Options);
            // TODO: Migrate those 2 lines to EF Core 2
            //relationalOptions.MigrationsHistoryTableName = "Migrations";
            //relationalOptions.MigrationsHistoryTableSchema = "ef";

            var options = optionsBuilder.Options;
            return options;
        }
    }
}
