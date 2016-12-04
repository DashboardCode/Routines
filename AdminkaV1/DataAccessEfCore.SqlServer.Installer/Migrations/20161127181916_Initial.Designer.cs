using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Vse.AdminkaV1.DataAccessEfCore;

namespace Vse.AdminkaV1.DataAccessEfCore.SqlServer.Installer.Migrations
{
    [DbContext(typeof(AdminkaDbContext))]
    [Migration("20161127181916_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Vse.AdminkaV1.Abstraction.DomAuthentication.Group", b =>
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

            modelBuilder.Entity("Vse.AdminkaV1.Abstraction.DomAuthentication.GroupsPrivileges", b =>
                {
                    b.Property<int>("GroupId");

                    b.Property<string>("PrivilegeId")
                        .HasMaxLength(4);

                    b.HasKey("GroupId", "PrivilegeId");

                    b.HasIndex("PrivilegeId");

                    b.ToTable("GroupsPrivileges");
                });

            modelBuilder.Entity("Vse.AdminkaV1.Abstraction.DomAuthentication.GroupsRoles", b =>
                {
                    b.Property<int>("GroupId");

                    b.Property<int>("RoleId");

                    b.HasKey("GroupId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("GroupsRoles");
                });

            modelBuilder.Entity("Vse.AdminkaV1.Abstraction.DomAuthentication.Privilege", b =>
                {
                    b.Property<string>("PrivilegeId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(4);

                    b.Property<string>("PrivilegeName")
                        .HasMaxLength(64);

                    b.HasKey("PrivilegeId");

                    b.ToTable("Privileges");
                });

            modelBuilder.Entity("Vse.AdminkaV1.Abstraction.DomAuthentication.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.HasKey("RoleId");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Vse.AdminkaV1.Abstraction.DomAuthentication.RolesPrivileges", b =>
                {
                    b.Property<int>("RoleId");

                    b.Property<string>("PrivilegeId")
                        .HasMaxLength(4);

                    b.HasKey("RoleId", "PrivilegeId");

                    b.HasIndex("PrivilegeId");

                    b.ToTable("RolesPrivileges");
                });

            modelBuilder.Entity("Vse.AdminkaV1.Abstraction.DomAuthentication.User", b =>
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

            modelBuilder.Entity("Vse.AdminkaV1.Abstraction.DomAuthentication.UsersGroups", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("GroupId");

                    b.HasKey("UserId", "GroupId");

                    b.HasIndex("GroupId");

                    b.ToTable("UsersGroups");
                });

            modelBuilder.Entity("Vse.AdminkaV1.Abstraction.DomAuthentication.UsersPrivileges", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<string>("PrivilegeId")
                        .HasMaxLength(4);

                    b.HasKey("UserId", "PrivilegeId");

                    b.HasIndex("PrivilegeId");

                    b.ToTable("UsersPrivileges");
                });

            modelBuilder.Entity("Vse.AdminkaV1.Abstraction.DomAuthentication.UsersRoles", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UsersRoles");
                });

            modelBuilder.Entity("Vse.AdminkaV1.Abstraction.DomLogging.ActivityRecord", b =>
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

            modelBuilder.Entity("Vse.AdminkaV1.Abstraction.DomLogging.VerboseRecord", b =>
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

            modelBuilder.Entity("Vse.AdminkaV1.Abstraction.DomTest.TestChildRecord", b =>
                {
                    b.Property<int>("TestParentRecordId")
                        .HasMaxLength(4);

                    b.Property<string>("TestTypeRecordId")
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

                    b.HasKey("TestParentRecordId", "TestTypeRecordId");

                    b.HasIndex("TestTypeRecordId");

                    b.ToTable("TestChildRecords","tst");
                });

            modelBuilder.Entity("Vse.AdminkaV1.Abstraction.DomTest.TestParentRecord", b =>
                {
                    b.Property<int>("TestParentRecordId")
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

                    b.Property<int>("FieldNullable");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime>("RowVersionAt");

                    b.Property<string>("RowVersionBy")
                        .HasMaxLength(126);

                    b.HasKey("TestParentRecordId");

                    b.HasAlternateKey("FieldCA");


                    b.HasAlternateKey("FieldCB1", "FieldCB2");

                    b.HasIndex("FieldA")
                        .IsUnique();

                    b.HasIndex("FieldB1", "FieldB2")
                        .IsUnique();

                    b.ToTable("TestParentRecords","tst");
                });

            modelBuilder.Entity("Vse.AdminkaV1.Abstraction.DomTest.TestTypeRecord", b =>
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

                    b.Property<string>("TestTypeRecordName")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.HasKey("TestTypeRecordId");

                    b.HasIndex("TestTypeRecordName")
                        .IsUnique();

                    b.ToTable("TestTypeRecords","tst");
                });

            modelBuilder.Entity("Vse.AdminkaV1.Abstraction.DomAuthentication.GroupsPrivileges", b =>
                {
                    b.HasOne("Vse.AdminkaV1.Abstraction.DomAuthentication.Group", "Group")
                        .WithMany("GroupsPrivileges")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Vse.AdminkaV1.Abstraction.DomAuthentication.Privilege", "Privilege")
                        .WithMany("GroupsPrivileges")
                        .HasForeignKey("PrivilegeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Vse.AdminkaV1.Abstraction.DomAuthentication.GroupsRoles", b =>
                {
                    b.HasOne("Vse.AdminkaV1.Abstraction.DomAuthentication.Group", "Group")
                        .WithMany("GroupsRoles")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Vse.AdminkaV1.Abstraction.DomAuthentication.Role", "Role")
                        .WithMany("GroupsRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Vse.AdminkaV1.Abstraction.DomAuthentication.RolesPrivileges", b =>
                {
                    b.HasOne("Vse.AdminkaV1.Abstraction.DomAuthentication.Privilege", "Privilege")
                        .WithMany("RolesPrivileges")
                        .HasForeignKey("PrivilegeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Vse.AdminkaV1.Abstraction.DomAuthentication.Role", "Role")
                        .WithMany("RolesPrivileges")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Vse.AdminkaV1.Abstraction.DomAuthentication.UsersGroups", b =>
                {
                    b.HasOne("Vse.AdminkaV1.Abstraction.DomAuthentication.Group", "Group")
                        .WithMany("UsersGroups")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Vse.AdminkaV1.Abstraction.DomAuthentication.User", "User")
                        .WithMany("UsersGroups")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Vse.AdminkaV1.Abstraction.DomAuthentication.UsersPrivileges", b =>
                {
                    b.HasOne("Vse.AdminkaV1.Abstraction.DomAuthentication.Privilege", "Privilege")
                        .WithMany("UsersPrivileges")
                        .HasForeignKey("PrivilegeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Vse.AdminkaV1.Abstraction.DomAuthentication.User", "User")
                        .WithMany("UsersPrivileges")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Vse.AdminkaV1.Abstraction.DomAuthentication.UsersRoles", b =>
                {
                    b.HasOne("Vse.AdminkaV1.Abstraction.DomAuthentication.Role", "Role")
                        .WithMany("UsersRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Vse.AdminkaV1.Abstraction.DomAuthentication.User", "User")
                        .WithMany("UsersRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Vse.AdminkaV1.Abstraction.DomTest.TestChildRecord", b =>
                {
                    b.HasOne("Vse.AdminkaV1.Abstraction.DomTest.TestParentRecord", "TestParentRecord")
                        .WithMany("TestChildRecords")
                        .HasForeignKey("TestParentRecordId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Vse.AdminkaV1.Abstraction.DomTest.TestTypeRecord", "TestTypeRecord")
                        .WithMany("TestChildRecords")
                        .HasForeignKey("TestTypeRecordId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
