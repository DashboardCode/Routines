
using Microsoft.EntityFrameworkCore;

namespace AdminkaV1.StorageDom.EfCore
{
    public class ExcConnectionsStore(Func<ExcDbContext> excDbContextFactory) : IExcConnectionsStore
    {
        readonly Func<ExcDbContext> excDbContextFactory = excDbContextFactory;

        public async Task DeleteAsync(string key)
        {
            using var excDbContext = excDbContextFactory();
            var entity = await excDbContext.ExcConnections.FindAsync(key);
            if (entity != null)
            {
                excDbContext.ExcConnections.Remove(entity);
                await excDbContext.SaveChangesAsync();
            }
        }

        public async Task<ExcConnection?> GetAsync(string key)
        {
            using var excDbContext = excDbContextFactory();
            var entity = await excDbContext.ExcConnections.FindAsync(key);
            return entity;
        }

        public async Task SetAsync(ExcConnection excConnection)
        {
            using var excDbContext = excDbContextFactory();
            var exists = await excDbContext.ExcConnections
                .AnyAsync(c => c.ExcConnectionId == excConnection.ExcConnectionId);
            if (exists)
            {
                excDbContext.Update(excConnection);
            }
            else
            {
                excDbContext.Add(excConnection);
            }
            await excDbContext.SaveChangesAsync();
        }
    }
}
