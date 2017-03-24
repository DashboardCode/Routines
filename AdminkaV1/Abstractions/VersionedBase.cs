using System;

namespace Vse.AdminkaV1
{
    public abstract class VersionedBase : IVersioned
    {
        //[DisplayName("Modified By")]
        public string RowVersionBy { get; set; }
        //[DisplayName("Modified At")]
        public DateTime RowVersionAt { get; set; }
        //[DisplayName("Stamp")]
        public byte[] RowVersion { get; set; }
    }
}
