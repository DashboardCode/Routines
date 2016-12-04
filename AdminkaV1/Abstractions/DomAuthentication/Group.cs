using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Vse.AdminkaV1.DomAuthentication
{
    public class Group
    {
        [Key]
        public int GroupId { get; set; }
        [Required, MaxLength(LengthConstants.GoodForTitle), DisplayName("Name")]
        public string GroupName { get; set; }
        [Required, MaxLength(LengthConstants.AdName), DisplayName("AD")]
        public string GroupAdName { get; set; }
        public ICollection<GroupsPrivileges> GroupsPrivileges { get; set; }
        public ICollection<GroupsRoles> GroupsRoles { get; set; }
        public ICollection<UsersGroups> UsersGroups { get; set; }
        public IReadOnlyCollection<User> GetUsers()
        {
            IReadOnlyCollection<User> @value = null;
            if (UsersGroups != null)
            {
                @value = UsersGroups.Select(e => e.User).ToList();
            }
            return @value;
        }
        public IReadOnlyCollection<Privilege> GetPrivileges()
        {
            IReadOnlyCollection<Privilege> @value = null;
            if (GroupsPrivileges != null)
            {
                @value = GroupsPrivileges.Select(e => e.Privilege).ToList();
            }
            return @value;
        }

        public IReadOnlyCollection<Role> GetRoles()
        {
            IReadOnlyCollection<Role> @value = null;
            if (GroupsRoles != null)
            {
                @value = GroupsRoles.Select(e => e.Role).ToList();
            }
            return @value;
        }
    }
}
