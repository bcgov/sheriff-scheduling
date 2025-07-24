using SS.Api.models.dto.generated;

namespace SS.Api.models.dto.generated
{
    public partial class SheriffStandardTrainingDto
    {
        public int Id { get; set; }
        public LookupCodeDto TrainingType { get; set; }
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
        public uint ConcurrencyToken { get; set; }
    }
}