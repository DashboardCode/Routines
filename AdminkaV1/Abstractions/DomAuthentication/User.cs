using System.Collections.Generic;
using System.Linq;

namespace Vse.AdminkaV1.DomAuthentication
{
    public class User
    {
        public int UserId { get; set; }

        //[DisplayName("Login")]
        public string LoginName { get; set; }

        //[DisplayName("First Name")]
        public string FirstName { get; set; }

        //[DisplayName("Second Name")]
        public string SecondName { get; set; }

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
        public IReadOnlyCollection<Privilege> GetPrivileges()
        {
            IReadOnlyCollection<Privilege> @value = null;
            if (UserPrivilegeMap != null)
            {
                @value = UserPrivilegeMap.Select(e => e.Privilege).ToList();
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
        #endregion
    }
}
