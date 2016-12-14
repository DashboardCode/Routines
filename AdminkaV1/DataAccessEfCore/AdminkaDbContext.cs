using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Vse.AdminkaV1.DomAuthentication;
using Vse.AdminkaV1.DomLogging;
using Vse.AdminkaV1.DomTest;
using Vse.Routines.Storage.EfCore;

namespace Vse.AdminkaV1.DataAccessEfCore
{
    public class AdminkaDbContext : DbContext
    {
        static AdminkaDbContext()
        {
            var loadit = new[] { typeof(Remotion.Linq.DefaultQueryProvider),
                typeof(System.Collections.Generic.AsyncEnumerator),
                typeof(Remotion.Linq.DefaultQueryProvider)
            };
        }

        public AdminkaDbContext(IAdminkaOptionsFactory optionsFactory)
            : base(optionsFactory.CreateOptions<AdminkaDbContext>())
        {
        }
        #region DbSets
        public DbSet<Privilege> Privileges { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<VerboseRecord> VerboseRecords { get; set; }
        public DbSet<ActivityRecord> ActivityRecords { get; set; }
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
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            var relationalOptions = RelationalOptionsExtension.Extract(optionsBuilder.Options);
            relationalOptions.MigrationsHistoryTableName = "Migrations";
            relationalOptions.MigrationsHistoryTableSchema = "ef";
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Test Island
            string testIslandSchema = "tst";
            modelBuilder.Entity<ParentRecord>()
                .ToTable(GetEntityTableName(nameof(ParentRecord)), schema: testIslandSchema)
                .HasKey(e => e.ParentRecordId);
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

            modelBuilder.Entity<ChildRecord>()
                .ToTable(GetEntityTableName(nameof(ChildRecord)), schema: testIslandSchema)
                .HasKey(e => new { e.ParentRecordId, e.TypeRecordId });

            modelBuilder.Entity<ChildRecord>().HasOne(e => e.ParentRecord)
                .WithMany(e => e.ChildRecords)
                .HasForeignKey(e => e.ParentRecordId);

            modelBuilder.Entity<ChildRecord>()
                .HasOne(e => e.TypeRecord)
                .WithMany(e => e.ChildRecords)
                .HasForeignKey(e => e.TypeRecordId);

            modelBuilder.Entity<TypeRecord>()
                .ToTable(GetEntityTableName(nameof(TypeRecord)), schema: testIslandSchema)
                .HasKey(e => e.TestTypeRecordId);
            modelBuilder.Entity<TypeRecord>()
                .HasIndex(e => e.TypeRecordName).IsUnique();

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

            modelBuilder.Entity<ActivityRecord>();
            modelBuilder.Entity<VerboseRecord>();

            modelBuilder.Entity<Privilege>();
            modelBuilder.Entity<User>();
            modelBuilder.Entity<Group>();
            modelBuilder.Entity<Role>();

            #region UsersPrivileges
            modelBuilder.Entity<UserPrivilege>()
                .ToTable(GetMapTableName(nameof(UserPrivilege)))
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
            modelBuilder.Entity<GroupPrivilege>()
                .ToTable(GetMapTableName(nameof(GroupPrivilege)))
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
                .ToTable(GetMapTableName(nameof(UserGroup)))
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
                .ToTable(GetMapTableName(nameof(GroupRole)))
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
               .ToTable(GetMapTableName(nameof(RolePrivilege)))
               .HasKey(e => new { e.RoleId, e.PrivilegeId });

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
                .ToTable(GetMapTableName(nameof(UserRole)))
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
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
