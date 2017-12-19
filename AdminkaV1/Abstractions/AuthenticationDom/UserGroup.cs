using System;

namespace DashboardCode.AdminkaV1.AuthenticationDom
{
    public class UserGroup : IVersioned
    {
        public int UserId  { get; set; }
        public User User   { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }

        public string RowVersionBy { get; set; }
        public DateTime RowVersionAt { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
