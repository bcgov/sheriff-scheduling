using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CAS.DB.Configuration;
using CAS.DB.models.courtAdmin;

namespace CAS.DB.configuration
{
    public class CourtAdminTrainingConfiguration : BaseEntityConfiguration<CourtAdminTraining>
    {
        public override void Configure(EntityTypeBuilder<CourtAdminTraining> builder)
        {
            builder.HasIndex(b => new { b.StartDate, b.EndDate });

            base.Configure(builder);
        }
    }
}
