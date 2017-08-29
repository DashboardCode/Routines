using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfCoreOnNetFrameworkTestApp.Migrations
{
    [DbContext(typeof(MyDbContext))]
    partial class MyDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("EfCoreTest.ChildRecord", b =>
                {
                    b.Property<int>("ParentRecordId")
                        .HasMaxLength(4);

                    b.Property<string>("TypeRecordId")
                        .HasMaxLength(4);

                    b.Property<string>("XmlField1")
                        .HasColumnType("xml");

                    b.Property<string>("XmlField2")
                        .HasColumnType("xml");

                    b.HasKey("ParentRecordId", "TypeRecordId");

                    b.HasIndex("TypeRecordId");

                    b.ToTable("ChildRecords","tst");
                });

            modelBuilder.Entity("EfCoreTest.HierarchyRecord", b =>
                {
                    b.Property<int>("HierarchyRecordId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("HierarchyRecordTitle")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<byte[]>("ParentHierarchyRecordId");

                    b.HasKey("HierarchyRecordId");

                    b.ToTable("HierarchyRecords","tst");
                });

            modelBuilder.Entity("EfCoreTest.ParentRecord", b =>
                {
                    b.Property<int>("ParentRecordId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FieldA")
                        .IsRequired()
                        .HasMaxLength(16);

                    b.Property<string>("FieldB1")
                        .IsRequired()
                        .HasMaxLength(16);

                    b.Property<string>("FieldB2")
                        .IsRequired()
                        .HasMaxLength(16);

                    b.Property<string>("FieldCA")
                        .IsRequired()
                        .HasMaxLength(16);

                    b.Property<string>("FieldCB1")
                        .IsRequired()
                        .HasMaxLength(16);

                    b.Property<string>("FieldCB2")
                        .IsRequired()
                        .HasMaxLength(16);

                    b.Property<int>("FieldNotNull");

                    b.HasKey("ParentRecordId");

                    b.HasAlternateKey("FieldCA");


                    b.HasAlternateKey("FieldCB1", "FieldCB2");

                    b.HasIndex("FieldA")
                        .IsUnique();

                    b.HasIndex("FieldB1", "FieldB2")
                        .IsUnique();

                    b.ToTable("ParentRecords","tst");
                });

            modelBuilder.Entity("EfCoreTest.ParentRecordHierarchyRecord", b =>
                {
                    b.Property<int>("ParentRecordId");

                    b.Property<int>("HierarchyRecordId");

                    b.HasKey("ParentRecordId", "HierarchyRecordId");

                    b.HasIndex("HierarchyRecordId");

                    b.ToTable("ParentRecordHierarchyRecordMap","tst");
                });

            modelBuilder.Entity("EfCoreTest.TypeRecord", b =>
                {
                    b.Property<string>("TestTypeRecordId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(4);

                    b.Property<string>("TypeRecordName")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.Property<string>("TypeRecordTestTypeRecordId");

                    b.HasKey("TestTypeRecordId");

                    b.HasIndex("TypeRecordName")
                        .IsUnique();

                    b.HasIndex("TypeRecordTestTypeRecordId");

                    b.ToTable("TypeRecords","tst");
                });

            modelBuilder.Entity("EfCoreTest.ChildRecord", b =>
                {
                    b.HasOne("EfCoreTest.ParentRecord", "ParentRecord")
                        .WithMany("ChildRecords")
                        .HasForeignKey("ParentRecordId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("EfCoreTest.TypeRecord", "TypeRecord")
                        .WithMany("ChildRecords")
                        .HasForeignKey("TypeRecordId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("EfCoreTest.ParentRecordHierarchyRecord", b =>
                {
                    b.HasOne("EfCoreTest.HierarchyRecord", "HierarchyRecord")
                        .WithMany("ParentRecordHierarchyRecordMap")
                        .HasForeignKey("HierarchyRecordId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("EfCoreTest.ParentRecord", "ParentRecord")
                        .WithMany("ParentRecordHierarchyRecordMap")
                        .HasForeignKey("ParentRecordId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("EfCoreTest.TypeRecord", b =>
                {
                    b.HasOne("EfCoreTest.TypeRecord")
                        .WithMany("TypeRecords")
                        .HasForeignKey("TypeRecordTestTypeRecordId");
                });
        }
    }
}
