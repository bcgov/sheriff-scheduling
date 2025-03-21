using db.models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS.DB.Configuration;
using SS.Db.models.auth;

namespace SS.Db.configuration
{
    public class RegionConfiguration : BaseEntityConfiguration<Region>
    {
        public override void Configure(EntityTypeBuilder<Region> builder)
        {
            builder.HasData(
                new Region { Id = 100, CreatedById = User.SystemUser, Name = "Central Programs"},
                new Region { Id = 101, CreatedById = User.SystemUser, Name = "Office of the Chief Sheriff"}
            );
            builder.HasIndex(b => b.JustinId).IsUnique();

            base.Configure(builder);
        }
    }
}
