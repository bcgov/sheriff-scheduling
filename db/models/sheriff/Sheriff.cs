using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Mapster;
using Newtonsoft.Json;
using CAS.DB.models.auth;

namespace CAS.DB.models.courtAdmin
{
    [AdaptTo("[name]Dto", IgnoreAttributes = new[] { typeof(JsonIgnoreAttribute) })]
    public class CourtAdmin : User
    {
        public Gender Gender { get; set; }
        public string BadgeNumber { get; set; }
        public string Rank { get; set; }
        public virtual List<CourtAdminAwayLocation> AwayLocation { get; set; } = new List<CourtAdminAwayLocation>();
        public virtual List<CourtAdminActingRank> ActingRank { get; set; } = new List<CourtAdminActingRank>();
        public virtual List<CourtAdminLeave> Leave { get; set; } = new List<CourtAdminLeave>();
        public virtual List<CourtAdminTraining> Training { get; set; } = new List<CourtAdminTraining>();

        [AdaptIgnore]
        public byte[] Photo { get; set; }

        [NotMapped]
        public string PhotoUrl => Photo?.Length > 0 ? $"/api/sheriff/getPhoto/{Id}?{LastPhotoUpdate.Ticks}" : null;

        public DateTimeOffset LastPhotoUpdate { get; set; }
    }
}