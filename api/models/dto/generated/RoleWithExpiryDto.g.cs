using System;
using CAS.API.models.dto.generated;

namespace CAS.API.models.dto.generated
{
    public partial class RoleWithExpiryDto
    {
        public RoleDto Role { get; set; }
        public DateTimeOffset EffectiveDate { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
    }
}