using System;
using System.Collections.Generic;
using CAS.API.models.dto.generated;
using CAS.DB.models.courtAdmin;

namespace CAS.API.models.dto.generated
{
    public partial class SheriffDto
    {
        public Gender Gender { get; set; }
        public string BadgeNumber { get; set; }
        public string Rank { get; set; }
        public List<SheriffAwayLocationDto> AwayLocation { get; set; }
        public List<SheriffActingRankDto> ActingRank { get; set; }
        public List<SheriffLeaveDto> Leave { get; set; }
        public List<SheriffTrainingDto> Training { get; set; }
        public string PhotoUrl { get; set; }
        public DateTimeOffset LastPhotoUpdate { get; set; }
        public Guid Id { get; set; }
        public bool IsEnabled { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int? HomeLocationId { get; set; }
        public LocationDto HomeLocation { get; set; }
        public ICollection<ActiveRoleWithExpiryDto> ActiveRoles { get; set; }
        public ICollection<RoleWithExpiryDto> Roles { get; set; }
        public uint ConcurrencyToken { get; set; }
    }
}