using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DashboardCode.Routines.Storage.EfCore.Relational.SqlServer
{
    // TODO: support sql express
    // optionsBuilder.UseSqlite("Filename=./blog.db");
    public class SqlServerAdminkaOptionsFactory: IDbContextOptionsFactory
    {
        readonly string connectionString;
        readonly string migrationAssembly;
        readonly string migrationsHistoryTable;
        readonly string migrationsHistoryTableSchema;
        public SqlServerAdminkaOptionsFactory(
            string connectionString, 
            string migrationAssembly,
            string migrationsHistoryTable,
            string migrationsHistoryTableSchema="dbo")
        {
            this.connectionString = connectionString;
            this.migrationAssembly = migrationAssembly;
            this.migrationsHistoryTable = migrationsHistoryTable;
            this.migrationsHistoryTableSchema = migrationsHistoryTableSchema;
        }

        public void Create(DbContextOptionsBuilder optionsBuilder) 
        {
            if (migrationAssembly != null)
                optionsBuilder.UseSqlServer(connectionString, 
                    sqlServerDbContextOptionsBuilder => sqlServerDbContextOptionsBuilder
                    .MigrationsAssembly(migrationAssembly)
                    .MigrationsHistoryTable(migrationsHistoryTable, migrationsHistoryTableSchema));
            else
                optionsBuilder.UseSqlServer(connectionString,
                    sqlServerDbContextOptionsBuilder =>
                    sqlServerDbContextOptionsBuilder
                        .MigrationsHistoryTable(migrationsHistoryTable, migrationsHistoryTableSchema)
                        );

            var relationalOptions = RelationalOptionsExtension.Extract(optionsBuilder.Options);
            // TODO: Migrate those 2 lines to EF Core 2
            //relationalOptions.MigrationsHistoryTableName = "Migrations";
            //relationalOptions.MigrationsHistoryTableSchema = "ef";
        }
    }
}