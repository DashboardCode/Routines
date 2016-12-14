namespace Vse.AdminkaV1.DomAuthentication
{
    public class GroupRole
    {
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
