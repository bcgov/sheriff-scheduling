using System.Collections.Generic;
using CAS.API.models.dto.generated;

namespace CAS.API.models.dto
{
    public class UpdateRoleDto
    {
        public RoleDto Role { get; set; }
        public List<int> PermissionIds { get; set; } = new List<int>();
    }
}
