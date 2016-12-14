using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Vse.AdminkaV1.DomTest
{
    /// <summary>
    /// Shares  key with ChildRecord. Tests inheritance implemented in relational model.
    /// </summary>
    public class ParentRecord : VersionedBase
    {
        public int ParentRecordId { get; set; }

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

        public int FieldNotNull { get; set; }

        public ICollection<ChildRecord> ChildRecords { get; set; }

        public ICollection<ParentRecordHierarchyRecord> ParentRecordHierarchyRecordMap { get; set; }

        public IReadOnlyCollection<HierarchyRecord> GetHierarchyRecords()
        {
            IReadOnlyCollection<HierarchyRecord> @value = null;
            if (ParentRecordHierarchyRecordMap != null)
            {
                @value = ParentRecordHierarchyRecordMap.Select(e => e.HierarchyRecord).ToList();
            }
            return @value;
        }
    }
}
