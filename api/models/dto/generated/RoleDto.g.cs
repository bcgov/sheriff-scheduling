using System.Collections.Generic;
using CAS.API.models.dto.generated;

namespace CAS.API.models.dto.generated
{
    public partial class RoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<RolePermissionDto> RolePermissions { get; set; }
        public uint ConcurrencyToken { get; set; }
    }
}