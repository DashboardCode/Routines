namespace DashboardCode.AdminkaV1.AuthenticationDom
{
    public class GroupPrivilege
    {
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public string PrivilegeId { get; set; }
        public Privilege Privilege { get; set; }
    }
}
