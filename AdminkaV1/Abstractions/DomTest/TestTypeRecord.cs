using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Vse.AdminkaV1.DomTest
{
    public class TestTypeRecord : VersionedEntityBase
    {
        [MaxLength(LengthConstants.GoodForKey)]
        public string TestTypeRecordId { get; set; }

        [Required, MaxLength(LengthConstants.GoodForName)]
        public string TestTypeRecordName { get; set; } //this field have check constraint in database (only letters and numbers)

        public ICollection<TestChildRecord> TestChildRecords { get; set; }
    }
}

