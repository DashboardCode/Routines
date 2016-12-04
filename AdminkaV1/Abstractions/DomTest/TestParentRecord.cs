using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Vse.AdminkaV1.DomTest
{
    public class TestParentRecord : VersionedEntityBase
    {
        public int TestParentRecordId { get; set; }

        [Required, MaxLength(LengthConstants.GoodForFormLabel)]
        public string FieldA { get; set; }

        [Required, MaxLength(LengthConstants.GoodForFormLabel)]
        public string FieldB1 { get; set; }

        [Required, MaxLength(LengthConstants.GoodForFormLabel)]
        public string FieldB2 { get; set; }

        [Required, MaxLength(LengthConstants.GoodForFormLabel)]
        public string FieldCA { get; set; }

        [Required, MaxLength(LengthConstants.GoodForFormLabel)]
        public string FieldCB1 { get; set; }

        [Required, MaxLength(LengthConstants.GoodForFormLabel)]
        public string FieldCB2 { get; set; }

        public int FieldNullable { get; set; }

        public ICollection<TestChildRecord> TestChildRecords { get; set; }
    }
}
