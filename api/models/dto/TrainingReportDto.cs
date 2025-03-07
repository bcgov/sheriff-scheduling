using System;

namespace SS.Api.models.dto
{
    public class TrainingReportDto
    {
        public string name { get; set; }
        public string trainingType { get; set; }
        public DateTimeOffset? end { get; set; }
        public DateTimeOffset? expiryDate { get; set; }
        public bool excluded { get; set; }
        public Guid sheriffId { get; set; }
        public string status { get; set; }
        public string location { get; set; }
        public string region { get; set; }
        public string _rowVariant { get; set; }
    }
}