using System.Collections.Generic;
using System.Linq;

namespace DashboardCode.AdminkaV1.AuthenticationDom
{
    public class Role
    {
        public int RoleId { get; set; }
        //[DisplayName("Name")]
        public string RoleName { get; set; }
        public ICollection<UserRole> UserRoleMap { get; set; }
        public ICollection<GroupRole> GroupRoleMap { get; set; }
        public ICollection<RolePrivilege> RolePrivilegeMap { get; set; }

        #region Many to Many
        public IReadOnlyCollection<User> GetUsers()
        {
            IReadOnlyCollection<User> @value = null;
            if (UserRoleMap != null)
            {
                @value = UserRoleMap.Select(e => e.User).ToList();
            }
            return @value;
        }
        public IReadOnlyCollection<Privilege> GetPrivileges()
        {
            IReadOnlyCollection<Privilege> @value = null;
            if (RolePrivilegeMap != null)
            {
                @value = RolePrivilegeMap.Select(e => e.Privilege).ToList();
            }
            return @value;
        }

        public IReadOnlyCollection<Group> GetGroups()
        {
            IReadOnlyCollection<Group> @value = null;
            if (GroupRoleMap != null)
            {
                @value = GroupRoleMap.Select(e => e.Group).ToList();
            }
            return @value;
        }
        #endregion
    }
}
