using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using db.models;
using Mapster;
using CAS.API.Models.DB;
using CAS.COMMON.attributes.mapping;
using CAS.DB.models.courtAdmin;

namespace CAS.DB.models.scheduling
{
    [AdaptTo("[name]Dto")]
    [GenerateUpdateDto, GenerateAddDto]
    public class Shift : BaseEntity
    {
        [Key, ExcludeFromAddDto]
        public int Id { get; set;}
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        [ExcludeFromAddAndUpdateDto]
        public CourtAdmin Sheriff { get; set; }
        public Guid SheriffId { get; set; }
        [ExcludeFromAddAndUpdateDto]
        [NotMapped]
        public ICollection<DutySlot> DutySlots { get; set; } = new List<DutySlot>();
        [ExcludeFromAddAndUpdateDto]
        public Assignment AnticipatedAssignment { get; set; }
        public int? AnticipatedAssignmentId { get; set; }
        [ExcludeFromAddAndUpdateDto]
        public Location Location { get; set; }
        public int LocationId { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public string Timezone { get; set; }
        public double OvertimeHours { get; set; }
        [MaxLength(200)]
        public string Comment { get; set; }
        [ExcludeFromAddAndUpdateDto]
        [NotMapped]
        public string WorkSection => DutySlots.FirstOrDefault(ds =>
            ds.StartDate == DutySlots.Min(ds => ds.StartDate))?.AssignmentLookupCode?.Type.ToString().Substring(0,1);
    }
}
