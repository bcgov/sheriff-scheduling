using System;
using System.Collections.Generic;
using CAS.API.models.dto.generated;
using CAS.DB.models.scheduling.notmapped;

namespace CAS.API.models.dto.generated
{
    public partial class ShiftAvailabilityConflictDto
    {
        public Guid? CourtAdminId { get; set; }
        public ShiftConflictType Conflict { get; set; }
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
        public int? LocationId { get; set; }
        public LocationDto Location { get; set; }
        public int? ShiftId { get; set; }
        public string WorkSection { get; set; }
        public string Timezone { get; set; }
        public double OvertimeHours { get; set; }
        public string CourtAdminEventType { get; set; }
        public string Comment { get; set; }
        public ICollection<DutySlotDto> DutySlots { get; set; }
    }
}