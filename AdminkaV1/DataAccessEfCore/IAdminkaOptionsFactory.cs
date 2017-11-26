using Microsoft.EntityFrameworkCore;

namespace DashboardCode.AdminkaV1.DataAccessEfCore
{
    public interface IAdminkaOptionsFactory
    {
        DbContextOptions BuildOptions(DbContextOptionsBuilder optionsBuilder);
    }
}