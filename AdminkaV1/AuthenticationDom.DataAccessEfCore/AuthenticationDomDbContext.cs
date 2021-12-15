using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Storage.EfCore;

namespace DashboardCode.AdminkaV1.AuthenticationDom.DataAccessEfCore
{
    public class AuthenticationDomDbContext : VerboseDbContext
    {
        public AuthenticationDomDbContext(Action<DbContextOptionsBuilder> buildOptionsBuilder, Action<string> verbose = null)
            : base(buildOptionsBuilder, verbose)
        {
        }

        #region DbSets
        public DbSet<Privilege> Privileges { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Group> Groups { get; set; }
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
            #region Security Island
            SetupVersioned(modelBuilder.Entity<Privilege>());
            SetupVersioned(modelBuilder.Entity<User>());
            SetupVersioned(modelBuilder.Entity<Group>());
            SetupVersioned(modelBuilder.Entity<Role>());

            SetupVersioned(modelBuilder.Entity<UserPrivilege>());
            SetupVersioned(modelBuilder.Entity<GroupPrivilege>());
            SetupVersioned(modelBuilder.Entity<UserGroup>());
            SetupVersioned(modelBuilder.Entity<GroupRole>());
            SetupVersioned(modelBuilder.Entity<RolePrivilege>());
            SetupVersioned(modelBuilder.Entity<UserRole>());

            string securityIslandSchema = "scr";
            modelBuilder.Entity<Privilege>()
                .ToTable(GetEntityTableName(nameof(Privilege)), schema: securityIslandSchema)
                .HasKey(e => e.PrivilegeId);
            modelBuilder.Entity<Privilege>().Property(e => e.PrivilegeId).HasMaxLength(LengthConstants.GoodForKey);
            modelBuilder.Entity<Privilege>().Property(e => e.PrivilegeName).IsRequired().HasMaxLength(LengthConstants.GoodForTitle);
            modelBuilder.Entity<Privilege>().HasIndex(e => e.PrivilegeName).HasDatabaseName("IX_scr_Privileges_PrivilegeName").IsUnique();

            modelBuilder.Entity<User>()
                .ToTable(GetEntityTableName(nameof(User)), schema: securityIslandSchema)
                .HasKey(e => e.UserId);
            modelBuilder.Entity<User>().Property(e => e.LoginName).IsRequired().HasMaxLength(LengthConstants.AdName);
            modelBuilder.Entity<User>().Property(e => e.FirstName).HasMaxLength(LengthConstants.GoodForTitle);
            modelBuilder.Entity<User>().Property(e => e.SecondName).HasMaxLength(LengthConstants.GoodForName);
            modelBuilder.Entity<User>().HasIndex(e => e.LoginName).HasDatabaseName("IX_scr_Users_LoginName").IsUnique();

            modelBuilder.Entity<Group>()
                .ToTable(GetEntityTableName(nameof(Group)), schema: securityIslandSchema)
                .HasKey(e => e.GroupId);
            modelBuilder.Entity<Group>().Property(e => e.GroupName).IsRequired().HasMaxLength(LengthConstants.GoodForName);
            modelBuilder.Entity<Group>().Property(e => e.GroupAdName).IsRequired().HasMaxLength(LengthConstants.AdName);
            modelBuilder.Entity<Group>().HasIndex(e => e.GroupName).HasDatabaseName("IX_scr_Groups_GroupName").IsUnique();
            modelBuilder.Entity<Group>().HasIndex(e => e.GroupAdName).HasDatabaseName("IX_scr_Groups_GroupAdName").IsUnique();

            modelBuilder.Entity<Role>()
                .ToTable(GetEntityTableName(nameof(Role)), schema: securityIslandSchema)
                .HasKey(e => e.RoleId);
            modelBuilder.Entity<Role>().Property(e => e.RoleName).IsRequired().HasMaxLength(LengthConstants.GoodForTitle);
            modelBuilder.Entity<Role>().HasIndex(e => e.RoleName).HasDatabaseName("IX_scr_Roles_RoleName").IsUnique();

            #region UsersPrivileges
            modelBuilder.Entity<UserPrivilege>().Property(e => e.PrivilegeId).HasMaxLength(LengthConstants.GoodForKey);
            modelBuilder.Entity<UserPrivilege>()
                .ToTable(GetMapTableName(nameof(UserPrivilege)), schema: securityIslandSchema)
                .HasKey(e => new { e.UserId, e.PrivilegeId });

            modelBuilder.Entity<UserPrivilege>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserPrivilegeMap)
                .HasForeignKey(up => up.UserId);

            modelBuilder.Entity<UserPrivilege>()
                .HasOne(up => up.Privilege)
                .WithMany(p => p.UserPrivilegeMap)
                .HasForeignKey(up => up.PrivilegeId);
            #endregion

            #region GroupsPrivileges
            modelBuilder.Entity<GroupPrivilege>().Property(e => e.PrivilegeId).HasMaxLength(LengthConstants.GoodForKey);
            modelBuilder.Entity<GroupPrivilege>()
                .ToTable(GetMapTableName(nameof(GroupPrivilege)), schema: securityIslandSchema)
                .HasKey(e => new { e.GroupId, e.PrivilegeId });

            modelBuilder.Entity<GroupPrivilege>()
                .HasOne(gp => gp.Group)
                .WithMany(g => g.GroupPrivilegeMap)
                .HasForeignKey(gp => gp.GroupId);

            modelBuilder.Entity<GroupPrivilege>()
                .HasOne(gp => gp.Privilege)
                .WithMany(p => p.GroupPrivilegeMap)
                .HasForeignKey(gp => gp.PrivilegeId);
            #endregion

            #region UsersGroups
            modelBuilder.Entity<UserGroup>()
                .ToTable(GetMapTableName(nameof(UserGroup)), schema: securityIslandSchema)
                .HasKey(e => new { e.UserId, e.GroupId });

            modelBuilder.Entity<UserGroup>()
                .HasOne(ug => ug.User)
                .WithMany(u => u.UserGroupMap)
                .HasForeignKey(ug => ug.UserId);

            modelBuilder.Entity<UserGroup>()
                .HasOne(ug => ug.Group)
                .WithMany(g => g.UserGroupMap)
                .HasForeignKey(ug => ug.GroupId);
            #endregion

            #region GroupsRoles
            modelBuilder.Entity<GroupRole>()
                .ToTable(GetMapTableName(nameof(GroupRole)), schema: securityIslandSchema)
                .HasKey(gr => new { gr.GroupId, gr.RoleId });

            modelBuilder.Entity<GroupRole>()
                .HasOne(gr => gr.Group)
                .WithMany(g => g.GroupRoleMap)
                .HasForeignKey(gr => gr.GroupId);

            modelBuilder.Entity<GroupRole>()
                .HasOne(gr => gr.Role)
                .WithMany(r => r.GroupRoleMap)
                .HasForeignKey(gr => gr.RoleId);
            #endregion

            #region RolesPrivileges
            modelBuilder.Entity<RolePrivilege>()
               .ToTable(GetMapTableName(nameof(RolePrivilege)), schema: securityIslandSchema)
               .HasKey(e => new { e.RoleId, e.PrivilegeId });
            modelBuilder.Entity<RolePrivilege>().Property(e => e.PrivilegeId).HasMaxLength(LengthConstants.GoodForKey);
            modelBuilder.Entity<RolePrivilege>()
                .HasOne(up => up.Role)
                .WithMany(u => u.RolePrivilegeMap)
                .HasForeignKey(up => up.RoleId);

            modelBuilder.Entity<RolePrivilege>()
                .HasOne(up => up.Privilege)
                .WithMany(p => p.RolePrivilegeMap)
                .HasForeignKey(up => up.PrivilegeId);
            #endregion

            #region UsersRoles
            modelBuilder.Entity<UserRole>()
                .ToTable(GetMapTableName(nameof(UserRole)), schema: securityIslandSchema)
                .HasKey(e => new { e.UserId, e.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserRoleMap)
                .HasForeignKey(up => up.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(up => up.Role)
                .WithMany(p => p.UserRoleMap)
                .HasForeignKey(up => up.RoleId);
            #endregion
            #endregion

            modelBuilder.Entity<Group>().HasAnnotation("Constraints",
                new[]
                    {
                        new Constraint { Name="CK_scr_Groups_GroupName",  Message=@"Only letters, numbers, space and .-_@:",   Fields = new[] { "GroupName" }, Body=@"CHECK (GroupName NOT LIKE '%[^a-z0-9!:!.!-!_!\!@! ]%' ESCAPE '!')"  },
                        new Constraint { Name="CK_scr_Groups_GroupAdName", Message=@"Only letters, numbers and .-_@", Fields = new[] { "GroupAdName" }, Body=@"CHECK (GroupAdName NOT LIKE '%[^a-z0-9!.!-!_!\!@]%' ESCAPE '!')"  }
                    }
                );

            modelBuilder.Entity<Role>().HasAnnotation("Constraints",
                new[]
                    {
                        new Constraint { Name="CK_scr_Roles_RoleName", Message=@"[^a-z0-9 ]", Fields = new[] { "RoleName" }, Body=@"CHECK(RoleName NOT LIKE '%[^a-z0-9 ]%')" }
                    }
                );

            modelBuilder.Entity<User>().HasAnnotation("Constraints",
                new[]
                    {
                        new Constraint { Name="CK_scr_Users_LoginName", Message=@"Only letters, numbers and .-_@", Fields = new[] { "LoginName" }, Body=@"CHECK (LoginName NOT LIKE '%[^a-z0-9!.!-!_!\!@]%' ESCAPE '!')" },
                        new Constraint { Name="CK_scr_Users_SecondName", Message=@"Only letters, space and apostrophe", Fields = new[] { "SecondName" }, Body=@"CHECK (SecondName NOT LIKE '%[^a-z '']%')" },
                        new Constraint { Name="CK_scr_Users_FirstName", Message=@"Only letters and space", Fields = new[] { "FirstName" }, Body=@"CHECK (FirstName NOT LIKE '%[^a-z ]%')" }
                    }
                );

            modelBuilder.Entity<Privilege>().HasAnnotation("Constraints",
                new[]
                    {
                        new Constraint { Name="CK_scr_Privileges_PrivilegeId", Message=@"Only letters and numbers", Fields = new[] { "PrivilegeId" }, Body=@"CHECK (PrivilegeId NOT LIKE '%[^a-z0-9]%')" }
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