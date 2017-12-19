using System;

namespace DashboardCode.AdminkaV1.AuthenticationDom
{
    public class UserPrivilege : IVersioned
    {
        public int UserId          { get; set; }
        public User User           { get; set; }
        public string PrivilegeId  { get; set; }
        public Privilege Privilege { get; set; }

        public string RowVersionBy { get; set; }
        public DateTime RowVersionAt { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
