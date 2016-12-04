using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vse.AdminkaV1.DomTest
{
    public class TestChildRecord : VersionedEntityBase
    {
        [MaxLength(LengthConstants.GoodForKey)]
        public int TestParentRecordId { get; set; }

        [MaxLength(LengthConstants.GoodForKey)]
        public string TestTypeRecordId { get; set; }

        [Column(TypeName = "xml")]
        public string XmlField1 { get; set; }

        [Column(TypeName = "xml")]
        public string XmlField2 { get; set; }

        public TestParentRecord TestParentRecord { get; set; }

        public TestTypeRecord TestTypeRecord { get; set; }
    }
}
