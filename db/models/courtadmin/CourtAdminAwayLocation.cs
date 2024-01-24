using Mapster;
using CAS.API.Models.DB;

namespace CAS.DB.models.courtAdmin
{
    [AdaptTo("[name]Dto")]
    public class CourtAdminAwayLocation : CourtAdminEvent
    {
        public virtual Location Location { get; set; }
        public int? LocationId { get; set; }
    }
}
