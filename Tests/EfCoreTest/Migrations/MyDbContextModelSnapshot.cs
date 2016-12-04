using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using EfCoreTest;

namespace EfCoreTest.Migrations
{
    [DbContext(typeof(MyDbContext))]
    partial class MyDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("EfCoreTest.Group", b =>
                {
                    b.Property<int>("GroupId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("GroupName")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.HasKey("GroupId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("EfCoreTest.GroupsRoles", b =>
                {
                    b.Property<int>("GroupId");

                    b.Property<int>("RoleId");

                    b.HasKey("GroupId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("GroupsRoles");
                });

            modelBuilder.Entity("EfCoreTest.Privilege", b =>
                {
                    b.Property<int>("PrivilegeId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("PrivilegeName")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.HasKey("PrivilegeId");

                    b.ToTable("Privileges");
                });

            modelBuilder.Entity("EfCoreTest.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.HasKey("RoleId");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("EfCoreTest.RolesPrivileges", b =>
                {
                    b.Property<int>("RoleId");

                    b.Property<int>("PrivilegeId");

                    b.HasKey("RoleId", "PrivilegeId");

                    b.HasIndex("PrivilegeId");

                    b.ToTable("RolesPrivileges");
                });

            modelBuilder.Entity("EfCoreTest.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("EfCoreTest.UsersGroups", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("GroupId");

                    b.HasKey("UserId", "GroupId");

                    b.HasIndex("GroupId");

                    b.ToTable("UsersGroups");
                });

            modelBuilder.Entity("EfCoreTest.GroupsRoles", b =>
                {
                    b.HasOne("EfCoreTest.Group", "Group")
                        .WithMany("GroupsRoles")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("EfCoreTest.Role", "Role")
                        .WithMany("GroupsRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("EfCoreTest.RolesPrivileges", b =>
                {
                    b.HasOne("EfCoreTest.Privilege", "Privilege")
                        .WithMany("RolesPrivileges")
                        .HasForeignKey("PrivilegeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("EfCoreTest.Role", "Role")
                        .WithMany("RolesPrivileges")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("EfCoreTest.UsersGroups", b =>
                {
                    b.HasOne("EfCoreTest.Group", "Group")
                        .WithMany("UsersGroups")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("EfCoreTest.User", "User")
                        .WithMany("UsersGroups")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
