using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CAS.DB.Configuration;
using CAS.DB.models.scheduling;

namespace CAS.DB.configuration
{
    public class AssignmentConfiguration : BaseEntityConfiguration<Assignment>
    {
        public override void Configure(EntityTypeBuilder<Assignment> builder)
        {
            builder.Property(b => b.Id).HasIdentityOptions(startValue: 200);

            base.Configure(builder);
        }
    }
}
