using System;

namespace Vse.AdminkaV1
{
    public interface IVersionedEntity
    {
        string RowVersionBy { get; set; }
        DateTime RowVersionAt { get; set; }
        byte[] RowVersion { get; set; }
    }
}
