using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace EfCoreTest
{
    /// <summary>
    /// Shares ParentRecord's key. Tests inheritance implemented in relational model.
    /// </summary>
    public class ChildRecord //: VersionedBase
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

    public class HierarchyRecord //: VersionedBase
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

    public class ParentRecord //: VersionedBase
    {
        public int ParentRecordId { get; set; }

        [Required, MaxLength(LengthConstants.GoodForFormLabel)]
        public string FieldA { get; set; }

        [Required, MaxLength(LengthConstants.GoodForFormLabel)]
        public string FieldB1 { get; set; }

        [Required, MaxLength(LengthConstants.GoodForFormLabel)]
        public string FieldB2 { get; set; }

        [Required, MaxLength(LengthConstants.GoodForFormLabel)]
        public string FieldCA { get; set; }

        [Required, MaxLength(LengthConstants.GoodForFormLabel)]
        public string FieldCB1 { get; set; }

        [Required, MaxLength(LengthConstants.GoodForFormLabel)]
        public string FieldCB2 { get; set; }

        public int FieldNotNull { get; set; }

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

    public class ParentRecordHierarchyRecord //: VersionedBase
    {
        public int ParentRecordId { get; set; }
        public int HierarchyRecordId { get; set; }
        public ParentRecord ParentRecord { get; set; }
        public HierarchyRecord HierarchyRecord { get; set; }
    }

    public class TypeRecord //: VersionedBase
    {
        [MaxLength(LengthConstants.GoodForKey)]
        public string TestTypeRecordId { get; set; }

        [Required, MaxLength(LengthConstants.GoodForName)]
        public string TypeRecordName { get; set; } //this field have check constraint in database (only letters and numbers)

        public ICollection<ChildRecord> ChildRecords { get; set; }

        public ICollection<TypeRecord> TypeRecords { get; set; }
    }

    public static class LengthConstants
    {
        public const int GoodForKey = 4;         // 64 bit can be effective primary key
        public const int GoodForColumnName = 8;
        public const int GoodForFormLabel = 16;
        public const int GoodForName = 32;   // real entities names (for single line, non-scrollable textboxes)
        public const int GoodForTitle = 64;
        public const int AdName = 126; //  21 (domain)  + 1 ("\") +  104 (username)
        public const int GoodForLongTitle = 128;
        public const int GoodForMultilineNote = 512;
    }
}
