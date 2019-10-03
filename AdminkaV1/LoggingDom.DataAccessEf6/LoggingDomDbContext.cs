using System;
using System.Data.Entity;

namespace DashboardCode.AdminkaV1.LoggingDom.DataAccessEf6
{
    public class LoggingDomDbContext : DbContext
    {
        internal LoggingDomDbContext(string connectionStringName, Action<string> verbose)
            : base(connectionStringName)
        {
            Database.SetInitializer(new LoggingDomCreateDatabaseIfNotExists());

            if (verbose != null)
                this.Database.Log += message =>
                    verbose(message);
        }

        public static void CreateModel(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActivityRecord>().HasKey(e => e.ActivityRecordId)
                .ToTable("ActivityRecords", "log");

            modelBuilder.Entity<VerboseRecord>().HasKey(e => e.ActivityRecordId)
                .ToTable("VerboseRecords", "log");
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            CreateModel(modelBuilder);
        }

        public DbSet<ActivityRecord> ActivityRecords { get; set; }
        public DbSet<VerboseRecord> VerboseRecords { get; set; }
        
    }

    public class LoggingDomCreateDatabaseIfNotExists : CreateDatabaseIfNotExists<LoggingDomDbContext>
    {
        protected override void Seed(LoggingDomDbContext context)
        {
            base.Seed(context);
        }
    }
}
