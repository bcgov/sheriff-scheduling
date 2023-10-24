using System;

namespace CAS.API.models.dto.generated
{
    public partial class UpdateShiftDto
    {
        public int Id { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public Guid SheriffId { get; set; }
        public int? AnticipatedAssignmentId { get; set; }
        public int LocationId { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public string Timezone { get; set; }
        public double OvertimeHours { get; set; }
        public string Comment { get; set; }
        public uint ConcurrencyToken { get; set; }
    }
}