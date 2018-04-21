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

        public bool HasPrivilege(string privilegeId)
        {
            var @value = UserPrivilegeMap != null && UserPrivilegeMap.Any(e => e.PrivilegeId == privilegeId);
            if (!@value)
                @value = (UserRoleMap != null && UserRoleMap.Any(e => e.Role.RolePrivilegeMap != null && e.Role.RolePrivilegeMap.Any(e2 => e2.PrivilegeId == privilegeId)));
            if (!@value)
                @value = (UserGroupMap != null && UserGroupMap.Any(e => e.Group.GroupPrivilegeMap != null && e.Group.GroupPrivilegeMap.Any(e2 => e2.PrivilegeId == privilegeId)));
            if (!@value)
                @value = (UserGroupMap != null && UserGroupMap.Any(e => e.Group.GroupRoleMap != null && e.Group.GroupRoleMap.Any(e2 => e2.Role.RolePrivilegeMap!=null && e2.Role.RolePrivilegeMap.Any(e3 => e3.PrivilegeId == privilegeId))));
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
        public IReadOnlyCollection<Privilege> GetPrivileges()
        {
            IReadOnlyCollection<Privilege> @value = null;
            if (UserPrivilegeMap != null)
            {
                @value = UserPrivilegeMap.Select(e => e.Privilege).ToList();
            }
            return @value;
        }
        public IReadOnlyCollection<Privilege> GetRolesPrivileges()
        {
            IReadOnlyCollection<Privilege> @value = null;
            if (UserRoleMap != null)
            {
                @value = UserRoleMap.SelectMany(e => e.Role.RolePrivilegeMap).Select(e => e.Privilege).Distinct().ToList();
            }
            return @value;
        }
        public IReadOnlyCollection<Privilege> GetGroupsPrivileges()
        {
            IReadOnlyCollection<Privilege> @value = null;
            if (UserGroupMap != null)
            {
                @value = UserGroupMap
                    .SelectMany(e => e.Group.GroupRoleMap).SelectMany(e=>e.Role.RolePrivilegeMap).Select(e=>e.Privilege).Distinct().ToList();
            }
            return @value;
        }
        public IReadOnlyCollection<Privilege> GetAllPrivileges()
        {
            IReadOnlyCollection<Privilege> priveleges1 = GetPrivileges();
            IReadOnlyCollection<Privilege> priveleges2 = GetRolesPrivileges();
            IReadOnlyCollection<Privilege> priveleges3 = GetGroupsPrivileges();
            var priveleges = new List<Privilege>();
            if (priveleges1 != null)
                priveleges.AddRange(priveleges1);
            if (priveleges2 != null)
                priveleges.AddRange(priveleges2);
            if (priveleges3 != null)
                priveleges.AddRange(priveleges3);
            return priveleges.Distinct().ToList();
        }
        #endregion
    }
}