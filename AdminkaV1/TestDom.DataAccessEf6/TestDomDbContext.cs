using System;
using System.Data.Entity;

namespace DashboardCode.AdminkaV1.TestDom.DataAccessEf6
{
    public class TestDomDbContext : DbContext
    {
        internal TestDomDbContext(string connectionStringName, Action<string> verbose)
            : base(connectionStringName)
        {
            Database.SetInitializer(new LoggingDomCreateDatabaseIfNotExists());

            if (verbose != null)
                this.Database.Log += message =>
                    verbose(message);
        }

        public static void CreateModel(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<TypeRecord>().HasKey(e => e.TestTypeRecordId)
            //    .ToTable("TypeRecords", "tst");

            //modelBuilder.Entity<ChildRecord>().HasKey(e => e.ActivityRecordId)
            //    .ToTable("ChildRecords", "tst");

            SetupVersioned<TypeRecord>(modelBuilder);
            SetupVersioned<ChildRecord>(modelBuilder);
            SetupVersioned<HierarchyRecord>(modelBuilder);
            SetupVersioned<ParentRecordHierarchyRecord>(modelBuilder);
            SetupVersioned<ParentRecord>(modelBuilder);
            
            string testIslandSchema = "tst";

            modelBuilder.Entity<ParentRecord>()
                .ToTable("ParentRecords", testIslandSchema)
                .HasKey(e => e.ParentRecordId);
            modelBuilder.Entity<ParentRecord>().Property(e => e.FieldA).IsRequired().HasMaxLength(LengthConstants.GoodForFormLabel);
            modelBuilder.Entity<ParentRecord>().Property(e => e.FieldB1).IsRequired().HasMaxLength(LengthConstants.GoodForFormLabel);
            modelBuilder.Entity<ParentRecord>().Property(e => e.FieldB2).IsRequired().HasMaxLength(LengthConstants.GoodForFormLabel);
            modelBuilder.Entity<ParentRecord>().Property(e => e.FieldCA).IsRequired().HasMaxLength(LengthConstants.GoodForFormLabel);
            modelBuilder.Entity<ParentRecord>().Property(e => e.FieldCB1).IsRequired().HasMaxLength(LengthConstants.GoodForFormLabel);
            modelBuilder.Entity<ParentRecord>().Property(e => e.FieldCB2).IsRequired().HasMaxLength(LengthConstants.GoodForFormLabel);
            // unique indexes
            modelBuilder.Entity<ParentRecord>()
                .HasIndex(e => e.FieldA).IsUnique();
            modelBuilder.Entity<ParentRecord>()
                .HasIndex(e => new { e.FieldB1, e.FieldB2 }).IsUnique();
            // unique constraints
            modelBuilder.Entity<ParentRecord>()
               .HasKey(e => e.FieldCA);
            modelBuilder.Entity<ParentRecord>()
               .HasIndex(x => new { x.FieldCB1, x.FieldCB2 }).IsUnique();

            modelBuilder.Entity<ChildRecord>().Property(e => e.ParentRecordId);
            modelBuilder.Entity<ChildRecord>().Property(e => e.TypeRecordId).HasMaxLength(LengthConstants.GoodForKey);
            modelBuilder.Entity<ChildRecord>().Property(e => e.XmlField1).HasColumnType("xml");
            modelBuilder.Entity<ChildRecord>().Property(e => e.XmlField2).HasColumnType("xml");
            modelBuilder.Entity<ChildRecord>()
                .ToTable("ChildRecords", testIslandSchema)
                .HasKey(e => new { e.ParentRecordId, e.TypeRecordId });

            modelBuilder.Entity<ChildRecord>()
                .HasRequired(e => e.ParentRecord)
                .WithMany(e => e.ChildRecords)
                .HasForeignKey(e => e.ParentRecordId);
            //HasColumnType("xml");
            modelBuilder.Entity<ChildRecord>()
                .HasRequired(e => e.TypeRecord)
                .WithMany(e => e.ChildRecords)
                .HasForeignKey(e => e.TypeRecordId);

            modelBuilder.Entity<TypeRecord>()
                .ToTable("TypeRecords", testIslandSchema)
                .HasKey(e => e.TestTypeRecordId);
            modelBuilder.Entity<TypeRecord>()
                .Property(e => e.TestTypeRecordId).HasMaxLength(LengthConstants.GoodForKey);
            modelBuilder.Entity<TypeRecord>().Property(e => e.TypeRecordName).IsRequired().HasMaxLength(LengthConstants.GoodForName);
            modelBuilder.Entity<TypeRecord>()
                .HasIndex(e => e.TypeRecordName).IsUnique();

            modelBuilder.Entity<HierarchyRecord>().Property(e => e.HierarchyRecordTitle).IsRequired().HasMaxLength(LengthConstants.GoodForLongTitle);
            modelBuilder.Entity<HierarchyRecord>()
               .ToTable("HierarchyRecords", testIslandSchema)
               .HasKey(e => e.HierarchyRecordId);

            #region ParentRecordHierarchyRecord

            modelBuilder.Entity<ParentRecordHierarchyRecord>()
                .ToTable("ParentRecordHierarchyRecordMap", testIslandSchema)
                .HasKey(e => new { e.ParentRecordId, e.HierarchyRecordId });

            modelBuilder.Entity<ParentRecordHierarchyRecord>()
                .HasRequired(r => r.ParentRecord)
                .WithMany(pr => pr.ParentRecordHierarchyRecordMap)
                .HasForeignKey(r => r.ParentRecordId);

            modelBuilder.Entity<ParentRecordHierarchyRecord>()
                .HasRequired(r => r.HierarchyRecord)
                .WithMany(hr => hr.ParentRecordHierarchyRecordMap)
                .HasForeignKey(r => r.HierarchyRecordId);
            #endregion
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            CreateModel(modelBuilder);
        }

        public DbSet<ParentRecord> ParentRecords { get; set; }
        public DbSet<ChildRecord> ChildRecords { get; set; }
        public DbSet<HierarchyRecord> HierarchyRecords { get; set; }
        public DbSet<ParentRecordHierarchyRecord> ParentRecordHierarchyRecords { get; set; }
        public DbSet<TypeRecord> TypeRecords { get; set; }

        private static void SetupVersioned<T>(DbModelBuilder builder) where T : class, IVersioned
        {
            builder.Entity<T>().Property(e => e.RowVersionBy).HasMaxLength(126 /*LengthConstants.AdName*/);
            builder.Entity<T>().Property(e => e.RowVersion).IsRowVersion();
        }
    }

    public class LoggingDomCreateDatabaseIfNotExists : CreateDatabaseIfNotExists<TestDomDbContext>
    {
        protected override void Seed(TestDomDbContext context)
        {
            base.Seed(context);
        }
    }
}
