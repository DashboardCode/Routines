using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Vse.AdminkaV1.DataAccessEfCore;

namespace Vse.AdminkaV1.DataAccessEfCore.SqlServer.InstallerApp.Migrations
{
    [DbContext(typeof(AdminkaDbContext))]
    partial class AdminkaDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Vse.AdminkaV1.DomAuthentication.Group", b =>
                {
                    b.Property<int>("GroupId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("GroupAdName")
                        .IsRequired()
                        .HasMaxLength(126);

                    b.Property<string>("GroupName")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.HasKey("GroupId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("Vse.AdminkaV1.DomAuthentication.GroupPrivilege", b =>
                {
                    b.Property<int>("GroupId");

                    b.Property<string>("PrivilegeId")
                        .HasMaxLength(4);

                    b.HasKey("GroupId", "PrivilegeId");

                    b.HasIndex("PrivilegeId");

                    b.ToTable("GroupPrivilegeMap");
                });

            modelBuilder.Entity("Vse.AdminkaV1.DomAuthentication.GroupRole", b =>
                {
                    b.Property<int>("GroupId");

                    b.Property<int>("RoleId");

                    b.HasKey("GroupId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("GroupRoleMap");
                });

            modelBuilder.Entity("Vse.AdminkaV1.DomAuthentication.Privilege", b =>
                {
                    b.Property<string>("PrivilegeId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(4);

                    b.Property<string>("PrivilegeName")
                        .HasMaxLength(64);

                    b.HasKey("PrivilegeId");

                    b.ToTable("Privileges");
                });

            modelBuilder.Entity("Vse.AdminkaV1.DomAuthentication.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.HasKey("RoleId");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Vse.AdminkaV1.DomAuthentication.RolePrivilege", b =>
                {
                    b.Property<int>("RoleId");

                    b.Property<string>("PrivilegeId")
                        .HasMaxLength(4);

                    b.HasKey("RoleId", "PrivilegeId");

                    b.HasIndex("PrivilegeId");

                    b.ToTable("RolePrivilegeMap");
                });

            modelBuilder.Entity("Vse.AdminkaV1.DomAuthentication.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FirstName")
                        .HasMaxLength(64);

                    b.Property<string>("LoginName")
                        .IsRequired()
                        .HasMaxLength(126);

                    b.Property<string>("SecondName")
                        .HasMaxLength(32);

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Vse.AdminkaV1.DomAuthentication.UserGroup", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("GroupId");

                    b.HasKey("UserId", "GroupId");

                    b.HasIndex("GroupId");

                    b.ToTable("UserGroupMap");
                });

            modelBuilder.Entity("Vse.AdminkaV1.DomAuthentication.UserPrivilege", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<string>("PrivilegeId")
                        .HasMaxLength(4);

                    b.HasKey("UserId", "PrivilegeId");

                    b.HasIndex("PrivilegeId");

                    b.ToTable("UserPrivilegeMap");
                });

            modelBuilder.Entity("Vse.AdminkaV1.DomAuthentication.UserRole", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoleMap");
                });

            modelBuilder.Entity("Vse.AdminkaV1.DomLogging.ActivityRecord", b =>
                {
                    b.Property<int>("ActivityRecordId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("ActivityRecordLoggedAt");

                    b.Property<string>("Application")
                        .IsRequired()
                        .HasMaxLength(4);

                    b.Property<Guid>("CorrelationToken");

                    b.Property<long>("DurationTicks");

                    b.Property<string>("FullActionName")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.Property<bool>("Successed");

                    b.HasKey("ActivityRecordId");

                    b.ToTable("ActivityRecords");
                });

            modelBuilder.Entity("Vse.AdminkaV1.DomLogging.VerboseRecord", b =>
                {
                    b.Property<int>("ActivityRecordId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Application")
                        .IsRequired()
                        .HasMaxLength(4);

                    b.Property<Guid>("CorrelationToken")
                        .HasMaxLength(32);

                    b.Property<string>("FullActionName")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.Property<DateTime>("VerboseRecordLoggedAt");

                    b.Property<string>("VerboseRecordMessage");

                    b.Property<string>("VerboseRecordTypeId")
                        .IsRequired()
                        .HasMaxLength(4);

                    b.HasKey("ActivityRecordId");

                    b.ToTable("VerboseRecords");
                });

            modelBuilder.Entity("Vse.AdminkaV1.DomTest.ChildRecord", b =>
                {
                    b.Property<int>("ParentRecordId")
                        .HasMaxLength(4);

                    b.Property<string>("TypeRecordId")
                        .HasMaxLength(4);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime>("RowVersionAt");

                    b.Property<string>("RowVersionBy")
                        .HasMaxLength(126);

                    b.Property<string>("XmlField1")
                        .HasColumnType("xml");

                    b.Property<string>("XmlField2")
                        .HasColumnType("xml");

                    b.HasKey("ParentRecordId", "TypeRecordId");

                    b.HasIndex("TypeRecordId");

                    b.ToTable("ChildRecords","tst");
                });

            modelBuilder.Entity("Vse.AdminkaV1.DomTest.HierarchyRecord", b =>
                {
                    b.Property<int>("HierarchyRecordId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("HierarchyRecordTitle")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<byte[]>("ParentHierarchyRecordId");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime>("RowVersionAt");

                    b.Property<string>("RowVersionBy")
                        .HasMaxLength(126);

                    b.HasKey("HierarchyRecordId");

                    b.ToTable("HierarchyRecords","tst");
                });

            modelBuilder.Entity("Vse.AdminkaV1.DomTest.ParentRecord", b =>
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

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime>("RowVersionAt");

                    b.Property<string>("RowVersionBy")
                        .HasMaxLength(126);

                    b.HasKey("ParentRecordId");

                    b.HasAlternateKey("FieldCA");


                    b.HasAlternateKey("FieldCB1", "FieldCB2");

                    b.HasIndex("FieldA")
                        .IsUnique();

                    b.HasIndex("FieldB1", "FieldB2")
                        .IsUnique();

                    b.ToTable("ParentRecords","tst");
                });

            modelBuilder.Entity("Vse.AdminkaV1.DomTest.ParentRecordHierarchyRecord", b =>
                {
                    b.Property<int>("ParentRecordId");

                    b.Property<int>("HierarchyRecordId");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime>("RowVersionAt");

                    b.Property<string>("RowVersionBy")
                        .HasMaxLength(126);

                    b.HasKey("ParentRecordId", "HierarchyRecordId");

                    b.HasIndex("HierarchyRecordId");

                    b.ToTable("ParentRecordHierarchyRecordMap","tst");
                });

            modelBuilder.Entity("Vse.AdminkaV1.DomTest.TypeRecord", b =>
                {
                    b.Property<string>("TestTypeRecordId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(4);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime>("RowVersionAt");

                    b.Property<string>("RowVersionBy")
                        .HasMaxLength(126);

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

            modelBuilder.Entity("Vse.AdminkaV1.DomAuthentication.GroupPrivilege", b =>
                {
                    b.HasOne("Vse.AdminkaV1.DomAuthentication.Group", "Group")
                        .WithMany("GroupPrivilegeMap")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Vse.AdminkaV1.DomAuthentication.Privilege", "Privilege")
                        .WithMany("GroupPrivilegeMap")
                        .HasForeignKey("PrivilegeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Vse.AdminkaV1.DomAuthentication.GroupRole", b =>
                {
                    b.HasOne("Vse.AdminkaV1.DomAuthentication.Group", "Group")
                        .WithMany("GroupRoleMap")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Vse.AdminkaV1.DomAuthentication.Role", "Role")
                        .WithMany("GroupRoleMap")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Vse.AdminkaV1.DomAuthentication.RolePrivilege", b =>
                {
                    b.HasOne("Vse.AdminkaV1.DomAuthentication.Privilege", "Privilege")
                        .WithMany("RolePrivilegeMap")
                        .HasForeignKey("PrivilegeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Vse.AdminkaV1.DomAuthentication.Role", "Role")
                        .WithMany("RolePrivilegeMap")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Vse.AdminkaV1.DomAuthentication.UserGroup", b =>
                {
                    b.HasOne("Vse.AdminkaV1.DomAuthentication.Group", "Group")
                        .WithMany("UserGroupMap")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Vse.AdminkaV1.DomAuthentication.User", "User")
                        .WithMany("UserGroupMap")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Vse.AdminkaV1.DomAuthentication.UserPrivilege", b =>
                {
                    b.HasOne("Vse.AdminkaV1.DomAuthentication.Privilege", "Privilege")
                        .WithMany("UserPrivilegeMap")
                        .HasForeignKey("PrivilegeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Vse.AdminkaV1.DomAuthentication.User", "User")
                        .WithMany("UserPrivilegeMap")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Vse.AdminkaV1.DomAuthentication.UserRole", b =>
                {
                    b.HasOne("Vse.AdminkaV1.DomAuthentication.Role", "Role")
                        .WithMany("UserRoleMap")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Vse.AdminkaV1.DomAuthentication.User", "User")
                        .WithMany("UserRoleMap")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Vse.AdminkaV1.DomTest.ChildRecord", b =>
                {
                    b.HasOne("Vse.AdminkaV1.DomTest.ParentRecord", "ParentRecord")
                        .WithMany("ChildRecords")
                        .HasForeignKey("ParentRecordId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Vse.AdminkaV1.DomTest.TypeRecord", "TypeRecord")
                        .WithMany("ChildRecords")
                        .HasForeignKey("TypeRecordId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Vse.AdminkaV1.DomTest.ParentRecordHierarchyRecord", b =>
                {
                    b.HasOne("Vse.AdminkaV1.DomTest.HierarchyRecord", "HierarchyRecord")
                        .WithMany("ParentRecordHierarchyRecordMap")
                        .HasForeignKey("HierarchyRecordId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Vse.AdminkaV1.DomTest.ParentRecord", "ParentRecord")
                        .WithMany("ParentRecordHierarchyRecordMap")
                        .HasForeignKey("ParentRecordId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Vse.AdminkaV1.DomTest.TypeRecord", b =>
                {
                    b.HasOne("Vse.AdminkaV1.DomTest.TypeRecord")
                        .WithMany("TypeRecords")
                        .HasForeignKey("TypeRecordTestTypeRecordId");
                });
        }
    }
}
