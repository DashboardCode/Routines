using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using DashboardCode.Routines.Storage.EfCore;
using DashboardCode.AdminkaV1.TestDom;
using DashboardCode.Routines.Storage;

namespace DashboardCode.AdminkaV1.TestDom.DataAccessEfCore
{
    public class TestDomDbContext : VerboseDbContext
    {
        public TestDomDbContext(Action<DbContextOptionsBuilder> buildOptionsBuilder, Action<string> verbose = null)
            : base(buildOptionsBuilder, verbose)
        {
        }

        #region DbSets
       
        public DbSet<ParentRecord> ParentRecords { get; set; }
        public DbSet<ChildRecord> ChildRecords { get; set; }
        public DbSet<HierarchyRecord> TestRecords { get; set; }

        private static string GetEntityTableName(string value)
        {
            return value + "s";
        }
        private static string GetMapTableName(string value)
        {
            return value + "Map";
        }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            BuildModel(modelBuilder);
        }

        public static void BuildModel(ModelBuilder modelBuilder)
        {
            #region Test Island
            SetupVersioned(modelBuilder.Entity<TypeRecord>());
            SetupVersioned(modelBuilder.Entity<ChildRecord>());
            SetupVersioned(modelBuilder.Entity<HierarchyRecord>());
            SetupVersioned(modelBuilder.Entity<ParentRecordHierarchyRecord>());
            SetupVersioned(modelBuilder.Entity<ParentRecord>());

            string testIslandSchema = "tst";
            modelBuilder.Entity<ParentRecord>()
                .ToTable(GetEntityTableName(nameof(ParentRecord)), schema: testIslandSchema)
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
               .HasAlternateKey(e => e.FieldCA);
            modelBuilder.Entity<ParentRecord>()
                .HasAlternateKey(e => new { e.FieldCB1, e.FieldCB2 });

            modelBuilder.Entity<ChildRecord>().Property(e => e.ParentRecordId).HasMaxLength(LengthConstants.GoodForKey);
            modelBuilder.Entity<ChildRecord>().Property(e => e.TypeRecordId).HasMaxLength(LengthConstants.GoodForKey);
            modelBuilder.Entity<ChildRecord>().Property(e => e.XmlField1).HasColumnType("xml");
            modelBuilder.Entity<ChildRecord>().Property(e => e.XmlField2).HasColumnType("xml");
            modelBuilder.Entity<ChildRecord>()
                .ToTable(GetEntityTableName(nameof(ChildRecord)), schema: testIslandSchema)
                .HasKey(e => new { e.ParentRecordId, e.TypeRecordId });

            modelBuilder.Entity<ChildRecord>().HasOne(e => e.ParentRecord)
                .WithMany(e => e.ChildRecords)
                .HasForeignKey(e => e.ParentRecordId);
            //HasColumnType("xml");
            modelBuilder.Entity<ChildRecord>()
                .HasOne(e => e.TypeRecord)
                .WithMany(e => e.ChildRecords)
                .HasForeignKey(e => e.TypeRecordId);

            modelBuilder.Entity<TypeRecord>()
                .ToTable(GetEntityTableName(nameof(TypeRecord)), schema: testIslandSchema)
                .HasKey(e => e.TestTypeRecordId);
            modelBuilder.Entity<TypeRecord>()
                .Property(e => e.TestTypeRecordId).HasMaxLength(LengthConstants.GoodForKey);
            modelBuilder.Entity<TypeRecord>().Property(e => e.TypeRecordName).IsRequired().HasMaxLength(LengthConstants.GoodForName);
            modelBuilder.Entity<TypeRecord>()
                .HasIndex(e => e.TypeRecordName).IsUnique();

            modelBuilder.Entity<HierarchyRecord>().Property(e => e.HierarchyRecordTitle).IsRequired().HasMaxLength(LengthConstants.GoodForLongTitle);
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

            modelBuilder.Entity<TypeRecord>().HasAnnotation("Constraints",
                new[] { new Constraint { Name = "CK_tst_TypeRecords_TypeRecordName", Fields = new[] { "TypeRecordName" }, Body = @"CHECK (TypeRecordName NOT LIKE '%[^a-z0-9 ]%')",
                    Message = @"Only letters, numbers and space" } }
                );

            modelBuilder.Entity<TypeRecord>().HasAnnotation("Constraints",
                new[] {
                        new Constraint { Fields = new[] { nameof(TypeRecord.TypeRecordName) },
                                         Message = "Only letters, numbers and space",
                                         Name = "CK_TypeRecords_TypeRecordName",
                                         Body=@"CHECK (TypeRecordName NOT LIKE '%[^a-z0-9 ]%')" }
                    }
                );
        }

        private static void SetupVersioned<T>(EntityTypeBuilder<T> builder) where T : class, IVersioned
        {
            builder.Property(e => e.RowVersionBy).HasMaxLength(LengthConstants.AdName);
            builder.Property(e => e.RowVersion).IsRowVersion();
        }
    }
}