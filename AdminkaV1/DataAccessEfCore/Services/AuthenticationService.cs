using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Storage.EfCore;
using DashboardCode.AdminkaV1.AuthenticationDom;
using System.Threading.Tasks;
using System;

namespace DashboardCode.AdminkaV1.DataAccessEfCore.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        readonly AdminkaDbContext adminkaDbContext;
        public AuthenticationService(AdminkaDbContext adminkaDbContext)
            => this.adminkaDbContext = adminkaDbContext;

        public async Task<User> GetUserAsync(string loginName, string firstName, string secondName, IEnumerable<string> adGroupsNames)
        {
            using (var transaction = await adminkaDbContext.Database.BeginTransactionAsync())
            {
                var needCommit = false;
                var userEntity = await adminkaDbContext.Users
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
                    .Include(e => e.UserGroupMap).ThenInclude(e2 => e2.Group.GroupRoleMap).ThenInclude(e3 => e3.Role.RolePrivilegeMap).ThenInclude(e4 => e4.Privilege)
                    .Where(e => e.LoginName == loginName).SingleOrDefaultAsync();
                if (userEntity != null)
                {
                    var adGroups = await adminkaDbContext.Groups.Where(e => adGroupsNames.Contains(e.GroupAdName)).ToListAsync();
                    needCommit = userEntity.UpdateGroups(adGroups);
                }
                else
                {
                    userEntity = new User() { LoginName = loginName, FirstName = firstName, SecondName = secondName };
                    await adminkaDbContext.Users.AddAsync(userEntity);
                    if (userEntity.UserGroupMap == null)
                        userEntity.UserGroupMap = new List<UserGroup>();
                    var groups = await adminkaDbContext.Groups.Where(e => adGroupsNames.Contains(e.GroupAdName)).ToListAsync();
                    foreach (var g in groups)
                        userEntity.UserGroupMap.Add(new UserGroup() { UserId = userEntity.UserId, GroupId = g.GroupId });
                    needCommit = true;
                }
                if (needCommit)
                {
                    await adminkaDbContext.SaveChangesAsync();
                    transaction.Commit();
                }
                adminkaDbContext.Entry(userEntity).State = EntityState.Detached;
                return userEntity;
            }
        }

        public async Task<User> GetUserAsync(string loginName, string firstName, string secondName, Func<string, bool> isInRole)
        {
            using (var transaction = await adminkaDbContext.Database.BeginTransactionAsync())
            {
                var needCommit = false;
                var userEntity = await adminkaDbContext.Users
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
                    .Include(e => e.UserGroupMap).ThenInclude(e2 => e2.Group.GroupRoleMap).ThenInclude(e3 => e3.Role.RolePrivilegeMap).ThenInclude(e4 => e4.Privilege)
                    .Where(e => e.LoginName == loginName).SingleOrDefaultAsync();
                if (userEntity != null)
                {
                    var adGroupsAll = await adminkaDbContext.Groups.ToListAsync();
                    var adGroups = adGroupsAll.Where(e => isInRole(e.GroupAdName));
                    needCommit = userEntity.UpdateGroups(adGroups);
                }
                else
                {
                    userEntity = new User() { LoginName = loginName, FirstName = firstName, SecondName = secondName };
                    await adminkaDbContext.Users.AddAsync(userEntity);
                    if (userEntity.UserGroupMap == null)
                        userEntity.UserGroupMap = new List<UserGroup>();
                    var adGroupsAll = await adminkaDbContext.Groups.ToListAsync();
                    var groups = adGroupsAll.Where(e => isInRole(e.GroupAdName));
                    foreach (var g in groups)
                        userEntity.UserGroupMap.Add(new UserGroup() { UserId = userEntity.UserId, GroupId = g.GroupId });
                    needCommit = true;
                }
                if (needCommit)
                {
                    await adminkaDbContext.SaveChangesAsync();
                    transaction.Commit();
                }
                adminkaDbContext.Entry(userEntity).State = EntityState.Detached;
                return userEntity;
            }
        }
    }
}