using System;

namespace CAS.API.models.dto
{
    public class UnassignRoleDto
    {
        public Guid UserId { get; set; }
        public int RoleId { get; set; }
        public string ExpiryReason { get; set; }
    }
}
