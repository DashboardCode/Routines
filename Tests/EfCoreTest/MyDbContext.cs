using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using System;

namespace EfCoreTest
{
    public class MyDbContext : DbContext
    {
        static MyDbContext()
        {
            var loadit = new[] { typeof(Remotion.Linq.DefaultQueryProvider),
                typeof(System.Collections.Generic.AsyncEnumerator)};
        }
        private static DbContextOptions<MyDbContext> CreateOptions(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MyDbContext>();
            // TODO: support sql express
            // optionsBuilder.UseSqlite("Filename=./blog.db");
            optionsBuilder.UseSqlServer(
                connectionString,
                sqlServerDbContextOptionsBuilder => sqlServerDbContextOptionsBuilder.MigrationsAssembly("EfCoreTest")
                );
            var options = optionsBuilder.Options;
            return options;
        }
        public readonly MyLoggerProvider LoggerProvider;

        public MyDbContext(string connectionString)
            : base(CreateOptions(connectionString))
        {
            LoggerProvider = new MyLoggerProvider();
            var loggerFactory = this.GetService<ILoggerFactory>();
            loggerFactory.AddProvider(LoggerProvider);
        }
        #region DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Privilege> Privileges { get; set; }

        public DbSet<UsersGroups> UsersGroups { get; set; }
        public DbSet<RolesPrivileges> RolesPrivileges { get; set; }
        public DbSet<GroupsRoles> GroupsRoles { get; set; }

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
            modelBuilder.Entity<User>();
            modelBuilder.Entity<Group>();
            modelBuilder.Entity<Role>();
            modelBuilder.Entity<Privilege>();


            #region UsersGroups
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
                .HasKey(rp => new { rp.RoleId, rp.PrivilegeId });

            modelBuilder.Entity<RolesPrivileges>()
                .HasOne(rp => rp.Role)
                .WithMany(g => g.RolesPrivileges)
                .HasForeignKey(rp => rp.RoleId);

            modelBuilder.Entity<RolesPrivileges>()
                .HasOne(rp => rp.Privilege)
                .WithMany(r => r.RolesPrivileges)
                .HasForeignKey(rp => rp.PrivilegeId);
            #endregion
        }
    }

    public sealed class MyLoggerProvider : ILoggerProvider
    {
        public Action<string> Verbose { get; set; }
        public MyLoggerProvider()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new MyV1Logger(categoryName, this);
        }

        public void Dispose()
        {

        }
    }

    class MyV1Logger : ILogger
    {
        readonly string categoryName;
        readonly MyLoggerProvider provider;
        public MyV1Logger(string categoryName, MyLoggerProvider provider)
        {
            this.categoryName = categoryName;
            this.provider = provider;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return provider.Verbose != null;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var text = formatter(state, exception);
            provider.Verbose?.Invoke(text + " ; categoryName" + categoryName);
        }
    }
}
