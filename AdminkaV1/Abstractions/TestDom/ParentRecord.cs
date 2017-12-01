using System.Linq;
using System.Collections.Generic;

namespace DashboardCode.AdminkaV1.TestDom
{
    /// <summary>
    /// Shares  key with ChildRecord. Tests inheritance implemented in relational model.
    /// </summary>
    public class ParentRecord : VersionedBase
    {
        public int ParentRecordId { get; set; }

        public string FieldA      { get; set; }

        public string FieldB1     { get; set; }

        public string FieldB2     { get; set; }

        public string FieldCA     { get; set; }

        public string FieldCB1    { get; set; }

        public string FieldCB2    { get; set; }

        public int FieldNotNull   { get; set; }

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