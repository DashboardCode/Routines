using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DashboardCode.Routines.Storage.EfCore.Relational.InMemory
{ 
    public class InMemoryAdminkaOptionsFactory: IDbContextOptionsFactory
    {
        readonly string databaseName;
        public InMemoryAdminkaOptionsFactory(string databaseName) =>
            this.databaseName = databaseName;

        public void Create(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName);
            optionsBuilder.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
        }
    }
}