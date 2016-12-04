using Microsoft.EntityFrameworkCore;

namespace Vse.AdminkaV1.DataAccessEfCore.SqlServer
{
    public class SqlServerAdminkaOptionsFactory: IAdminkaOptionsFactory
    {
        readonly string connectionString;
        readonly string migrationAssembly;
        public SqlServerAdminkaOptionsFactory(string connectionString, string migrationAssembly)
        {
            this.connectionString = connectionString;
            this.migrationAssembly = migrationAssembly;
        }

        public DbContextOptions<TContext> CreateOptions<TContext>() where TContext : DbContext
        {
            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            // TODO: support sql express
            // optionsBuilder.UseSqlite("Filename=./blog.db");
            // TODO: move out migration assembly name
            optionsBuilder.UseSqlServer(
                connectionString,
                sqlServerDbContextOptionsBuilder => 
                    sqlServerDbContextOptionsBuilder.MigrationsAssembly(migrationAssembly)
                );
            var options = optionsBuilder.Options;
            return options;
        }
    }
}
