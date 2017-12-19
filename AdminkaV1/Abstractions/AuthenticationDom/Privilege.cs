using System.Linq;
using System.Collections.Generic;
using System;

namespace DashboardCode.AdminkaV1.AuthenticationDom
{
    public class Privilege : IVersioned
    {
        public const string ConfigureSystem = "CFGS";
        public const string VerboseLogging = "VLOG";

        public string PrivilegeId          { get; set; }
        public string PrivilegeName        { get; set; }

        public string RowVersionBy   { get; set; }
        public DateTime RowVersionAt { get; set; }
        public byte[] RowVersion     { get; set; }

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