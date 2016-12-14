
namespace Vse.AdminkaV1.DomTest
{
    public class ParentRecordHierarchyRecord : VersionedBase
    {
        public int ParentRecordId { get; set; }
        public int HierarchyRecordId { get; set; }
        public ParentRecord ParentRecord { get; set; }
        public HierarchyRecord HierarchyRecord { get; set; }
    }
}
