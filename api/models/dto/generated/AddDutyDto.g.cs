using System;

namespace CAS.API.models.dto.generated
{
    public partial class AddDutyDto
    {
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public int LocationId { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public int? AssignmentId { get; set; }
        public string Timezone { get; set; }
        public string Comment { get; set; }
        public uint ConcurrencyToken { get; set; }
    }
}