namespace Vse.AdminkaV1.DomTest
{
    /// <summary>
    /// Shares ParentRecord's key. Tests inheritance implemented in relational model.
    /// </summary>
    public class ChildRecord : VersionedBase
    {
        public int ParentRecordId { get; set; }

        public string TypeRecordId { get; set; }

        public string XmlField1 { get; set; }

        public string XmlField2 { get; set; }

        public ParentRecord ParentRecord { get; set; }

        public TypeRecord TypeRecord { get; set; }
    }
}
