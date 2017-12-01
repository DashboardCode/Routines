using System.Linq;
using System.Collections.Generic;

namespace DashboardCode.AdminkaV1.AuthenticationDom
{
    public class Privilege
    {
        public const string ConfigureSystem = "CFGS";
        public const string VerboseLogging = "VLOG";

        //[DisplayName("ID")]
        public string PrivilegeId { get; set; }

        //[DisplayName("Name")]
        public string PrivilegeName { get; set; }

        public ICollection<UserPrivilege>  UserPrivilegeMap  { get; set; }
        public ICollection<GroupPrivilege> GroupPrivilegeMap { get; set; }
        public ICollection<RolePrivilege>  RolePrivilegeMap  { get; set; }

        #region Many to Many
        public IReadOnlyCollection<User> GetUsers()
        {
            IReadOnlyCollection<User> @value = null;
            if (UserPrivilegeMap != null)
            {
                @value = UserPrivilegeMap.Select(e => e.User).ToList();
            }
            return @value;
        }
        public IReadOnlyCollection<Role> GetRoles()
        {
            IReadOnlyCollection<Role> @value = null;
            if (RolePrivilegeMap != null)
            {
                @value = RolePrivilegeMap.Select(e => e.Role).ToList();
            }
            return @value;
        }

        public IReadOnlyCollection<Group> GetGroups()
        {
            IReadOnlyCollection<Group> @value = null;
            if (GroupPrivilegeMap != null)
            {
                @value = GroupPrivilegeMap.Select(e => e.Group).ToList();
            }
            return @value;
        }
        #endregion
    }
}