using Microsoft.EntityFrameworkCore;

namespace DashboardCode.Routines.Storage.EfCore
{
    public interface IDbContextOptionsBuilder
    {
        void Build(DbContextOptionsBuilder optionsBuilder);
    }
}