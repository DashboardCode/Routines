using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Vse.AdminkaV1.DomTest
{
    public class HierarchyRecord : VersionedBase
    {
        public int HierarchyRecordId { get; set; }

        //[Column(TypeName = "hierarchyid")]
        public byte[] ParentHierarchyRecordId { get; set; }

        [Required, MaxLength(LengthConstants.GoodForLongTitle)]
        public string HierarchyRecordTitle { get; set; }

        public ICollection<ParentRecordHierarchyRecord> ParentRecordHierarchyRecordMap { get; set; }

        public IReadOnlyCollection<ParentRecord> GetParentRecords()
        {
            IReadOnlyCollection<ParentRecord> @value = null;
            if (ParentRecordHierarchyRecordMap != null)
            {
                @value = ParentRecordHierarchyRecordMap.Select(e => e.ParentRecord).ToList();
            }
            return @value;
        }
    }
}
