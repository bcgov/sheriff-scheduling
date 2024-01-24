using Mapster;
using CAS.DB.models;

namespace CAS.DB.models.courtAdmin
{
    [AdaptTo("[name]Dto")]
    public class CourtAdminLeave : CourtAdminEvent
    {
        public virtual LookupCode LeaveType { get; set; }
        public int? LeaveTypeId { get; set; }
    }
}
