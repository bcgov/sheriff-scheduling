using System;
using CAS.API.models.dto.generated;

namespace CAS.API.models.dto.generated
{
    public partial class DutySlotDto
    {
        public int Id { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public int DutyId { get; set; }
        public SheriffDto Sheriff { get; set; }
        public Guid? SheriffId { get; set; }
        public LocationDto Location { get; set; }
        public int LocationId { get; set; }
        public string Timezone { get; set; }
        public bool IsNotRequired { get; set; }
        public bool IsNotAvailable { get; set; }
        public bool IsOvertime { get; set; }
        public bool IsClosed { get; set; }
        public LookupCodeDto AssignmentLookupCode { get; set; }
        public string DutyComment { get; set; }
        public string AssignmentComment { get; set; }
        public uint ConcurrencyToken { get; set; }
    }
}