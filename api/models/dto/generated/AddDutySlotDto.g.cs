using System;

namespace CAS.API.models.dto.generated
{
    public partial class AddDutySlotDto
    {
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public int DutyId { get; set; }
        public Guid? CourtAdminId { get; set; }
        public string Timezone { get; set; }
        public bool IsNotRequired { get; set; }
        public bool IsNotAvailable { get; set; }
        public bool IsOvertime { get; set; }
        public bool IsClosed { get; set; }
        public uint ConcurrencyToken { get; set; }
    }
}