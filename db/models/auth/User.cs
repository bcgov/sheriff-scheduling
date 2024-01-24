using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using db.models;
using Mapster;
using Newtonsoft.Json;
using CAS.API.Models.DB;
using CAS.DB.models.auth.notmapped;

namespace CAS.DB.models.auth
{
    public class User : BaseEntity
    {
        [AdaptIgnore]
        [NotMapped]

        public static readonly Guid SystemUser = new Guid("00000000-0000-0000-0000-000000000001");
        [Key]
        public Guid Id { get; set; }
        [JsonIgnore]
        public string IdirName { get; set; }
        [AdaptIgnore]
        [JsonIgnore]
        public Guid? IdirId { get; set; }
        [AdaptIgnore]
        [JsonIgnore]
        public Guid? KeyCloakId { get; set; }
        public bool IsEnabled { get; set;}
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int? HomeLocationId { get; set; }
        public virtual Location HomeLocation { get; set; }
        [AdaptIgnore]
        [JsonIgnore]
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        [NotMapped]
        public virtual ICollection<ActiveRoleWithExpiry> ActiveRoles =>
            UserRoles.Where(x => x.EffectiveDate <= DateTimeOffset.Now &&
                                 (x.ExpiryDate == null || x.ExpiryDate > DateTimeOffset.Now))
                .Select(ur =>
                    new ActiveRoleWithExpiry
                    { Role = ur.Role, EffectiveDate = ur.EffectiveDate, ExpiryDate = ur.ExpiryDate})
                .ToList();

        [NotMapped]
        public virtual ICollection<RoleWithExpiry> Roles => 
            UserRoles.Select(ur => new RoleWithExpiry
                    {Role = ur.Role, EffectiveDate = ur.EffectiveDate, ExpiryDate = ur.ExpiryDate})
                .ToList();

        [AdaptIgnore]
        [NotMapped]
        public virtual ICollection<Permission> Permissions =>
            ActiveRoles.
                SelectMany(x => x.Role.RolePermissions).Select(x => x.Permission).Distinct().ToList();

        [AdaptIgnore]
        [JsonIgnore]
        public DateTimeOffset? LastLogin { get; set; }
    }
}
