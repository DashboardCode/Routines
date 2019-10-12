using System.Linq;
using System.Collections.Generic;

namespace DashboardCode.AdminkaV1.TestDom
{
    public partial class HierarchyRecord : VersionedBase
    {
        public int HierarchyRecordId { get; set; }

        //[Column(TypeName = "hierarchyid")]
        public byte[] ParentHierarchyRecordId { get; set; }

        public string HierarchyRecordTitle { get; set; }

        public ICollection<ParentRecordHierarchyRecord> ParentRecordHierarchyRecordMap { get; set; }
    }

    //public class HierarchyRecord : VersionedBase
    //{
    //    public int HierarchyRecordId { get; set; }

    //    //[Column(TypeName = "hierarchyid")]
    //    public byte[] ParentHierarchyRecordId { get; set; }

    //    public string HierarchyRecordTitle { get; set; }

    //    public ICollection<ParentRecordHierarchyRecord> ParentRecordHierarchyRecordMap { get; set; }

    //    public IReadOnlyCollection<ParentRecord> GetParentRecords()
    //    {
    //        IReadOnlyCollection<ParentRecord> @value = null;
    //        if (ParentRecordHierarchyRecordMap != null)
    //        {
    //            @value = ParentRecordHierarchyRecordMap.Select(e => e.ParentRecord).ToList();
    //        }
    //        return @value;
    //    }
    //}

    //[Table("tst.HierarchyRecords")]
    //public class HierarchyRecord : VersionedBase
    //{
    //    //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    //    //public HierarchyRecord()
    //    //{
    //    //    ParentRecordHierarchyRecordMaps = new HashSet<ParentRecordHierarchyRecordMap>();
    //    //}

    //    public int HierarchyRecordId { get; set; }

    //    //[StringLength(126)]
    //    //public string RowVersionBy { get; set; }

    //    //[Column(TypeName = "datetime2")]
    //    //public DateTime RowVersionAt { get; set; }

    //    //[Column(TypeName = "timestamp")]
    //    //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    //    //[MaxLength(8)]
    //    //public byte[] RowVersion { get; set; }

    //    public byte[] ParentHierarchyRecordId { get; set; }

    //    //[Required]
    //    //[StringLength(128)]
    //    public string HierarchyRecordTitle { get; set; }

    //    //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    //    //public virtual ICollection<ParentRecordHierarchyRecord> ParentRecordHierarchyRecordMap { get; set; }
    //}
}