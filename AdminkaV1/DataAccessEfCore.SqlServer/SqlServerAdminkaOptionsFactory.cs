using DashboardCode.Routines.Storage.EfCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DashboardCode.AdminkaV1.DataAccessEfCore.SqlServer
{
    // TODO: support sql express
    // optionsBuilder.UseSqlite("Filename=./blog.db");
    public class SqlServerAdminkaOptionsFactory: IDbContextOptionsFactory
    {
        readonly string connectionString;
        readonly string migrationAssembly;
        public SqlServerAdminkaOptionsFactory(string connectionString, string migrationAssembly)
        {
            this.connectionString = connectionString;
            this.migrationAssembly = migrationAssembly;
        }

        public void Create(DbContextOptionsBuilder optionsBuilder) 
        {
            if (migrationAssembly != null)
                optionsBuilder.UseSqlServer(connectionString, 
                    sqlServerDbContextOptionsBuilder => sqlServerDbContextOptionsBuilder
                    .MigrationsAssembly(migrationAssembly)
                    .MigrationsHistoryTable("AdminkaDbContextMigrationHistory", "ef"));
            else
                optionsBuilder.UseSqlServer(connectionString,
                    sqlServerDbContextOptionsBuilder =>
                    sqlServerDbContextOptionsBuilder
                        .MigrationsHistoryTable("AdminkaDbContextMigrationHistory", "ef")
                        );

            var relationalOptions = RelationalOptionsExtension.Extract(optionsBuilder.Options);
            // TODO: Migrate those 2 lines to EF Core 2
            //relationalOptions.MigrationsHistoryTableName = "Migrations";
            //relationalOptions.MigrationsHistoryTableSchema = "ef";
        }
    }
}