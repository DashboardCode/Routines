using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Vse.AdminkaV1.DomTest
{
    public class TypeRecord : VersionedBase
    {
        [MaxLength(LengthConstants.GoodForKey)]
        public string TestTypeRecordId { get; set; }

        [Required, MaxLength(LengthConstants.GoodForName)]
        public string TypeRecordName { get; set; } //this field have check constraint in database (only letters and numbers)

        public ICollection<ChildRecord> ChildRecords { get; set; }

        public ICollection<TypeRecord> TypeRecords { get; set; }
    }
}

