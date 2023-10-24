using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CAS.DB.Configuration;
using CAS.DB.models.audit;

namespace CAS.DB.configuration
{
    public class AuditConfiguration : BaseEntityConfiguration<Audit>
    {
        public override void Configure(EntityTypeBuilder<Audit> builder)
        {
            base.Configure(builder);

            builder.Ignore(a => a.UpdatedBy);
            builder.Ignore(a => a.UpdatedById);
            builder.Ignore(a => a.UpdatedOn);
            builder.Ignore(a => a.ConcurrencyToken);
        }
    }
}
