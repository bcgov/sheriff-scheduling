using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using db.models;
using Mapster;
using CAS.API.Models.DB;
using CAS.DB.models.lookupcodes;

namespace CAS.DB.models
{
    [AdaptTo("[name]Dto")]
    public class LookupCode : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public LookupTypes Type { get; set; }
        public string Code { get; set; }
        public string SubCode { get; set; }
        public string Description { get; set; }
        public DateTimeOffset? EffectiveDate { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public virtual Location Location { get; set; }
        public int? LocationId { get; set; }
        [AdaptIgnore]
        public List<LookupSortOrder> SortOrder { get; set; }
        [NotMapped] 
        public LookupSortOrder SortOrderForLocation;

        public Boolean Mandatory { get; set; }
        public int ValidityPeriod { get; set; }
        public string Category { get; set; }
        public int AdvanceNotice { get; set; }

    }
}
