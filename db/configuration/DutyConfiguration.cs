using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CAS.DB.Configuration;
using CAS.DB.models.scheduling;

namespace CAS.DB.configuration
{
    public class DutyConfiguration : BaseEntityConfiguration<Duty>
    {
        public override void Configure(EntityTypeBuilder<Duty> builder)
        {
            builder.Property(b => b.Id).HasIdentityOptions(startValue: 200);

            builder.HasOne(d => d.Assignment).WithMany().OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(d => d.Location).WithMany().OnDelete(DeleteBehavior.SetNull);
            builder.HasMany(d => d.DutySlots).WithOne().HasForeignKey(m => m.DutyId).OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(b => new { b.StartDate, b.EndDate });

            base.Configure(builder);
        }
    }
}
