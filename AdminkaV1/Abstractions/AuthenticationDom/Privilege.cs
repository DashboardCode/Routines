using System.Linq;
using System.Collections.Generic;
using System;

namespace DashboardCode.AdminkaV1.AuthenticationDom
{
    public enum UserPrivilegeOption
    {
        ConfigureSystem, VerboseLogging
    }

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
        public IReadOnlyCollection<(User, bool)> GetUsers()
        {
            IReadOnlyCollection<(User, bool)> @value = null;
            if (UserPrivilegeMap != null)
            {
                @value = UserPrivilegeMap.Select(e => (e.User, e.IsAllowed)).ToList();
            }
            return @value;
        }
        public IReadOnlyCollection<(Role, bool)> GetRoles()
        {
            IReadOnlyCollection<(Role, bool)> @value = null;
            if (RolePrivilegeMap != null)
            {
                @value = RolePrivilegeMap.Select(e => (e.Role, e.IsAllowed)).ToList();
            }
            return @value;
        }

        public IReadOnlyCollection<(Group, bool)> GetGroups()
        {
            IReadOnlyCollection<(Group, bool)> @value = null;
            if (GroupPrivilegeMap != null)
            {
                @value = GroupPrivilegeMap.Select(e => (e.Group, e.IsAllowed)).ToList();
            }
            return @value;
        }
        #endregion
    }
}