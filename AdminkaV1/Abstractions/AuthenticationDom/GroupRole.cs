using System;

namespace DashboardCode.AdminkaV1.AuthenticationDom
{
    public class GroupRole : IVersioned
    {
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public int RoleId  { get; set; }
        public Role Role   { get; set; }

        public string RowVersionBy { get; set; }
        public DateTime RowVersionAt { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
