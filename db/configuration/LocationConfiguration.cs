using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS.Api.Models.DB;
using SS.DB.Configuration;
using SS.Db.models.auth;

namespace SS.Db.configuration
{
    public class LocationConfiguration : BaseEntityConfiguration<Location>
    {

        public override void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.Property(b => b.Id).HasIdentityOptions(startValue: 200);

            builder.HasData(
                new Location { Id = 1,  AgencyId = "SS1", CreatedById = User.SystemUser, Name = "Office of Professional Standards", Timezone = "America/Vancouver" },
                new Location { Id = 2, AgencyId = "SS2", CreatedById = User.SystemUser, Name = "Sheriff Provincial Operation Centre", Timezone = "America/Vancouver" },
                new Location { Id = 3, AgencyId = "SS3", CreatedById = User.SystemUser, Name = "Central Float Pool", Timezone = "America/Vancouver" },
                new Location { Id = 4, RegionId = 100, AgencyId = "SS4", CreatedById = User.SystemUser, Name = "Integrated Threat Assessment Unit", Timezone = "America/Vancouver" },
                new Location { Id = 5, RegionId = 101, AgencyId = "SS5", CreatedById = User.SystemUser, Name = "Office of the Chief Sheriff", Timezone = "America/Vancouver" },
                new Location { Id = 6, AgencyId = "SS6", JustinCode = "4882", CreatedById = User.SystemUser, Name = "South Okanagan Escort Centre", Timezone = "America/Vancouver" },
                new Location { Id = 7, RegionId = 100, AgencyId = "SS7", CreatedById = User.SystemUser, Name = "Training Section", Timezone = "America/Vancouver" },
                new Location { Id = 9, RegionId = 100, AgencyId = "SS9", CreatedById = User.SystemUser, Name = "Recruitment Office", Timezone = "America/Vancouver" },
                new Location { Id = 10, RegionId = 100, AgencyId = "SS10", CreatedById = User.SystemUser, Name = "Provincial Programs", Timezone = "America/Vancouver" }
            );

            builder.HasOne(b => b.Region).WithMany().HasForeignKey(m => m.RegionId).OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(b => b.AgencyId).IsUnique();

            base.Configure(builder);
        }
    }
}
