using System.ComponentModel.DataAnnotations;

namespace Vse.AdminkaV1.DomAuthentication
{
    public class UsersPrivileges
    {
        public int UserId { get; set; }
        public User User { get; set; }
        [MaxLength(LengthConstants.GoodForKey)]
        public string PrivilegeId { get; set; }
        public Privilege Privilege { get; set; }
    }
}
