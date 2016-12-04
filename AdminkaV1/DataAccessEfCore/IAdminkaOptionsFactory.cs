using Microsoft.EntityFrameworkCore;

namespace Vse.AdminkaV1.DataAccessEfCore
{
    public interface IAdminkaOptionsFactory
    {
        DbContextOptions<TContext> CreateOptions<TContext>() where TContext:DbContext;
    }
}
