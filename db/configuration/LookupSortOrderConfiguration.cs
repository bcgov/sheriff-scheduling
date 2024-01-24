using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CAS.DB.Configuration;
using CAS.DB.models.lookupcodes;
using CAS.DB.models.auth;

namespace CAS.DB.configuration
{
    public class LookupSortOrderConfiguration : BaseEntityConfiguration<LookupSortOrder>
    {
        public override void Configure(EntityTypeBuilder<LookupSortOrder> builder)
        {
            builder.Property(b => b.Id).HasIdentityOptions(startValue: 1000);

            builder.HasOne(b => b.Location).WithMany().HasForeignKey(m => m.LocationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(b => b.LookupCode).WithMany(a => a.SortOrder).OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(lc => new { lc.LookupCodeId, lc.LocationId }).IsUnique();

            builder.HasData(
                new LookupSortOrder { CreatedById = User.SystemUser, Id = 1, LookupCodeId = 1, SortOrder = 1},
                new LookupSortOrder { CreatedById = User.SystemUser, Id = 2, LookupCodeId = 2, SortOrder = 2},
                new LookupSortOrder { CreatedById = User.SystemUser, Id = 3, LookupCodeId = 3, SortOrder = 3},
                new LookupSortOrder { CreatedById = User.SystemUser, Id = 4, LookupCodeId = 4, SortOrder = 4},
                new LookupSortOrder { CreatedById = User.SystemUser, Id = 5, LookupCodeId = 5, SortOrder = 5},
                new LookupSortOrder { CreatedById = User.SystemUser, Id = 6, LookupCodeId = 6, SortOrder = 6},
                new LookupSortOrder { CreatedById = User.SystemUser, Id = 7, LookupCodeId = 7, SortOrder = 7}
            );

            builder.HasIndex(b => b.LookupCodeId);

            base.Configure(builder);
        }
    }
}
