using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Vse.AdminkaV1
{
    public abstract class VersionedEntityBase : IVersionedEntity
    {
        [MaxLength(LengthConstants.AdName), DisplayName("Modified By")]
        public string RowVersionBy { get; set; }
        [DisplayName("Modified At")]
        public DateTime RowVersionAt { get; set; }
        [Timestamp, DisplayName("Stamp")]
        public byte[] RowVersion { get; set; }
    }
}
