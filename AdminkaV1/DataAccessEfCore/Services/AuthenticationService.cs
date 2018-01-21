using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Storage.EfCore;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.DataAccessEfCore.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        readonly RepositoryDbContextHandler<UserContext, AdminkaDbContext> repositoryDbContextHandler;
        public AuthenticationService(RepositoryDbContextHandler<UserContext, AdminkaDbContext> repositoryDbContextHandler)
            => this.repositoryDbContextHandler = repositoryDbContextHandler;
        
        public User GetUser(string loginName, string firstName, string secondName, IEnumerable<string> adGroupsNames)
        {
            var user = repositoryDbContextHandler.Handle(dbContext =>
            {
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    var needCommit = false;
                    var userEntity = dbContext.Users
                        .Include(e => e.UserPrivilegeMap).ThenInclude(e2 => e2.Privilege)
                        .Include(e => e.UserRoleMap)
                        .Include(e => e.UserRoleMap).ThenInclude(e2 => e2.Role)
                        .Include(e => e.UserRoleMap).ThenInclude(e2 => e2.Role.RolePrivilegeMap)
                        .Include(e => e.UserRoleMap).ThenInclude(e2 => e2.Role.RolePrivilegeMap).ThenInclude(e3 => e3.Privilege)
                        .Include(e => e.UserGroupMap)
                        .Include(e => e.UserGroupMap).ThenInclude(e2 => e2.Group)
                        .Include(e => e.UserGroupMap).ThenInclude(e2 => e2.Group.GroupPrivilegeMap)
                        .Include(e => e.UserGroupMap).ThenInclude(e2 => e2.Group.GroupPrivilegeMap).ThenInclude(e3 => e3.Privilege)
                        .Include(e => e.UserGroupMap).ThenInclude(e2 => e2.Group.GroupRoleMap)
                        .Include(e => e.UserGroupMap).ThenInclude(e2 => e2.Group.GroupRoleMap).ThenInclude(e3 => e3.Role)
                        .Include(e => e.UserGroupMap).ThenInclude(e2 => e2.Group.GroupRoleMap).ThenInclude(e3 => e3.Role.RolePrivilegeMap)
                        .Include(e => e.UserGroupMap).ThenInclude(e2 => e2.Group.GroupRoleMap).ThenInclude(e3 => e3.Role.RolePrivilegeMap).ThenInclude(e4=> e4.Privilege)
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
                        if (userEntity.UserGroupMap == null)
                            userEntity.UserGroupMap = new List<UserGroup>();
                        var groups = dbContext.Groups.Where(e => adGroupsNames.Contains(e.GroupAdName)).ToList();
                        foreach (var g in groups)
                            userEntity.UserGroupMap.Add(new UserGroup() { UserId=userEntity.UserId, GroupId=g.GroupId});
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