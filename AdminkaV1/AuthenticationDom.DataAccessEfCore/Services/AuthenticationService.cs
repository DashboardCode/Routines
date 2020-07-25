using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Storage.EfCore;

namespace DashboardCode.AdminkaV1.AuthenticationDom.DataAccessEfCore.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        readonly AuthenticationDomDbContext dbContext;
        public AuthenticationService(AuthenticationDomDbContext dbContext)
            => this.dbContext = dbContext;

        public async Task<User> GetUserAsync(string loginName, string firstName, string secondName, IEnumerable<string> adGroupsNames)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            {
                var needCommit = false;
                var userEntity = await dbContext.Users
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
                    var adGroups = await dbContext.Groups.Where(e => adGroupsNames.Contains(e.GroupAdName)).ToListAsync();
                    needCommit = userEntity.UpdateGroups(adGroups);
                }
                else
                {
                    userEntity = new User() { LoginName = loginName, FirstName = firstName, SecondName = secondName };
                    await dbContext.Users.AddAsync(userEntity);
                    if (userEntity.UserGroupMap == null)
                        userEntity.UserGroupMap = new List<UserGroup>();
                    var groups = await dbContext.Groups.Where(e => adGroupsNames.Contains(e.GroupAdName)).ToListAsync();
                    foreach (var g in groups)
                        userEntity.UserGroupMap.Add(new UserGroup() { UserId = userEntity.UserId, GroupId = g.GroupId });
                    needCommit = true;
                }
                if (needCommit)
                {
                    await dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                dbContext.Entry(userEntity).State = EntityState.Detached;
                return userEntity;
            }
        }

        public async Task<User> GetUserAsync(string loginName, string firstName, string secondName, Func<string, bool> isInRole)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            {
                var needCommit = false;
                var userEntity = await dbContext.Users
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
                    var adGroupsAll = await dbContext.Groups.ToListAsync();
                    var adGroups = adGroupsAll.Where(e => isInRole(e.GroupAdName));
                    needCommit = userEntity.UpdateGroups(adGroups);
                }
                else
                {
                    userEntity = new User() { LoginName = loginName, FirstName = firstName, SecondName = secondName };
                    await dbContext.Users.AddAsync(userEntity);
                    if (userEntity.UserGroupMap == null)
                        userEntity.UserGroupMap = new List<UserGroup>();
                    var adGroupsAll = await dbContext.Groups.ToListAsync();
                    var groups = adGroupsAll.Where(e => isInRole(e.GroupAdName));
                    foreach (var g in groups)
                        userEntity.UserGroupMap.Add(new UserGroup() { UserId = userEntity.UserId, GroupId = g.GroupId });
                    needCommit = true;
                }
                if (needCommit)
                {
                    await dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                dbContext.Entry(userEntity).State = EntityState.Detached;
                return userEntity;
            }
        }
    }
}