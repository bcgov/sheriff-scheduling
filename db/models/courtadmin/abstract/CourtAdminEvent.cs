using System;
using System.ComponentModel.DataAnnotations;
using db.models;
using Mapster;
using CAS.DB.models.courtAdmin;

namespace CAS.DB.models
{
    public class CourtAdminEvent : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public string ExpiryReason { get; set; }
        public Guid CourtAdminId { get; set; }
        [AdaptIgnore]
        public virtual CourtAdmin CourtAdmin { get; set; }
        [MaxLength(200)]
        public string Comment { get; set; }
        public string Timezone { get; set; }
    }
}
