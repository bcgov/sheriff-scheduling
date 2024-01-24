using System;

namespace CAS.API.models.dto.generated
{
    public partial class AddAssignmentDto
    {
        public int LookupCodeId { get; set; }
        public DateTimeOffset? AdhocStartDate { get; set; }
        public DateTimeOffset? AdhocEndDate { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public string Timezone { get; set; }
        public string Name { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
        public int LocationId { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public string ExpiryReason { get; set; }
        public string Comment { get; set; }
        public uint ConcurrencyToken { get; set; }
    }
}