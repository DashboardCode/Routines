using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using DashboardCode.Routines.Storage.SqlServer;
using DashboardCode.Routines.Storage.EfCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;

namespace DashboardCode.Routines.Storage.EfModelTest.EfCore
{
    public class MyDbContext : DbContext
    {
        //public static void AddLoggerProvider(Action<DbContextOptionsBuilder<MyDbContext>> buildOptionsBuilder, MyLoggerProvider loggerProvider)
        //{
        //    var _serviceScope = ((IServiceScopeFactory)ServiceProviderServiceExtensions.GetRequiredService<IServiceScopeFactory>(ServiceProviderCache.Instance.GetOrAdd((IDbContextOptions)options, true))).CreateScope();

        //    IServiceProvider serviceProvider = _serviceScope.ServiceProvider;
        //    IDbContextServices service = (IDbContextServices)ServiceProviderServiceExtensions.GetService<IDbContextServices>(serviceProvider);

        //    //ServiceProviderServiceExtensions.GetService<ILoggerFactory>()
        //    using (var dbContex = new MyDbContext(buildOptionsBuilder))
        //        dbContex.GetService<ILoggerFactory>().AddProvider(loggerProvider);
        //}

        private static DbContextOptions<MyDbContext> CreateOptions(
            Action<DbContextOptionsBuilder<MyDbContext>> buildOptionsBuilder
            )
        {
            var optionsBuilder = new DbContextOptionsBuilder<MyDbContext>();
            buildOptionsBuilder(optionsBuilder);

            var options = optionsBuilder.Options;
            return options;
        }

        public MyDbContext(Action<DbContextOptionsBuilder<MyDbContext>> buildOptionsBuilder)
            : base(CreateOptions(buildOptionsBuilder))
        {
        }
        //public MyDbContext(Action<DbContextOptionsBuilder<MyDbContext>> buildOptionsBuilder, MyLoggerProvider loggerProvider)
        //    : base(CreateOptions(buildOptionsBuilder))
        //{
        //    this.GetService<ILoggerFactory>().AddProvider(loggerProvider);
        //}
        private static string GetEntityTableName(string value)
        {
            return value + "s";
        }
        private static string GetMapTableName(string value)
        {
            return value + "Map";
        }
        #region DbSets
        public DbSet<ParentRecord> ParentRecords { get; set; }
        public DbSet<ChildRecord> ChildRecords { get; set; }
        public DbSet<HierarchyRecord> TestRecords { get; set; }
        public DbSet<TypeRecord> TypeRecords { get; set; }
        public DbSet<HierarchyRecord> HierarchyRecords { get; set; }
        public DbSet<ParentRecordHierarchyRecord> ParentRecordHierarchyRecords { get; set; }
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
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

            #region ParentRecordHierarchyRecord


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
            #endregion
        }
    }
}