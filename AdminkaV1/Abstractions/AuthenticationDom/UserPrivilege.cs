namespace DashboardCode.AdminkaV1.AuthenticationDom
{
    public class UserPrivilege
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public string PrivilegeId { get; set; }
        public Privilege Privilege { get; set; }
    }
}
