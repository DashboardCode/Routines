namespace DashboardCode.AdminkaV1.TestDom
{
    public partial class ParentRecordHierarchyRecord : VersionedBase
    {
        public int ParentRecordId { get; set; }

        public int HierarchyRecordId { get; set; }

        public HierarchyRecord HierarchyRecord { get; set; }

        public ParentRecord ParentRecord { get; set; }
    }

    //public class ParentRecordHierarchyRecord : VersionedBase
    //{
    //    public int ParentRecordId { get; set; }
    //    public int HierarchyRecordId { get; set; }
    //    public ParentRecord ParentRecord { get; set; }
    //    public HierarchyRecord HierarchyRecord { get; set; }
    //}

    //[Table("tst.ParentRecordHierarchyRecordMap")]
    //public partial class ParentRecordHierarchyRecord : VersionedBase
    //{
    //    //[Key]
    //    //[Column(Order = 0)]
    //    //[DatabaseGenerated(DatabaseGeneratedOption.None)]
    //    public int ParentRecordId { get; set; }

    //    //[Key]
    //    //[Column(Order = 1)]
    //    //[DatabaseGenerated(DatabaseGeneratedOption.None)]
    //    public int HierarchyRecordId { get; set; }

    //    //[StringLength(126)]
    //    //public string RowVersionBy { get; set; }

    //    //[Column(TypeName = "datetime2")]
    //    //public DateTime RowVersionAt { get; set; }

    //    //[Column(TypeName = "timestamp")]
    //    //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    //    //[MaxLength(8)]
    //    //public byte[] RowVersion { get; set; }

    //    public virtual HierarchyRecord HierarchyRecord { get; set; }

    //    public virtual ParentRecord ParentRecord { get; set; }
    //}
}