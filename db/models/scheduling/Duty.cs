using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using db.models;
using Mapster;
using CAS.API.Models.DB;
using CAS.COMMON.attributes.mapping;

namespace CAS.DB.models.scheduling
{
    [AdaptTo("[name]Dto")]
    [GenerateUpdateDto, GenerateAddDto]
    public class Duty : BaseEntity
    {
        [ExcludeFromAddDto]
        [Key]
        public int Id { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        [ExcludeFromAddAndUpdateDto]
        public Location Location { get; set; }
        public int LocationId { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        [ExcludeFromAddAndUpdateDto]
        public Assignment Assignment { get; set; }
        public int? AssignmentId { get; set; }
        [ExcludeFromAddDto]
        public virtual ICollection<DutySlot> DutySlots { get; set; } = new List<DutySlot>();
        public string Timezone { get; set; }
        [MaxLength(200)]
        public string Comment { get; set; }
    }
}
