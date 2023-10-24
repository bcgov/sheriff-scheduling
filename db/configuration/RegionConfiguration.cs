using db.models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CAS.DB.Configuration;

namespace CAS.DB.configuration
{
    public class RegionConfiguration : BaseEntityConfiguration<Region>
    {
        public override void Configure(EntityTypeBuilder<Region> builder)
        {
            builder.HasIndex(b => b.JustinId).IsUnique();

            base.Configure(builder);
        }
    }
}
