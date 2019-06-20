using System;
using System.Collections.Generic;
using System.Linq;

namespace DashboardCode.AdminkaV1.AuthenticationDom
{
    public class User : IVersioned
    {
        public int UserId { get; set; }

        public string LoginName { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }

        public string   RowVersionBy { get; set; }
        public DateTime RowVersionAt { get; set; }
        public byte[]   RowVersion { get; set; }

        public ICollection<UserGroup> UserGroupMap { get; set; }
        public ICollection<UserRole> UserRoleMap { get; set; }
        public ICollection<UserPrivilege> UserPrivilegeMap { get; set; }

        public bool HasPrivilege(params string[] privileges)
        {
            var @value = true;
            foreach (var privilegeId in privileges)
            {
                var hasPrivilege = UserPrivilegeMap != null && UserPrivilegeMap.Any(e => e.PrivilegeId == privilegeId);
                if (!hasPrivilege)
                {
                    hasPrivilege = UserRoleMap != null && UserRoleMap.Any(e => e.Role.RolePrivilegeMap != null && e.Role.RolePrivilegeMap.Any(e2 => e2.PrivilegeId == privilegeId));
                    if (!hasPrivilege)
                    {
                        hasPrivilege = UserGroupMap != null && UserGroupMap.Any(e => e.Group.GroupPrivilegeMap != null && e.Group.GroupPrivilegeMap.Any(e2 => e2.PrivilegeId == privilegeId));
                        if (!hasPrivilege)
                        {
                            hasPrivilege = UserGroupMap != null && UserGroupMap.Any(e => e.Group.GroupRoleMap != null && e.Group.GroupRoleMap.Any(e2 => e2.Role.RolePrivilegeMap != null && e2.Role.RolePrivilegeMap.Any(e3 => e3.PrivilegeId == privilegeId)));
                            if (!hasPrivilege)
                            {
                                @value = false;
                                break;
                            }
                        }
                    }
                }
            }
            return @value;
        }

        public bool UpdateGroups(IEnumerable<Group> adGroups)
        {
            var @value = false;
            var groupsToAdd = adGroups.Where(e => !this.UserGroupMap.Any(e2 => e.GroupId == e2.GroupId)).ToList();
            foreach (var g in groupsToAdd)
            {
                this.UserGroupMap.Add(new UserGroup() { User = this, Group = g });
                @value = true;
            }

            var groupsToRemove = this.UserGroupMap.Where(e => !adGroups.Any(e2 => e.GroupId == e2.GroupId)).ToList();
            foreach (var g in groupsToRemove)
            {
                this.UserGroupMap.Remove(g);
                @value = true;
            }
            return @value;
        }

        #region Many to Many
        public IReadOnlyCollection<Role> GetRoles()
        {
            IReadOnlyCollection<Role> @value = null;
            if (UserRoleMap != null)
            {
                @value = UserRoleMap.Select(e => e.Role).ToList();
            }
            return @value;
        }
        public IReadOnlyCollection<Group> GetGroups()
        {
            IReadOnlyCollection<Group> @value = null;
            if (UserGroupMap != null)
            {
                @value = UserGroupMap.Select(e => e.Group).ToList();
            }
            return @value;
        }
        public IReadOnlyCollection<(Privilege,bool)> GetPrivileges()
        {
            IReadOnlyCollection<(Privilege, bool)> @value = null;
            if (UserPrivilegeMap != null)
            {
                @value = UserPrivilegeMap.Select(e => (e.Privilege, e.IsAllowed)).ToList();
            }
            return @value;
        }
        public IReadOnlyCollection<(Privilege, bool)> GetRolesPrivileges()
        {
            IReadOnlyCollection<(Privilege, bool)> @value = null;
            if (UserRoleMap != null)
            {
                @value = UserRoleMap.SelectMany(e => e.Role.RolePrivilegeMap).Select(e => (e.Privilege, e.IsAllowed)).Distinct().ToList();
            }
            return @value;
        }
        public IReadOnlyCollection<(Privilege, bool)> GetGroupsPrivileges()
        {
            IReadOnlyCollection<(Privilege, bool)> @value = null;
            if (UserGroupMap != null)
            {
                @value = UserGroupMap
                    .SelectMany(e => e.Group.GroupRoleMap).SelectMany(e=>e.Role.RolePrivilegeMap).Select(e=> (e.Privilege, e.IsAllowed)).Distinct().ToList();
            }
            return @value;
        }
        public IReadOnlyCollection<(Privilege, bool)> GetGroupsRolesPrivileges()
        {
            IReadOnlyCollection<(Privilege, bool)> @value = null;
            if (UserGroupMap != null)
            {
                @value = UserGroupMap
                    .SelectMany(e => e.Group.GroupRoleMap)
                        .SelectMany(e => e.Role.RolePrivilegeMap).Select(e => (e.Privilege, e.IsAllowed)).Distinct().ToList();
            }
            return @value;
        }
        public IReadOnlyCollection<Privilege> GetAllPrivileges()
        {
            IReadOnlyCollection<(Privilege privilege, bool isAllowed)> priveleges1 = GetPrivileges();
            IReadOnlyCollection<(Privilege privilege, bool isAllowed)> priveleges2 = GetRolesPrivileges();
            IReadOnlyCollection<(Privilege privilege, bool isAllowed)> priveleges3 = GetGroupsPrivileges();
            IReadOnlyCollection<(Privilege privilege, bool isAllowed)> priveleges4 = GetGroupsRolesPrivileges();

            var priveleges = new List<Privilege>();
            var denied = new List<Privilege>();
            void addPrivilege(Privilege privilege)
            {
                if (!denied.Any(e => e.PrivilegeId == privilege.PrivilegeId))
                    if (!priveleges.Any(e => e.PrivilegeId == privilege.PrivilegeId))
                        priveleges.Add(privilege);
            }
            void addDenied(Privilege privilege)
            {
                if (!denied.Any(e => e.PrivilegeId == privilege.PrivilegeId))
                {
                    priveleges.RemoveAll(e => e.PrivilegeId == privilege.PrivilegeId);
                    denied.Add(privilege);
                }
            }

            if (priveleges1 != null)
                foreach (var (privilege, isAllowed) in priveleges1)
                {
                    if (isAllowed)
                        addPrivilege(privilege);
                    else
                        addDenied(privilege);
                }
            if (priveleges2 != null)
                foreach (var (privilege, isAllowed) in priveleges2)
                {
                    if (isAllowed)
                        addPrivilege(privilege);
                    else
                        addDenied(privilege);
                }
            if (priveleges3 != null)
                foreach (var (privilege, isAllowed) in priveleges3)
                {
                    if (isAllowed)
                        addPrivilege(privilege);
                    else
                        addDenied(privilege);
                }
            if (priveleges4 != null)
                foreach (var (privilege, isAllowed) in priveleges4)
                {
                    if (isAllowed)
                        addPrivilege(privilege);
                    else
                        addDenied(privilege);
                }
            return priveleges;
        }
        #endregion
    }
}