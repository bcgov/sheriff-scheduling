using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CAS.DB.Configuration;
using CAS.DB.models;
using CAS.DB.models.auth;
using CAS.DB.models.lookupcodes;

namespace CAS.DB.configuration
{
    public class LookupCodeOrderConfiguration : BaseEntityConfiguration<LookupCode>
    {
        public override void Configure(EntityTypeBuilder<LookupCode> builder)
        {
            builder.Property(b => b.Id).HasIdentityOptions(startValue: 1000);

            builder.HasIndex(lc => new {lc.Type, lc.Code, lc.LocationId}).IsUnique();

            builder.HasOne(b => b.Location).WithMany().HasForeignKey(lc => lc.LocationId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(b => b.SortOrder).WithOne(a=> a.LookupCode).HasForeignKey(lc => lc.LookupCodeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasData(
                new LookupCode { CreatedById = User.SystemUser, Id = 1, Code = "Chief CourtAdmin", Description = "Chief CourtAdmin", Type = LookupTypes.CourtAdminRank },
                new LookupCode { CreatedById = User.SystemUser, Id = 2, Code = "Superintendent", Description = "Superintendent", Type = LookupTypes.CourtAdminRank },
                new LookupCode { CreatedById = User.SystemUser, Id = 3, Code = "Staff Inspector", Description = "Staff Inspector", Type = LookupTypes.CourtAdminRank },
                new LookupCode { CreatedById = User.SystemUser, Id = 4, Code = "Inspector", Description = "Inspector", Type = LookupTypes.CourtAdminRank },
                new LookupCode { CreatedById = User.SystemUser, Id = 5, Code = "Staff Sergeant", Description = "Staff Sergeant", Type = LookupTypes.CourtAdminRank },
                new LookupCode { CreatedById = User.SystemUser, Id = 6, Code = "Sergeant", Description = "Sergeant", Type = LookupTypes.CourtAdminRank },
                new LookupCode { CreatedById = User.SystemUser, Id = 7, Code = "Deputy CourtAdmin", Description = "Deputy CourtAdmin", Type = LookupTypes.CourtAdminRank },
              
                new LookupCode { CreatedById = User.SystemUser, Id = 8, Code = "CEW (Taser)", Description = "CEW (Taser)", Type = LookupTypes.TrainingType },
                new LookupCode { CreatedById = User.SystemUser, Id = 9, Code = "DNA", Description = "DNA", Type = LookupTypes.TrainingType },
                new LookupCode { CreatedById = User.SystemUser, Id = 10, Code = "FRO", Description = "FRO", Type = LookupTypes.TrainingType },
                new LookupCode { CreatedById = User.SystemUser, Id = 11, Code = "Fire Arm", Description = "Fire Arm", Type = LookupTypes.TrainingType },
                new LookupCode { CreatedById = User.SystemUser, Id = 12, Code = "First Aid", Description = "First Aid", Type = LookupTypes.TrainingType },
                new LookupCode { CreatedById = User.SystemUser, Id = 13, Code = "Advanced Escort SPC (AESOC)", Description = "Advanced Escort SPC (AESOC)", Type = LookupTypes.TrainingType },
                new LookupCode { CreatedById = User.SystemUser, Id = 14, Code = "Extenuating Circumstances SPC (EXSPC)", Description = "Extenuating Circumstances SPC (EXSPC)", Type = LookupTypes.TrainingType },
                new LookupCode { CreatedById = User.SystemUser, Id = 15, Code = "Search Gate", Description = "Search Gate", Type = LookupTypes.TrainingType },
                new LookupCode { CreatedById = User.SystemUser, Id = 16, Code = "Other", Description = "Other", Type = LookupTypes.TrainingType },
                
                new LookupCode { CreatedById = User.SystemUser, Id = 17, Code = "STIP", Description = "STIP", Type = LookupTypes.LeaveType},
                new LookupCode { CreatedById = User.SystemUser, Id = 18, Code = "Annual", Description = "Annual", Type = LookupTypes.LeaveType },
                new LookupCode { CreatedById = User.SystemUser, Id = 19, Code = "Illness", Description = "Illness", Type = LookupTypes.LeaveType },
                new LookupCode { CreatedById = User.SystemUser, Id = 20, Code = "Special", Description = "Special", Type = LookupTypes.LeaveType }
            );
            base.Configure(builder);
        }
    }
}
