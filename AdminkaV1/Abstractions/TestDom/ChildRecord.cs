namespace DashboardCode.AdminkaV1.TestDom
{
    /// <summary>
    /// Shares ParentRecord's key. Tests inheritance implemented in relational model.
    /// </summary>
    public partial class ChildRecord : VersionedBase
    {
        public int ParentRecordId { get; set; }

        public string TypeRecordId { get; set; }

        public string XmlField1 { get; set; }

        public string XmlField2 { get; set; }

        public ParentRecord ParentRecord { get; set; }

        public TypeRecord TypeRecord { get; set; }
    }
    //public class ChildRecord : VersionedBase
    //{
    //    public int ParentRecordId { get; set; }

    //    public string TypeRecordId { get; set; }

    //    public string XmlField1 { get; set; }

    //    public string XmlField2 { get; set; }

    //    public ParentRecord ParentRecord { get; set; }

    //    public TypeRecord TypeRecord { get; set; }
    //}

    //[Table("tst.ChildRecords")]
    //public class ChildRecord : VersionedBase
    //{
    //    //[Key]
    //    //[Column(Order = 0)]
    //    //[DatabaseGenerated(DatabaseGeneratedOption.None)]
    //    public int ParentRecordId { get; set; }

    //    //[Key]
    //    //[Column(Order = 1)]
    //    //[StringLength(4)]
    //    public string TypeRecordId { get; set; }

    //    //[Column(TypeName = "xml")]
    //    public string XmlField1 { get; set; }

    //    //[Column(TypeName = "xml")]
    //    public string XmlField2 { get; set; }

    //    //[StringLength(126)]
    //    //public string RowVersionBy { get; set; }

    //    //[Column(TypeName = "datetime2")]
    //    //public DateTime RowVersionAt { get; set; }

    //    //[Column(TypeName = "timestamp")]
    //    //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    //    //[MaxLength(8)]
    //    //public byte[] RowVersion { get; set; }

    //    public virtual ParentRecord ParentRecord { get; set; }

    //    public virtual TypeRecord TypeRecord { get; set; }
    //}
}