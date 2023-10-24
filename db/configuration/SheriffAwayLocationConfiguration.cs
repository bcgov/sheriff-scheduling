using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CAS.DB.Configuration;
using CAS.DB.models.courtAdmin;

namespace CAS.DB.configuration
{
    public class SheriffAwayLocationConfiguration : BaseEntityConfiguration<CourtAdminAwayLocation>
    {
        public override void Configure(EntityTypeBuilder<CourtAdminAwayLocation> builder)
        {
            builder.HasOne(b => b.Location).WithMany().HasForeignKey(m => m.LocationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(b => new {b.StartDate, b.EndDate});

            base.Configure(builder);
        }
    }
}
