using System;
using System.ComponentModel.DataAnnotations;
using Mapster;
using CAS.DB.models;

namespace CAS.DB.models.courtAdmin
{
    [AdaptTo("[name]Dto")]
    public class CourtAdminTraining : CourtAdminEvent
    {
        public virtual LookupCode TrainingType { get; set; }
        public int? TrainingTypeId { get; set; }
        public DateTimeOffset? TrainingCertificationExpiry { get; set; }
        [MaxLength(200)]
        public string Note { get; set; }

        public Boolean FirstNotice { get; set; }
    }
}
