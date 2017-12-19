using System;

namespace DashboardCode.AdminkaV1.AuthenticationDom
{
    public class UserRole : IVersioned
    {
        public int UserId { get; set; }
        public User User  { get; set; }
        public int RoleId { get; set; }
        public Role Role  { get; set; }

        public string RowVersionBy { get; set; }
        public DateTime RowVersionAt { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
