namespace DashboardCode.AdminkaV1.AuthenticationDom
{
    public class GroupRole
    {
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public int RoleId  { get; set; }
        public Role Role   { get; set; }
    }
}
