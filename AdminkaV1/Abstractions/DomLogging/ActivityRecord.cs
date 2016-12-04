using System;
using System.ComponentModel.DataAnnotations;

namespace Vse.AdminkaV1.DomLogging
{
    public class ActivityRecord
    {
        [Key]
        public int ActivityRecordId {get;set;}
        [Required]
        public Guid CorrelationToken { get; set; }
        [Required, MaxLength(LengthConstants.GoodForKey)]
        public string Application { get; set; }
        [Required, MaxLength(LengthConstants.GoodForName)]
        public string FullActionName { get; set; }
        [Required]
        public DateTime ActivityRecordLoggedAt { get; set; }
        [Required]
        public bool Successed { get; set; }
        [Required]
        public long DurationTicks { get; set; }
    }
}
