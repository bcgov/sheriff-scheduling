using System.Collections.Generic;
using SS.Api.models.dto.generated;

namespace SS.Api.models.dto.generated
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