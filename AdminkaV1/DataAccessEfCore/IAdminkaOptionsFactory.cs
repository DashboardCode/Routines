using Microsoft.EntityFrameworkCore;

namespace DashboardCode.AdminkaV1.DataAccessEfCore
{
    public interface IAdminkaOptionsFactory
    {
        DbContextOptions<TContext> CreateOptions<TContext>() where TContext:DbContext;
    }
}
