namespace DashboardCode.AdminkaV1.DomAuthentication
{
    public class GroupPrivilege
    {
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public string PrivilegeId { get; set; }
        public Privilege Privilege { get; set; }
    }
}
