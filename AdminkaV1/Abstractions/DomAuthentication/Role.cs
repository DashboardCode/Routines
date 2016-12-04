using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Vse.AdminkaV1.DomAuthentication
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        [Required, MaxLength(LengthConstants.GoodForTitle), DisplayName("Name")]
        public string RoleName { get; set; }
        public ICollection<UsersRoles> UsersRoles { get; set; }
        public ICollection<GroupsRoles> GroupsRoles { get; set; }
        public ICollection<RolesPrivileges> RolesPrivileges { get; set; }

        #region Many to Many
        public IReadOnlyCollection<User> GetUsers()
        {
            IReadOnlyCollection<User> @value = null;
            if (UsersRoles != null)
            {
                @value = UsersRoles.Select(e => e.User).ToList();
            }
            return @value;
        }
        public IReadOnlyCollection<Privilege> GetPrivileges()
        {
            IReadOnlyCollection<Privilege> @value = null;
            if (RolesPrivileges != null)
            {
                @value = RolesPrivileges.Select(e => e.Privilege).ToList();
            }
            return @value;
        }

        public IReadOnlyCollection<Group> GetGroups()
        {
            IReadOnlyCollection<Group> @value = null;
            if (GroupsRoles != null)
            {
                @value = GroupsRoles.Select(e => e.Group).ToList();
            }
            return @value;
        }
        #endregion
    }
}
