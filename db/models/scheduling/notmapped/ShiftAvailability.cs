using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Mapster;
using CAS.API.Models.DB;
using CAS.DB.models.courtAdmin;

namespace CAS.DB.models.scheduling.notmapped
{
    [AdaptTo("[name]Dto")]
    public class ShiftAvailability
    {
        [NotMapped]
        public DateTimeOffset Start { get; set; }

        [NotMapped]
        public DateTimeOffset End { get; set; }

        [NotMapped]
        public List<ShiftAvailabilityConflict> Conflicts { get; set; }

        [NotMapped]
        public CourtAdmin Sheriff { get; set; }

        [NotMapped]
        public Guid? SheriffId { get; set; }
    }

    [AdaptTo("[name]Dto")]
    public class ShiftAvailabilityConflict
    {
        public Guid? SheriffId { get; set; }
        public ShiftConflictType Conflict { get; set; }
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
        public int? LocationId { get; set; }
        public Location Location { get; set; }
        public int? ShiftId { get; set; }
        public string WorkSection { get; set; }
        public string Timezone { get; set; }
        public double OvertimeHours { get; set; }
        public string SheriffEventType { get; set; }
        public string Comment { get; set; }

        public ICollection<DutySlot> DutySlots { get; set; }
    }

    public enum ShiftConflictType
    {
        Training,
        Leave,
        AwayLocation,
        Scheduled
    }
}