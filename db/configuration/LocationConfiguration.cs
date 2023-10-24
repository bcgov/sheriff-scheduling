using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CAS.API.Models.DB;
using CAS.DB.Configuration;
using CAS.DB.models.auth;

namespace CAS.DB.configuration
{
    public class LocationConfiguration : BaseEntityConfiguration<Location>
    {

        public override void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.Property(b => b.Id).HasIdentityOptions(startValue: 200);

            builder.HasData(
                new Location { Id = 1, AgencyId = "SS1", CreatedById = User.SystemUser, Name = "Office of Professional Standards", Timezone = "America/Vancouver" },
                new Location { Id = 2, AgencyId = "SS2", CreatedById = User.SystemUser, Name = "CourtAdmin Provincial Operation Centre", Timezone = "America/Vancouver" },
                new Location { Id = 3, AgencyId = "SS3", CreatedById = User.SystemUser, Name = "Central Float Pool", Timezone = "America/Vancouver" },
                new Location { Id = 4, AgencyId = "SS4", CreatedById = User.SystemUser, Name = "ITAU", Timezone = "America/Vancouver" },
                new Location { Id = 5, AgencyId = "SS5", CreatedById = User.SystemUser, Name = "Office of the Chief CourtAdmin", Timezone = "America/Vancouver" },
                new Location { Id = 6, AgencyId = "SS6", JustinCode = "4882", CreatedById = User.SystemUser, Name = "South Okanagan Escort Centre", Timezone = "America/Vancouver" }
            );

            builder.HasOne(b => b.Region).WithMany().HasForeignKey(m => m.RegionId).OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(b => b.AgencyId).IsUnique();

            base.Configure(builder);
        }
    }
}
