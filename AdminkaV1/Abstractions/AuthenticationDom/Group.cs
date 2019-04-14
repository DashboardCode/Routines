using System;
using System.Collections.Generic;
using System.Linq;

namespace DashboardCode.AdminkaV1.AuthenticationDom
{
    public class Group : IVersioned
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupAdName { get; set; }
        public ICollection<GroupPrivilege> GroupPrivilegeMap { get; set; }
        public ICollection<GroupRole> GroupRoleMap { get; set; }
        public ICollection<UserGroup> UserGroupMap { get; set; }

        public string RowVersionBy { get; set; }
        public DateTime RowVersionAt { get; set; }
        public byte[] RowVersion { get; set; }

        public IReadOnlyCollection<User> GetUsers()
        {
            IReadOnlyCollection<User> @value = null;
            if (UserGroupMap != null)
            {
                @value = UserGroupMap.Select(e => e.User).ToList();
            }
            return @value;
        }
        public IReadOnlyCollection<(Privilege, bool)> GetPrivileges()
        {
            IReadOnlyCollection<(Privilege, bool)> @value = null;
            if (GroupPrivilegeMap != null)
            {
                @value = GroupPrivilegeMap.Select(e => (e.Privilege, e.IsAllowed)).ToList();
            }
            return @value;
        }

        public IReadOnlyCollection<Role> GetRoles()
        {
            IReadOnlyCollection<Role> @value = null;
            if (GroupRoleMap != null)
            {
                @value = GroupRoleMap.Select(e => e.Role).ToList();
            }
            return @value;
        }
    }
}
