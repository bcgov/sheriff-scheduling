using System;

namespace CAS.API.models.dto
{
    public class AssignRoleDto
    {
        public Guid UserId { get; set; }
        public int RoleId { get; set; }
        public DateTimeOffset EffectiveDate { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
    }
}
