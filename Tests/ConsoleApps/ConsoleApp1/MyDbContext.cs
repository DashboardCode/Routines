using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DashboardCode.Routines.Storage.EfModelTest.EfCore
{
    public class MyDbContext : DbContext
    {
        public static DbContextOptions Build(string connectionString, Action<string> verbose = null)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MyDbContext>();
            optionsBuilder.UseInMemoryDatabase(connectionString);
            if (verbose != null)
            {
                var loggerFactory = new LoggerFactory(new[] { new MyLoggerProvider(verbose) });
                optionsBuilder.UseLoggerFactory(loggerFactory);
            }
            return optionsBuilder.Options;
        }

        public MyDbContext(DbContextOptions options): base(options)
        {
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Test Island

            modelBuilder.Entity<ParentRecord>()
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
                .HasKey(e => new { e.ParentRecordId, e.TypeRecordId });

            modelBuilder.Entity<ChildRecord>().HasOne(e => e.ParentRecord)
                .WithMany(e => e.ChildRecords)
                .HasForeignKey(e => e.ParentRecordId);

            modelBuilder.Entity<ChildRecord>()
                .HasOne(e => e.TypeRecord)
                .WithMany(e => e.ChildRecords)
                .HasForeignKey(e => e.TypeRecordId);

            modelBuilder.Entity<TypeRecord>()
                .HasKey(e => e.TestTypeRecordId);
            modelBuilder.Entity<TypeRecord>()
                .HasIndex(e => e.TypeRecordName).IsUnique();

            modelBuilder.Entity<HierarchyRecord>()
               .HasKey(e => e.HierarchyRecordId);

            modelBuilder.Entity<ParentRecordHierarchyRecord>()
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

    }

    public class MyLoggerProvider : ILoggerProvider
    {
        internal Action<string> verbose;
        internal MyLoggerProvider(Action<string> verbose) {
            this.verbose = verbose;
        }

        public ILogger CreateLogger(string categoryName) =>
            new MyLogger(categoryName, this);

        void IDisposable.Dispose() { }
    }

    class MyLogger : ILogger
    {
        readonly string categoryName;
        readonly MyLoggerProvider statefullLoggerProvider;
        public MyLogger(string categoryName, MyLoggerProvider statefullLoggerProvider)
        {
            this.categoryName = categoryName;
            this.statefullLoggerProvider = statefullLoggerProvider;
        }

        public IDisposable BeginScope<TState>(TState state) =>
            null;

        public bool IsEnabled(LogLevel logLevel) =>
            statefullLoggerProvider?.verbose != null;


        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (statefullLoggerProvider?.verbose != null)
            {
                    var text = formatter(state, exception);
                    statefullLoggerProvider.verbose($"MESSAGE; categoryName={categoryName} eventId={eventId} logLevel={logLevel}" + Environment.NewLine + text);
            }
        }
    }
}