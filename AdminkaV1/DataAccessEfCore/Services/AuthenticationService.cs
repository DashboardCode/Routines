using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Vse.AdminkaV1.DomAuthentication;
using Vse.Routines.Storage.EfCore;

namespace Vse.AdminkaV1.DataAccessEfCore.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        readonly AdminkaDbContextHandler dbContextManager;
        public AuthenticationService(AdminkaDbContextHandler dbContextManager)
        {
            this.dbContextManager = dbContextManager;
        }
        public User GetUser(string loginName, string firstName, string secondName, IEnumerable<string> adGroupsNames)
        {
            var user = dbContextManager.Handle(dbContext =>
            {
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    var needCommit = false;
                    var userEntity = dbContext.Users
                        .Include(e => e.UsersPrivileges).ThenInclude(e2 => e2.Privilege)
                        .Include(e => e.UsersRoles)
                        .Include(e => e.UsersRoles).ThenInclude(e2 => e2.Role)
                        .Include(e => e.UsersRoles).ThenInclude(e2 => e2.Role.RolesPrivileges)
                        .Include(e => e.UsersRoles).ThenInclude(e2 => e2.Role.RolesPrivileges).ThenInclude(e3 => e3.Privilege)
                        .Include(e => e.UsersGroups)
                        .Include(e => e.UsersGroups).ThenInclude(e2 => e2.Group)
                        .Include(e => e.UsersGroups).ThenInclude(e2 => e2.Group.GroupsPrivileges)
                        .Include(e => e.UsersGroups).ThenInclude(e2 => e2.Group.GroupsPrivileges).ThenInclude(e3 => e3.Privilege)
                        .Include(e => e.UsersGroups).ThenInclude(e2 => e2.Group.GroupsRoles)
                        .Include(e => e.UsersGroups).ThenInclude(e2 => e2.Group.GroupsRoles).ThenInclude(e3 => e3.Role)
                        .Include(e => e.UsersGroups).ThenInclude(e2 => e2.Group.GroupsRoles).ThenInclude(e3 => e3.Role.RolesPrivileges)
                        .Include(e => e.UsersGroups).ThenInclude(e2 => e2.Group.GroupsRoles).ThenInclude(e3 => e3.Role.RolesPrivileges).ThenInclude(e4=> e4.Privilege)
                        .Where(e => e.LoginName == loginName).SingleOrDefault();
                    if (userEntity != null)
                    {
                        var adGroups = dbContext.Groups.Where(e => adGroupsNames.Contains(e.GroupAdName)).ToList();
                        needCommit = userEntity.UpdateGroups(adGroups);
                    }
                    else
                    {
                        userEntity = new User() { LoginName = loginName, FirstName = firstName, SecondName = secondName };
                        dbContext.Users.Add(userEntity);
                        if (userEntity.UsersGroups == null)
                            userEntity.UsersGroups = new List<UsersGroups>();
                        var groups = dbContext.Groups.Where(e => adGroupsNames.Contains(e.GroupAdName)).ToList();
                        foreach (var g in groups)
                            userEntity.UsersGroups.Add(new UsersGroups() { UserId=userEntity.UserId, GroupId=g.GroupId});
                        needCommit = true;
                    }
                    if (needCommit)
                    {
                        dbContext.SaveChanges();
                        transaction.Commit();
                    }
                    dbContext.Entry(userEntity).State = EntityState.Detached;
                    return userEntity;
                }
            });
            return user;
        }
    }
}
