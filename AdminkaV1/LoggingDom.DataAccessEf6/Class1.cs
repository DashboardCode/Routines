using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;

namespace DashboardCode.AdminkaV1.LoggingDom.DataAccessEf6
{
    public class MyDbContext : DbContext
    {
        internal MyDbContext(string connectionStringName, Action<string> verbose)
            : base(connectionStringName)
        {
            Database.SetInitializer(new MyCreateDatabaseIfNotExists());

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

    public class MyCreateDatabaseIfNotExists : CreateDatabaseIfNotExists<MyDbContext>
    {
        protected override void Seed(MyDbContext context)
        {
            base.Seed(context);
        }
    }
}
