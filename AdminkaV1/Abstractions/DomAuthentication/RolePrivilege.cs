using System.ComponentModel.DataAnnotations;

namespace Vse.AdminkaV1.DomAuthentication
{
    public class RolePrivilege
    {
        public int RoleId { get; set; }
        public Role Role { get; set; }
        [MaxLength(LengthConstants.GoodForKey)]
        public string PrivilegeId { get; set; }
        public Privilege Privilege { get; set; }
    }
}
