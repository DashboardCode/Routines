using System.Collections.Generic;

namespace Vse.AdminkaV1.DomTest
{
    public class TypeRecord : VersionedBase
    {
        public string TestTypeRecordId { get; set; }

        public string TypeRecordName { get; set; } //this field have check constraint in database (only letters and numbers)

        public ICollection<ChildRecord> ChildRecords { get; set; }

        public ICollection<TypeRecord> TypeRecords { get; set; }
    }
}

