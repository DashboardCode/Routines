using System;

namespace DashboardCode.AdminkaV1.AuthenticationDom
{
    public class RolePrivilege: IVersioned
    {
        public int RoleId { get; set; }
        public Role Role  { get; set; }
        public string PrivilegeId  { get; set; }
        public Privilege Privilege { get; set; }

        public string RowVersionBy { get; set; }
        public DateTime RowVersionAt { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
