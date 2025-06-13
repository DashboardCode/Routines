using Microsoft.EntityFrameworkCore;

namespace AdminkaV1.StorageDom.EfCore
{
    public class ExcDbContext(DbContextOptions<ExcDbContext> options) : DbContext(options)
    {
        public DbSet<ExcTable> ExcTables { get; set; }
        public DbSet<ExcConnection> ExcConnections { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the ExcTable entity
            modelBuilder.Entity<ExcTable>(entity =>
            {
                entity.HasKey(e => e.ExcTableId); // Primary key
                entity.Property(e => e.ExcTableDescription).IsRequired(false);
                entity.Property(e => e.ExcTableXMeta).IsRequired(false);
                entity.Property(e => e.ExcTablePath).IsRequired(false);
                entity.Property(e => e.ExcTableFields).IsRequired(false);
                entity.Property(e => e.ExcConnectionId).IsRequired(true);
            });

            modelBuilder.Entity<ExcConnection>(entity =>
            {
                entity.HasKey(e => e.ExcConnectionId); // Primary key
                entity.Property(e => e.ExcConnectionCode).IsRequired(false);
                entity.Property(e => e.ExcConnectionName).IsRequired(false);
                entity.Property(e => e.ExcConnectionDescription).IsRequired(false);
                entity.Property(e => e.ExcConnectionType).IsRequired(false);
                entity.Property(e => e.ExcConnectionId).IsRequired(true);
                entity.Property(e => e.ExcConnectionString).IsRequired(false);
                entity.Property(e => e.ExcConnectionIsActive).IsRequired(true);
            });
        }
    }
}
