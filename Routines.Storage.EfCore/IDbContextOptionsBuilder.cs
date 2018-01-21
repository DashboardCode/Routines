using Microsoft.EntityFrameworkCore;

namespace DashboardCode.Routines.Storage.EfCore
{
    public interface IDbContextOptionsFactory
    {
        void Create(DbContextOptionsBuilder optionsBuilder);
    }
}