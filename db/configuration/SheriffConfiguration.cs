using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CAS.DB.Configuration;
using CAS.DB.models.courtAdmin;

namespace CAS.DB.configuration
{
    public class SheriffConfiguration : BaseEntityConfiguration<CourtAdmin>
    {
        public override void Configure(EntityTypeBuilder<CourtAdmin> builder)
        {
            builder.HasIndex(s => s.BadgeNumber).IsUnique();

            base.Configure(builder);
        }
    }
}
