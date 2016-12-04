using System;
using System.Configuration;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Web.Script.Serialization;

using Vse.Includables2;

namespace EfCoreTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["EfCoreTest"].ConnectionString;
            Console.WriteLine("Check connection string:");
            Console.WriteLine(connectionString);
            CreateDatabase(connectionString);
            using (var dbContext = new MyDbContext(connectionString))
            {
                Action<IIncludable<Group>> includes = includable =>
                   includable
                     .IncludeAll(y => y.UsersGroups)
                        .ThenInclude(y => y.User)
                     .IncludeAll(y => y.GroupsRoles)
                        .ThenInclude(y => y.Role)
                        .ThenIncludeAll(y => y.RolesPrivileges)
                            .ThenInclude(y => y.Privilege);
                     //.Include(y => y.GroupType)
                     //   .ThenInclude(y => y.GroupTypeChanges);

                var group = dbContext.Groups.Include(includes).First();

                // comment this line to get circulare exception during serialization to json
                dbContext.Detach(group, includes);

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(group);
            }
        }

        static void CreateDatabase(string connectionString)
        {
            using (var dbContext = new MyDbContext(connectionString))
            {
                dbContext.Database.Migrate();
                //dbContext.Database.ExecuteSqlCommand("TRUNCATE TABLE dbo.UsersGroups");
                //dbContext.Database.ExecuteSqlCommand("TRUNCATE TABLE dbo.GroupsRoles");
                //dbContext.Database.ExecuteSqlCommand("TRUNCATE TABLE dbo.RolesPrivileges");

                //dbContext.Database.ExecuteSqlCommand("DELETE FROM dbo.Groups");
                //dbContext.Database.ExecuteSqlCommand("TRUNCATE TABLE dbo.Roles");
                //dbContext.Database.ExecuteSqlCommand("TRUNCATE TABLE dbo.Users");
                //dbContext.Database.ExecuteSqlCommand("TRUNCATE TABLE dbo.Privileges");
                if (dbContext.Groups.Count() == 0)
                    dbContext.Groups.Add(new Group() { GroupName = "Group1" });
                if (dbContext.Roles.Count() == 0)
                    dbContext.Roles.Add(new Role() { RoleName = "Role1" });
                if (dbContext.Users.Count() == 0)
                    dbContext.Users.Add(new User() { UserName = "User1" });
                if (dbContext.Privileges.Count() == 0)
                    dbContext.Privileges.Add(new Privilege() { PrivilegeName = "Privilege1" });

                dbContext.SaveChanges();

                if (dbContext.UsersGroups.Count() == 0)
                    dbContext.UsersGroups.Add(new UsersGroups() { UserId = 1, GroupId=1 });

                if (dbContext.GroupsRoles.Count() == 0)
                    dbContext.GroupsRoles.Add(new GroupsRoles() { GroupId = 1, RoleId = 1 });

                if (dbContext.RolesPrivileges.Count() == 0)
                    dbContext.RolesPrivileges.Add(new RolesPrivileges() { RoleId = 1, PrivilegeId = 1 });

                dbContext.SaveChanges();
            }
        }
    }
}
