using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CAS.DB.Configuration;
using CAS.DB.models.scheduling;

namespace CAS.DB.configuration
{
    public class DutySlotConfiguration : BaseEntityConfiguration<DutySlot>
    {
        public override void Configure(EntityTypeBuilder<DutySlot> builder)
        {
            builder.Property(b => b.Id).HasIdentityOptions(startValue: 200);

            builder.HasOne(d => d.Sheriff).WithMany().OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(d => d.Location).WithMany().OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(d => d.Duty).WithMany(d => d.DutySlots).HasForeignKey(d => d.DutyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(b => new { b.StartDate, b.EndDate });

            base.Configure(builder);
        }
    }
}
