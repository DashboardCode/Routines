using System.ComponentModel.DataAnnotations;

namespace Vse.AdminkaV1.DomAuthentication
{
    public class GroupsPrivileges
    {
        public int GroupId { get; set; }
        public Group Group { get; set; }
        [MaxLength(LengthConstants.GoodForKey)]
        public string PrivilegeId { get; set; }
        public Privilege Privilege { get; set; }
    }
}
