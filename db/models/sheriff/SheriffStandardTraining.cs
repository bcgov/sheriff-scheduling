using System;
using System.ComponentModel.DataAnnotations;
using db.models;
using Mapster;
using ss.db.models;

namespace SS.Db.models.sheriff
{
    [AdaptTo("[name]Dto")]
    public class SheriffStandardTraining : BaseEntity
    {
        public int Id { get; set; }
        public virtual LookupCode TrainingType { get; set; }
        public int? TrainingTypeId { get; set; }
        public int? ExpiryInYears { get; set; }
        public int? ExpiryMonth { get; set; }
        public int? ExpiryDate { get; set; }
        public string ConditionOn { get; set; }
        public string ConditionOperator { get; set; }
        public int? ConditionValue { get; set; }
        public int? ConditionExpiryInYears { get; set; }
        public int? ConditionExpiryMonth { get; set; }
        public int? ConditionExpiryDate { get; set; }
    }
}
