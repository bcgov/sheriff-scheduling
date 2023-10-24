using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CAS.DB.Configuration;
using CAS.DB.models.courtAdmin;

namespace CAS.DB.configuration
{
    public class SheriffLeaveConfiguration : BaseEntityConfiguration<CourtAdminLeave>
    {
        public override void Configure(EntityTypeBuilder<CourtAdminLeave> builder)
        {
            builder.HasIndex(b => new { b.StartDate, b.EndDate });

            base.Configure(builder);
        }
    }
}
