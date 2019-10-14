using System;
using System.Data.Entity;

namespace DashboardCode.AdminkaV1.TestDom.DataAccessEf6
{
    public partial class TestDomDbContext : DbContext
    {
        private static void SetupVersioned<T>(DbModelBuilder builder) where T : class, IVersioned
        {
            builder.Entity<T>().Property(e => e.RowVersionBy).HasMaxLength(LengthConstants.AdName);
            builder.Entity<T>().Property(e => e.RowVersionAt).HasColumnType("datetime2");
            builder.Entity<T>().Property(e => e.RowVersion).IsRowVersion();//.IsFixedLength();
        }

        public TestDomDbContext(string connectionStringName, Action<string> verbose)
            : base(connectionStringName)
        {
            // some imortant options
            // Database.SetInitializer(new LoggingDomCreateDatabaseIfNotExists());
            // this.Configuration.ValidateOnSaveEnabled = false; // could be used for WCF ?
            if (verbose != null)
                this.Database.Log += message =>
                       verbose(message);
        }

        public virtual DbSet<ChildRecord> ChildRecords { get; set; }
        public virtual DbSet<HierarchyRecord> HierarchyRecords { get; set; }
        public virtual DbSet<ParentRecordHierarchyRecord> ParentRecordHierarchyRecords { get; set; }
        public virtual DbSet<ParentRecord> ParentRecords { get; set; }
        public virtual DbSet<TypeRecord> TypeRecords { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            CreateModel(modelBuilder);
        }
        public void CreateModel(DbModelBuilder modelBuilder)
        {
            SetupVersioned<TypeRecord>(modelBuilder);
            SetupVersioned<ChildRecord>(modelBuilder);
            SetupVersioned<HierarchyRecord>(modelBuilder);
            SetupVersioned<ParentRecordHierarchyRecord>(modelBuilder);
            SetupVersioned<ParentRecord>(modelBuilder);
            string testIslandSchema = "tst";

            modelBuilder.Entity<ChildRecord>()
                .ToTable("ChildRecords", testIslandSchema)
                .HasKey(e => new { e.ParentRecordId, e.TypeRecordId });
            modelBuilder.Entity<ChildRecord>().Property(e => e.TypeRecordId).HasMaxLength(LengthConstants.GoodForKey);
            modelBuilder.Entity<ChildRecord>().Property(e => e.XmlField1).HasColumnType("xml");
            modelBuilder.Entity<ChildRecord>().Property(e => e.XmlField2).HasColumnType("xml");

            modelBuilder.Entity<HierarchyRecord>()
                .ToTable("HierarchyRecords", testIslandSchema)
                .HasKey(e => e.HierarchyRecordId);
            modelBuilder.Entity<HierarchyRecord>().Property(e => e.HierarchyRecordTitle).IsRequired().HasMaxLength(LengthConstants.GoodForLongTitle);
            modelBuilder.Entity<HierarchyRecord>().Property(e => e.ParentHierarchyRecordId).HasColumnType("varbinary").HasMaxLength(892); 


            modelBuilder.Entity<ParentRecordHierarchyRecord>()
                .ToTable("ParentRecordHierarchyRecordMap", testIslandSchema)
                .HasKey(e => new { e.ParentRecordId, e.HierarchyRecordId });

            modelBuilder.Entity<ParentRecord>().ToTable("ParentRecords", testIslandSchema).HasKey(e => e.ParentRecordId);
            modelBuilder.Entity<ParentRecord>().Property(e => e.FieldA).IsRequired().HasMaxLength(LengthConstants.GoodForFormLabel);
            modelBuilder.Entity<ParentRecord>().Property(e => e.FieldB1).IsRequired().HasMaxLength(LengthConstants.GoodForFormLabel);
            modelBuilder.Entity<ParentRecord>().Property(e => e.FieldB2).IsRequired().HasMaxLength(LengthConstants.GoodForFormLabel);
            modelBuilder.Entity<ParentRecord>().Property(e => e.FieldCA).IsRequired().HasMaxLength(LengthConstants.GoodForFormLabel);
            modelBuilder.Entity<ParentRecord>().Property(e => e.FieldCB1).IsRequired().HasMaxLength(LengthConstants.GoodForFormLabel);
            modelBuilder.Entity<ParentRecord>().Property(e => e.FieldCB2).IsRequired().HasMaxLength(LengthConstants.GoodForFormLabel);

            modelBuilder.Entity<ParentRecord>()
                .HasIndex(e => e.FieldA).IsUnique();
            modelBuilder.Entity<ParentRecord>()
                .HasIndex(e => new { e.FieldB1, e.FieldB2 }).IsUnique();
            // unique constraints
            modelBuilder.Entity<ParentRecord>()
               .HasIndex(e => e.FieldCA).IsUnique();
            modelBuilder.Entity<ParentRecord>()
               .HasIndex(x => new { x.FieldCB1, x.FieldCB2 }).IsUnique();

            modelBuilder.Entity<TypeRecord>().ToTable("TypeRecords", testIslandSchema).HasKey(e => e.TestTypeRecordId);

            modelBuilder.Entity<TypeRecord>()
                .Property(e => e.TestTypeRecordId)
                .HasMaxLength(LengthConstants.GoodForKey);
            modelBuilder.Entity<TypeRecord>()
                .Property(e => e.TypeRecordTestTypeRecordId)
                .HasMaxLength(LengthConstants.GoodForKey);

            modelBuilder.Entity<TypeRecord>()
                .Property(e => e.TypeRecordName).IsRequired().HasMaxLength(LengthConstants.GoodForName);
            modelBuilder.Entity<TypeRecord>()
                .HasIndex(e => e.TypeRecordName).IsUnique();

            // --------------------------------------------------------------------------------------------------------

            modelBuilder.Entity<ChildRecord>()
               .HasRequired(e => e.TypeRecord)
               .WithMany(e => e.ChildRecords)
               .HasForeignKey(e => e.TypeRecordId);
            modelBuilder.Entity<ChildRecord>()
                .HasRequired(e => e.ParentRecord)
                .WithMany(e => e.ChildRecords)
                .HasForeignKey(e => e.ParentRecordId);

            modelBuilder.Entity<TypeRecord>()
                .HasMany(e => e.TypeRecordChildren)
                .WithOptional(e => e.TypeRecordParent)
                .HasForeignKey(e => e.TypeRecordTestTypeRecordId);


            modelBuilder.Entity<ParentRecordHierarchyRecord>()
               .HasRequired(r => r.ParentRecord)
               .WithMany(pr => pr.ParentRecordHierarchyRecordMap)
               .HasForeignKey(r => r.ParentRecordId);

            modelBuilder.Entity<ParentRecordHierarchyRecord>()
                .HasRequired(r => r.HierarchyRecord)
                .WithMany(hr => hr.ParentRecordHierarchyRecordMap)
                .HasForeignKey(r => r.HierarchyRecordId);
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
