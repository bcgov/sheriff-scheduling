using System;
using System.ComponentModel.DataAnnotations;
using db.models;
using Mapster;

namespace CAS.DB.models.auth
{
    [AdaptTo("[name]Dto")]
    public class UserRole : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public Guid UserId { get; set; }
        [AdaptIgnore]
        public virtual User User { get; set; }
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }
        [Required]
        public DateTimeOffset EffectiveDate { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public string ExpiryReason { get; set; }
    }
}
