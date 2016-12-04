using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Vse.AdminkaV1.DomAuthentication
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required, MaxLength(LengthConstants.AdName), DisplayName("Login")]
        public string LoginName { get; set; }

        [MaxLength(LengthConstants.GoodForTitle), DisplayName("First Name")]
        public string FirstName { get; set; }

        [MaxLength(LengthConstants.GoodForName), DisplayName("Second Name")]
        public string SecondName { get; set; }

        public ICollection<UsersGroups> UsersGroups { get; set; }
        public ICollection<UsersRoles> UsersRoles { get; set; }
        public ICollection<UsersPrivileges> UsersPrivileges { get; set; }

        public bool HasPrivilege(string privilegeId)
        {
            var @value = UsersPrivileges != null && UsersPrivileges.Any(e => e.PrivilegeId == privilegeId);
            if (!@value)
                @value = (UsersRoles != null && UsersRoles.Any(e => e.Role.RolesPrivileges != null && e.Role.RolesPrivileges.Any(e2 => e2.PrivilegeId == privilegeId)));
            if (!@value)
                @value = (UsersGroups != null && UsersGroups.Any(e => e.Group.GroupsPrivileges != null && e.Group.GroupsPrivileges.Any(e2 => e2.PrivilegeId == privilegeId)));
            if (!@value)
                @value = (UsersGroups != null && UsersGroups.Any(e => e.Group.GroupsRoles != null && e.Group.GroupsRoles.Any(e2 => e2.Role.RolesPrivileges!=null && e2.Role.RolesPrivileges.Any(e3 => e3.PrivilegeId == privilegeId))));
            return @value;
        }

        public bool UpdateGroups(IEnumerable<Group> adGroups)
        {
            var @value = false;
            var groupsToAdd = adGroups.Where(e => !this.UsersGroups.Any(e2 => e.GroupId == e2.GroupId)).ToList();
            foreach (var g in groupsToAdd)
            {
                this.UsersGroups.Add(new UsersGroups() { User = this, Group = g });
                @value = true;
            }

            var groupsToRemove = this.UsersGroups.Where(e => !adGroups.Any(e2 => e.GroupId == e2.GroupId)).ToList();
            foreach (var g in groupsToRemove)
            {
                this.UsersGroups.Remove(g);
                @value = true;
            }
            return @value;
        }

        #region Many to Many
        public IReadOnlyCollection<Role> GetRoles()
        {
            IReadOnlyCollection<Role> @value = null;
            if (UsersRoles != null)
            {
                @value = UsersRoles.Select(e => e.Role).ToList();
            }
            return @value;
        }
        public IReadOnlyCollection<Privilege> GetPrivileges()
        {
            IReadOnlyCollection<Privilege> @value = null;
            if (UsersPrivileges != null)
            {
                @value = UsersPrivileges.Select(e => e.Privilege).ToList();
            }
            return @value;
        }

        public IReadOnlyCollection<Group> GetGroups()
        {
            IReadOnlyCollection<Group> @value = null;
            if (UsersGroups != null)
            {
                @value = UsersGroups.Select(e => e.Group).ToList();
            }
            return @value;
        }
        #endregion
    }
}
