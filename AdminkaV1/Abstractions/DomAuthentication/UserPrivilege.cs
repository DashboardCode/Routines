namespace Vse.AdminkaV1.DomAuthentication
{
    public class UserPrivilege
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public string PrivilegeId { get; set; }
        public Privilege Privilege { get; set; }
    }
}
