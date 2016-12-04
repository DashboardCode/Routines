using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EfCoreTest
{
    public class Group
    {
        [Key]
        public int GroupId { get; set; }
        [Required, MaxLength(32)]
        public string GroupName { get; set; }
        [Required, MaxLength(32)]
        public ICollection<GroupsRoles> GroupsRoles { get; set; }
        public ICollection<UsersGroups> UsersGroups { get; set; }
        
    }
    public class GroupsRoles
    {
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
    public class UsersGroups
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
    }
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        [Required, MaxLength(32)]
        public string RoleName { get; set; }
        public ICollection<GroupsRoles> GroupsRoles { get; set; }

        public ICollection<RolesPrivileges> RolesPrivileges { get; set; }
    }
    public class RolesPrivileges
    {
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public int PrivilegeId { get; set; }
        public Privilege Privilege { get; set; }
    }
    public class Privilege
    {
        [Key]
        public int PrivilegeId { get; set; }
        [Required, MaxLength(32)]
        public string PrivilegeName { get; set; }
        public ICollection<RolesPrivileges> RolesPrivileges { get; set; }
    }
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required, MaxLength(32)]
        public string UserName { get; set; }
        public ICollection<UsersGroups> UsersGroups { get; set; }
    }
}
