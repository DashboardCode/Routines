using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Vse.AdminkaV1.DomAuthentication
{
    public class Privilege
    {
        public const string ConfigureSystem = "CFGS";
        public const string VerboseLogging = "VLOG";

        [Key, MaxLength(LengthConstants.GoodForKey), DisplayName("ID")]
        public string PrivilegeId { get; set; }

        [MaxLength(LengthConstants.GoodForTitle), DisplayName("Name")]
        public string PrivilegeName { get; set; }

        public ICollection<UsersPrivileges> UsersPrivileges { get; set; }
        public ICollection<GroupsPrivileges> GroupsPrivileges { get; set; }
        public ICollection<RolesPrivileges> RolesPrivileges { get; set; }

        #region Many to Many
        public IReadOnlyCollection<User> GetUsers()
        {
            IReadOnlyCollection<User> @value = null;
            if (UsersPrivileges != null)
            {
                @value = UsersPrivileges.Select(e => e.User).ToList();
            }
            return @value;
        }
        public IReadOnlyCollection<Role> GetRoles()
        {
            IReadOnlyCollection<Role> @value = null;
            if (RolesPrivileges != null)
            {
                @value = RolesPrivileges.Select(e => e.Role).ToList();
            }
            return @value;
        }

        public IReadOnlyCollection<Group> GetGroups()
        {
            IReadOnlyCollection<Group> @value = null;
            if (GroupsPrivileges != null)
            {
                @value = GroupsPrivileges.Select(e => e.Group).ToList();
            }
            return @value;
        }
        #endregion
    }
}
