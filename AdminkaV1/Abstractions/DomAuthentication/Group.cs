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
        public ICollection<GroupPrivilege> GroupPrivilegeMap { get; set; }
        public ICollection<GroupRole> GroupRoleMap { get; set; }
        public ICollection<UserGroup> UserGroupMap { get; set; }
        public IReadOnlyCollection<User> GetUsers()
        {
            IReadOnlyCollection<User> @value = null;
            if (UserGroupMap != null)
            {
                @value = UserGroupMap.Select(e => e.User).ToList();
            }
            return @value;
        }
        public IReadOnlyCollection<Privilege> GetPrivileges()
        {
            IReadOnlyCollection<Privilege> @value = null;
            if (GroupPrivilegeMap != null)
            {
                @value = GroupPrivilegeMap.Select(e => e.Privilege).ToList();
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
