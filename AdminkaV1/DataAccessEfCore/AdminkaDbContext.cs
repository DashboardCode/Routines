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

        public readonly LoggerProvider LoggerProvider;
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
        public DbSet<TestParentRecord> TestParentRecords { get; set; }
        public DbSet<TestChildRecord> TestChildRecords { get; set; }

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
            #region Test island
            string testIslandSchema = "tst";
            modelBuilder.Entity<TestParentRecord>()
                .ToTable("TestParentRecords", schema: testIslandSchema)
                .HasKey(e => e.TestParentRecordId);
            // unique indexes
            modelBuilder.Entity<TestParentRecord>()
                .HasIndex(e => e.FieldA).IsUnique();
            modelBuilder.Entity<TestParentRecord>()
                .HasIndex(e => new { e.FieldB1, e.FieldB2 }).IsUnique();
            // unique constraints
            modelBuilder.Entity<TestParentRecord>()
               .HasAlternateKey(e => e.FieldCA);
            modelBuilder.Entity<TestParentRecord>()
                .HasAlternateKey(e => new { e.FieldCB1, e.FieldCB2 });

            modelBuilder.Entity<TestChildRecord>()
                .ToTable("TestChildRecords", schema: testIslandSchema)
                .HasKey(e => new { e.TestParentRecordId, e.TestTypeRecordId });

            modelBuilder.Entity<TestChildRecord>().HasOne(e => e.TestParentRecord)
                .WithMany(e => e.TestChildRecords)
                .HasForeignKey(e => e.TestParentRecordId);

            modelBuilder.Entity<TestChildRecord>()
                .HasOne(e => e.TestTypeRecord)
                .WithMany(e => e.TestChildRecords)
                .HasForeignKey(e => e.TestTypeRecordId);

            modelBuilder.Entity<TestTypeRecord>()
                .ToTable("TestTypeRecords", schema: testIslandSchema)
                .HasKey(e => e.TestTypeRecordId);
            modelBuilder.Entity<TestTypeRecord>()
                .HasIndex(e => e.TestTypeRecordName).IsUnique();
            #endregion

            modelBuilder.Entity<ActivityRecord>();
            modelBuilder.Entity<VerboseRecord>();

            modelBuilder.Entity<Privilege>();
            modelBuilder.Entity<User>();
            modelBuilder.Entity<Group>();
            modelBuilder.Entity<Role>();

            #region UsersPrivileges
            modelBuilder.Entity<UsersPrivileges>()
                .HasKey(e => new { e.UserId, e.PrivilegeId });

            modelBuilder.Entity<UsersPrivileges>()
                .HasOne(up => up.User)
                .WithMany(u => u.UsersPrivileges)
                .HasForeignKey(up => up.UserId);

            modelBuilder.Entity<UsersPrivileges>()
                .HasOne(up => up.Privilege)
                .WithMany(p => p.UsersPrivileges)
                .HasForeignKey(up => up.PrivilegeId);
            #endregion

            #region GroupsPrivileges
            modelBuilder.Entity<GroupsPrivileges>()
                .HasKey(e => new { e.GroupId, e.PrivilegeId });

            modelBuilder.Entity<GroupsPrivileges>()
                .HasOne(gp => gp.Group)
                .WithMany(g => g.GroupsPrivileges)
                .HasForeignKey(gp => gp.GroupId);

            modelBuilder.Entity<GroupsPrivileges>()
                .HasOne(gp => gp.Privilege)
                .WithMany(p => p.GroupsPrivileges)
                .HasForeignKey(gp => gp.PrivilegeId);
            #endregion

            #region GroupsPrivileges
            modelBuilder.Entity<UsersGroups>()
                            .HasKey(e => new { e.UserId, e.GroupId });

            modelBuilder.Entity<UsersGroups>()
                .HasOne(ug => ug.User)
                .WithMany(u => u.UsersGroups)
                .HasForeignKey(ug => ug.UserId);

            modelBuilder.Entity<UsersGroups>()
                .HasOne(ug => ug.Group)
                .WithMany(g => g.UsersGroups)
                .HasForeignKey(ug => ug.GroupId);
            #endregion

            #region GroupsRoles
            modelBuilder.Entity<GroupsRoles>()
                .HasKey(gr => new { gr.GroupId, gr.RoleId });

            modelBuilder.Entity<GroupsRoles>()
                .HasOne(gr => gr.Group)
                .WithMany(g => g.GroupsRoles)
                .HasForeignKey(gr => gr.GroupId);

            modelBuilder.Entity<GroupsRoles>()
                .HasOne(gr => gr.Role)
                .WithMany(r => r.GroupsRoles)
                .HasForeignKey(gr => gr.RoleId);
            #endregion

            #region RolesPrivileges
            modelBuilder.Entity<RolesPrivileges>()
               .HasKey(e => new { e.RoleId, e.PrivilegeId });

            modelBuilder.Entity<RolesPrivileges>()
                .HasOne(up => up.Role)
                .WithMany(u => u.RolesPrivileges)
                .HasForeignKey(up => up.RoleId);

            modelBuilder.Entity<RolesPrivileges>()
                .HasOne(up => up.Privilege)
                .WithMany(p => p.RolesPrivileges)
                .HasForeignKey(up => up.PrivilegeId);
            #endregion

            #region UsersRoles
            modelBuilder.Entity<UsersRoles>()
                            .HasKey(e => new { e.UserId, e.RoleId });

            modelBuilder.Entity<UsersRoles>()
                .HasOne(up => up.User)
                .WithMany(u => u.UsersRoles)
                .HasForeignKey(up => up.UserId);

            modelBuilder.Entity<UsersRoles>()
                .HasOne(up => up.Role)
                .WithMany(p => p.UsersRoles)
                .HasForeignKey(up => up.RoleId);
            #endregion
        }

    }
}
