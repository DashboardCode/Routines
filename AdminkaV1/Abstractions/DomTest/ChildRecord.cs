using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vse.AdminkaV1.DomTest
{
    /// <summary>
    /// Shares ParentRecord's key. Tests inheritance implemented in relational model.
    /// </summary>
    public class ChildRecord : VersionedBase
    {
        [MaxLength(LengthConstants.GoodForKey)]
        public int ParentRecordId { get; set; }

        [MaxLength(LengthConstants.GoodForKey)]
        public string TypeRecordId { get; set; }

        [Column(TypeName = "xml")]
        public string XmlField1 { get; set; }

        [Column(TypeName = "xml")]
        public string XmlField2 { get; set; }

        public ParentRecord ParentRecord { get; set; }

        public TypeRecord TypeRecord { get; set; }
    }
}
