namespace DashboardCode.AdminkaV1.DomAuthentication
{
    public class RolePrivilege
    {
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public string PrivilegeId { get; set; }
        public Privilege Privilege { get; set; }
    }
}
