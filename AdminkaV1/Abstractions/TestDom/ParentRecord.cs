using System.Linq;
using System.Collections.Generic;

namespace DashboardCode.AdminkaV1.TestDom
{
    /// <summary>
    /// Shares  key with ChildRecord. Tests inheritance implemented in relational model.
    /// </summary>
    /// 

    public partial class ParentRecord : VersionedBase
    {

        public int ParentRecordId { get; set; }

        public string FieldA { get; set; }

        public string FieldB1 { get; set; }

        public string FieldB2 { get; set; }

        public string FieldCA { get; set; }

        public string FieldCB1 { get; set; }

        public string FieldCB2 { get; set; }

        public int FieldNotNull { get; set; }

        public ICollection<ChildRecord> ChildRecords { get; set; }

        public ICollection<ParentRecordHierarchyRecord> ParentRecordHierarchyRecordMap { get; set; }
    }

    //public class ParentRecord : VersionedBase
    //{
    //    public int ParentRecordId { get; set; }

    //    public string FieldA      { get; set; }

    //    public string FieldB1     { get; set; }

    //    public string FieldB2     { get; set; }

    //    public string FieldCA     { get; set; }

    //    public string FieldCB1    { get; set; }

    //    public string FieldCB2    { get; set; }

    //    public int FieldNotNull   { get; set; }

    //    public ICollection<ChildRecord> ChildRecords { get; set; }

    //    //public ICollection<ParentRecordHierarchyRecord> ParentRecordHierarchyRecordMap { get; set; }

    //    //public IReadOnlyCollection<HierarchyRecord> GetHierarchyRecords()
    //    //{
    //    //    IReadOnlyCollection<HierarchyRecord> @value = null;
    //    //    if (ParentRecordHierarchyRecordMap != null)
    //    //    {
    //    //        @value = ParentRecordHierarchyRecordMap.Select(e => e.HierarchyRecord).ToList();
    //    //    }
    //    //    return @value;
    //    //}
    //}


    //[Table("tst.ParentRecords")]
    //public class ParentRecord : VersionedBase
    //{
    //    //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    //    //public ParentRecord()
    //    //{
    //    //    ChildRecords = new HashSet<ChildRecord>();
    //    //    ParentRecordHierarchyRecordMaps = new HashSet<ParentRecordHierarchyRecordMap>();
    //    //}

    //    public int ParentRecordId { get; set; }

    //    //[StringLength(126)]
    //    //public string RowVersionBy { get; set; }

    //    //[Column(TypeName = "datetime2")]
    //    //public DateTime RowVersionAt { get; set; }

    //    //[Column(TypeName = "timestamp")]
    //    //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    //    //[MaxLength(8)]
    //    //public byte[] RowVersion { get; set; }

    //    //[Required]
    //    //[StringLength(16)]
    //    public string FieldA { get; set; }

    //    //[Required]
    //    //[StringLength(16)]
    //    public string FieldB1 { get; set; }

    //    //[Required]
    //    //[StringLength(16)]
    //    public string FieldB2 { get; set; }

    //    //[Required]
    //    //[StringLength(16)]
    //    public string FieldCA { get; set; }

    //    //[Required]
    //    //[StringLength(16)]
    //    public string FieldCB1 { get; set; }

    //    //[Required]
    //    //[StringLength(16)]
    //    public string FieldCB2 { get; set; }

    //    public int FieldNotNull { get; set; }

    //    //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    //    public virtual ICollection<ChildRecord> ChildRecords { get; set; }

    //    //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    //    public virtual ICollection<ParentRecordHierarchyRecord> ParentRecordHierarchyRecordMap { get; set; }
    //}
}