using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using DashboardCode.Routines.Storage.EfCore;

namespace DashboardCode.AdminkaV1.LoggingDom.DataAccessEfCore
{
    public class LoggingDomDbContext : VerboseDbContext
    {
        public LoggingDomDbContext(Action<DbContextOptionsBuilder> buildOptionsBuilder, Action<string> verbose = null)
            : base(buildOptionsBuilder, verbose)
        {
        }

        #region DbSets
        public DbSet<VerboseRecord> VerboseRecords { get; set; }
        public DbSet<ActivityRecord> ActivityRecords { get; set; }
        private static string GetEntityTableName(string value)
        {
            return value + "s";
        }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            BuildModel(modelBuilder);
        }

        public static void BuildModel(ModelBuilder modelBuilder)
        {
            #region Logging Island
            string loggingIslandSchema = "log";
            modelBuilder.Entity<ActivityRecord>()
                .ToTable(GetEntityTableName(nameof(ActivityRecord)), schema: loggingIslandSchema)
                .HasKey(e => e.ActivityRecordId);
            modelBuilder.Entity<ActivityRecord>().Property(e => e.CorrelationToken).IsRequired();
            modelBuilder.Entity<ActivityRecord>().Property(e => e.Application).IsRequired().HasMaxLength(LengthConstants.GoodForKey);
            modelBuilder.Entity<ActivityRecord>().Property(e => e.FullActionName).IsRequired().HasMaxLength(LengthConstants.GoodForName);

            modelBuilder.Entity<VerboseRecord>()
                .ToTable(GetEntityTableName(nameof(VerboseRecord)), schema: loggingIslandSchema)
                .HasKey(e => e.ActivityRecordId);
            modelBuilder.Entity<VerboseRecord>().Property(e => e.CorrelationToken).IsRequired();
            modelBuilder.Entity<VerboseRecord>().Property(e => e.Application).IsRequired().HasMaxLength(LengthConstants.GoodForKey);
            modelBuilder.Entity<VerboseRecord>().Property(e => e.FullActionName).IsRequired().HasMaxLength(LengthConstants.GoodForName);
            modelBuilder.Entity<VerboseRecord>().Property(e => e.VerboseRecordTypeId).IsRequired().HasMaxLength(LengthConstants.GoodForKey);
            #endregion

        }
    }
}