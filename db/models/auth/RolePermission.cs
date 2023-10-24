using System.ComponentModel.DataAnnotations;
using db.models;
using Mapster;

namespace CAS.DB.models.auth
{
    [AdaptTo("[name]Dto")]
    public class RolePermission : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [AdaptIgnore]
        public virtual Role Role { get; set; }
        public int RoleId { get; set; }
        public virtual Permission Permission { get; set; }
        public int PermissionId { get; set; }
    }
}
