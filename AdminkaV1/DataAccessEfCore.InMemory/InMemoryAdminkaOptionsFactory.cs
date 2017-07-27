using Microsoft.EntityFrameworkCore;

namespace DashboardCode.AdminkaV1.DataAccessEfCore.InMemory
{
    public class InMemoryAdminkaOptionsFactory: IAdminkaOptionsFactory
    {
        readonly string connectionString;
        public InMemoryAdminkaOptionsFactory(string connectionString) =>
            this.connectionString = connectionString;

        public DbContextOptions<TContext> CreateOptions<TContext>() where TContext : DbContext
        {
            var databaseName = connectionString;
            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName);
            var options = optionsBuilder.Options;
            return options;
        }
    }
}
