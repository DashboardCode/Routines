using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using DashboardCode.Routines.Storage.EfCore;

namespace DashboardCode.Routines.Storage.EfModelTest.EfCore
{
    public class MyDbContext : DbContext
    {
        readonly Action<DbContextOptionsBuilder> buildOptionsBuilder;
        readonly Action<string> verbose;
        public MyDbContext(Action<DbContextOptionsBuilder> buildOptionsBuilder, Action<string> verbose=null): base()
        {
            this.buildOptionsBuilder = buildOptionsBuilder;
            this.verbose = verbose;
        }

        private static string GetEntityTableName(string value)
        {
            return value + "s";
        }

        private static string GetMapTableName(string value)
        {
            return value + "Map";
        }

        #region DbSets
        public DbSet<ParentRecord> ParentRecords  { get; set; }
        public DbSet<ChildRecord> ChildRecords    { get; set; }
        public DbSet<HierarchyRecord> TestRecords { get; set; }
        public DbSet<TypeRecord> TypeRecords { get; set; }
        public DbSet<HierarchyRecord> HierarchyRecords { get; set; }
        public DbSet<ParentRecordHierarchyRecord> ParentRecordHierarchyRecords { get; set; }
        #endregion

        private Action returnLoggerFactory;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (verbose != null)
            {
                var loggerFactory = StatefullLoggerFactoryPool.Instance.Get(verbose, new LoggerProviderConfiguration { Enabled = true, CommandBuilderOnly = false });
                returnLoggerFactory = () => StatefullLoggerFactoryPool.Instance.Return(loggerFactory);
                optionsBuilder.UseLoggerFactory(loggerFactory);
            }
            buildOptionsBuilder(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Test Island
            string testIslandSchema = "tst";
            modelBuilder.Entity<ParentRecord>()
                .ToTable(GetEntityTableName(nameof(ParentRecord)), schema: testIslandSchema)
                .HasKey(e => e.ParentRecordId);
            // unique indexes
            modelBuilder.Entity<ParentRecord>()
                .HasIndex(e => e.FieldA).IsUnique();
            modelBuilder.Entity<ParentRecord>()
                .HasIndex(e => new { e.FieldB1, e.FieldB2 }).IsUnique();
            // unique constraints
            modelBuilder.Entity<ParentRecord>()
               .HasAlternateKey(e => e.FieldCA);
            modelBuilder.Entity<ParentRecord>()
                .HasAlternateKey(e => new { e.FieldCB1, e.FieldCB2 });

            modelBuilder.Entity<ChildRecord>()
                .ToTable(GetEntityTableName(nameof(ChildRecord)), schema: testIslandSchema)
                .HasKey(e => new { e.ParentRecordId, e.TypeRecordId });

            modelBuilder.Entity<ChildRecord>().HasOne(e => e.ParentRecord)
                .WithMany(e => e.ChildRecords)
                .HasForeignKey(e => e.ParentRecordId);

            modelBuilder.Entity<ChildRecord>()
                .HasOne(e => e.TypeRecord)
                .WithMany(e => e.ChildRecords)
                .HasForeignKey(e => e.TypeRecordId);

            modelBuilder.Entity<TypeRecord>()
                .ToTable(GetEntityTableName(nameof(TypeRecord)), schema: testIslandSchema)
                .HasKey(e => e.TestTypeRecordId);
            modelBuilder.Entity<TypeRecord>()
                .HasIndex(e => e.TypeRecordName).IsUnique();

            modelBuilder.Entity<HierarchyRecord>()
               .ToTable(GetEntityTableName(nameof(HierarchyRecord)), schema: testIslandSchema)
               .HasKey(e => e.HierarchyRecordId);

            modelBuilder.Entity<ParentRecordHierarchyRecord>()
                .ToTable(GetMapTableName(nameof(ParentRecordHierarchyRecord)), schema: testIslandSchema)
                .HasKey(e => new { e.ParentRecordId, e.HierarchyRecordId });

            modelBuilder.Entity<ParentRecordHierarchyRecord>()
                .HasOne(r => r.ParentRecord)
                .WithMany(pr => pr.ParentRecordHierarchyRecordMap)
                .HasForeignKey(r => r.ParentRecordId);

            modelBuilder.Entity<ParentRecordHierarchyRecord>()
                .HasOne(r => r.HierarchyRecord)
                .WithMany(hr => hr.ParentRecordHierarchyRecordMap)
                .HasForeignKey(r => r.HierarchyRecordId);
            #endregion
        }

        // NOTE: not threadsafe way of disposing
        public sealed override void Dispose()
        {
            returnLoggerFactory?.Invoke();
            returnLoggerFactory = null;
            base.Dispose();
        }

        public static Action<DbContextOptionsBuilder> BuildOptionsBuilder(string connectionString, bool inMemory = false)
        {
            return (optionsBuilder) =>
            {
                if (inMemory)
                    optionsBuilder.UseInMemoryDatabase(connectionString);
                else
                {
                    string assembly = typeof(MyDbContext).GetTypeInfo().Assembly.FullName;
                    optionsBuilder.UseSqlServer(
                            connectionString,
                            sqlServerDbContextOptionsBuilder => sqlServerDbContextOptionsBuilder.MigrationsAssembly(assembly)
                            );
                }
            };
        }
    }
}