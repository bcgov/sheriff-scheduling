using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CAS.DB.Configuration;
using CAS.DB.models.auth;

namespace CAS.DB.configuration
{
    public class RoleConfiguration : BaseEntityConfiguration<Role>
    {
        public override void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.Property(b => b.Id).HasIdentityOptions(startValue: 50);

            builder.HasData(
                new Role { CreatedById = User.SystemUser, Id = 1, Name = Role.Administrator, Description = "Administrator" },
                new Role { CreatedById = User.SystemUser, Id = 2, Name = Role.Manager, Description = "Manager" },
                new Role { CreatedById = User.SystemUser, Id = 3, Name = Role.CourtAdmin, Description = "CourtAdmin" }
            );

            base.Configure(builder);
        }
    }
}
