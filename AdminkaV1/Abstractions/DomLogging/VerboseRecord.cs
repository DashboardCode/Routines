using System;
using System.ComponentModel.DataAnnotations;

namespace Vse.AdminkaV1.DomLogging
{
    public class VerboseRecord
    {
        [Key]
        public int ActivityRecordId { get; set; }
        [Required, MaxLength(LengthConstants.GoodForName)]
        public Guid CorrelationToken { get; set; }
        [Required, MaxLength(LengthConstants.GoodForKey)]
        public string Application { get; set; }
        [Required, MaxLength(LengthConstants.GoodForName)]
        public string FullActionName { get; set; }
        [Required]
        public DateTime VerboseRecordLoggedAt { get; set; }
        [Required, MaxLength(LengthConstants.GoodForKey)]
        public string VerboseRecordTypeId { get; set; }
        public string VerboseRecordMessage { get; set; }
    }
}
