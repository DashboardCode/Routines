using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DashboardCode.AdminkaV1.DataAccessEfCore.InMemory
{
    public class InMemoryAdminkaOptionsFactory: IAdminkaOptionsFactory
    {
        readonly string databaseName;
        public InMemoryAdminkaOptionsFactory(string databaseName) =>
            this.databaseName = databaseName;

        public DbContextOptions<TContext> CreateOptions<TContext>() where TContext : DbContext
        {
            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName);
            optionsBuilder.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            //optionsBuilder.ConfigureWarnings(x => x.Ignore(InMemoryEventId.));
            var options = optionsBuilder.Options;
            return options;
        }
    }
}
