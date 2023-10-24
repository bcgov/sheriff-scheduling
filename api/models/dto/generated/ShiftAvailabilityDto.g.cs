using System;
using System.Collections.Generic;
using CAS.API.models.dto.generated;

namespace CAS.API.models.dto.generated
{
    public partial class ShiftAvailabilityDto
    {
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
        public List<ShiftAvailabilityConflictDto> Conflicts { get; set; }
        public SheriffDto Sheriff { get; set; }
        public Guid? SheriffId { get; set; }
    }
}