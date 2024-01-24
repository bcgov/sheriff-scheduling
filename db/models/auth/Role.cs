using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using db.models;
using Mapster;

namespace CAS.DB.models.auth
{
    [AdaptTo("[name]Dto")]
    public class Role : BaseEntity
    {
        public const string Manager = nameof(Manager);
        public const string Administrator = nameof(Administrator);
        public const string CourtAdmin = nameof(CourtAdmin);
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        [AdaptIgnore]
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
