using Mapster;
using CAS.API.Models.DB;

namespace CAS.DB.models.courtAdmin
{
    [AdaptTo("[name]Dto")]
    public class CourtAdminActingRank : CourtAdminEvent
    {
        public string Rank { get; set; }
    }
}