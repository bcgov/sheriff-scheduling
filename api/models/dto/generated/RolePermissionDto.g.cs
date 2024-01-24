using CAS.API.models.dto.generated;

namespace CAS.API.models.dto.generated
{
    public partial class RolePermissionDto
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public PermissionDto Permission { get; set; }
        public int PermissionId { get; set; }
        public uint ConcurrencyToken { get; set; }
    }
}