using System;
using CAS.API.models.dto.generated;

namespace CAS.API.models.dto
{
    public partial class AuditDto
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public dynamic KeyValuesJson { get; set; }
        public dynamic OldValuesJson { get; set; }
        public dynamic NewValuesJson { get; set; }
        public uint ConcurrencyToken { get; set; }
        public Guid? CreatedById { get; set; }
        public CourtAdminDto CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}