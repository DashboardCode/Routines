using System;

namespace DashboardCode.AdminkaV1.AuthenticationDom
{
    public class GroupPrivilege : IVersioned
    {
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public string PrivilegeId { get; set; }
        public Privilege Privilege { get; set; }
        public bool IsAllowed { get; set; }

        public string RowVersionBy { get; set; }
        public DateTime RowVersionAt { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
