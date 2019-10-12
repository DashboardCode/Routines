using System.Collections.Generic;

namespace DashboardCode.AdminkaV1.TestDom
{
    public partial class TypeRecord : VersionedBase
    {
        public string TestTypeRecordId { get; set; }

        public string TypeRecordName { get; set; }

        public string TypeRecordTestTypeRecordId { get; set; }

        public ICollection<ChildRecord> ChildRecords { get; set; }

        public ICollection<TypeRecord> TypeRecordChildren { get; set; }

        public TypeRecord TypeRecordParent { get; set; }
    }

    //public class TypeRecord : VersionedBase
    //{
    //    public string TestTypeRecordId { get; set; }

    //    public string TypeRecordName { get; set; } //this field have check constraint in database (only letters and numbers)

    //    public string TypeRecordTestTypeRecordId { get; set; }
    //    public ICollection<ChildRecord> ChildRecords { get; set; }

    //    //public ICollection<TypeRecord> TypeRecords { get; set; }
    //}

    //[Table("tst.TypeRecords")]
    //public class TypeRecord : VersionedBase
    //{
    //    //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    //    //public TypeRecord()
    //    //{
    //    //    ChildRecords = new HashSet<ChildRecord>();
    //    //    TypeRecords1 = new HashSet<TypeRecord>();
    //    //}

    //    //[Key]
    //    //[StringLength(4)]
    //    public string TestTypeRecordId { get; set; }

    //    //[Required]
    //    //[StringLength(32)]
    //    public string TypeRecordName { get; set; }

    //    //[StringLength(4)]
    //    public string TypeRecordTestTypeRecordId { get; set; }

    //    //[StringLength(126)]
    //    //public string RowVersionBy { get; set; }

    //    //[Column(TypeName = "datetime2")]
    //    //public DateTime RowVersionAt { get; set; }

    //    //[Column(TypeName = "timestamp")]
    //    //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    //    //[MaxLength(8)]
    //    //public byte[] RowVersion { get; set; }

    //    //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    //    public virtual ICollection<ChildRecord> ChildRecords { get; set; }

    //    //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    //    public virtual ICollection<TypeRecord> TypeRecords1 { get; set; }

    //    public virtual TypeRecord TypeRecord1 { get; set; }
    //}
}