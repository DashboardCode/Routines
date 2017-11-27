using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using DashboardCode.Routines.Storage.EfCore;

namespace DashboardCode.AdminkaV1.DataAccessEfCore.InMemory
{
    public class InMemoryAdminkaOptionsBuilder: IDbContextOptionsBuilder
    {
        readonly string databaseName;
        public InMemoryAdminkaOptionsBuilder(string databaseName) =>
            this.databaseName = databaseName;

        public void Build(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName);
            optionsBuilder.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
        }
    }
}