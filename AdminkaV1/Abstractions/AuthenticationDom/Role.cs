using System;
using System.Collections.Generic;
using System.Linq;

namespace DashboardCode.AdminkaV1.AuthenticationDom
{
    public class Role : IVersioned
    {
        public int    RoleId { get; set; }
        public string RoleName { get; set; }
        public ICollection<UserRole>      UserRoleMap { get; set; }
        public ICollection<GroupRole>     GroupRoleMap { get; set; }
        public ICollection<RolePrivilege> RolePrivilegeMap { get; set; }

        public string   RowVersionBy { get; set; }
        public DateTime RowVersionAt { get; set; }
        public byte[]   RowVersion { get; set; }

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
        public IReadOnlyCollection<(Privilege, bool)> GetPrivileges()
        {
            IReadOnlyCollection<(Privilege, bool)> @value = null;
            if (RolePrivilegeMap != null)
            {
                @value = RolePrivilegeMap.Select(e => (e.Privilege, e.IsAllowed)).ToList();
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
