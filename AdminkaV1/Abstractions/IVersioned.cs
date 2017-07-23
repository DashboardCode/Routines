using System;

namespace DashboardCode.AdminkaV1
{
    public interface IVersioned
    {
        string RowVersionBy { get; set; }
        DateTime RowVersionAt { get; set; }
        byte[] RowVersion { get; set; }
    }
}
