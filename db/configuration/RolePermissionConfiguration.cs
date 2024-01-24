using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CAS.DB.Configuration;
using CAS.DB.models.auth;

namespace CAS.DB.configuration
{
    public class RolePermissionConfiguration : BaseEntityConfiguration<RolePermission>
    {
        public override void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.Property(b => b.Id).HasIdentityOptions(startValue: 100);

            base.Configure(builder);
        }
    }
}
