using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS.DB.Configuration;
using SS.Db.models.sheriff;
using SS.Db.models.auth;

namespace SS.Db.configuration
{
    public class SheriffStandardTrainingConfiguration : BaseEntityConfiguration<SheriffStandardTraining>
    {
        public override void Configure(EntityTypeBuilder<SheriffStandardTraining> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id).HasIdentityOptions(startValue: 2000);
           
            builder.Property(b => b.TrainingTypeId).IsRequired();
           
            builder.HasIndex(b => b.TrainingTypeId).IsUnique();
           
            builder.HasOne(b => b.TrainingType)
                .WithOne()
                .HasForeignKey<SheriffStandardTraining>(b => b.TrainingTypeId);
            
             builder.HasData(
                // Emergency Vehicle Operator (JIBC Website)
                new SheriffStandardTraining { Id = 1, TrainingTypeId = 1718748, ExpiryInYears = 100, ExpiryMonth = 12, ExpiryDate = 31, CreatedById = User.SystemUser },
                // Narcan
                new SheriffStandardTraining { Id = 2, TrainingTypeId = 1716961, ExpiryInYears = 0, ExpiryMonth = 12, ExpiryDate = 31, CreatedById = User.SystemUser },
                // CEW Training (Taser)
                new SheriffStandardTraining { Id = 3, TrainingTypeId = 1718646, ExpiryInYears = 0, ExpiryMonth = 12, ExpiryDate = 31, CreatedById = User.SystemUser },
                // Annual Review of Standards of Conduct and Oath of Employment
                new SheriffStandardTraining { Id = 4, TrainingTypeId = 1716948, ExpiryInYears = 0, ExpiryMonth = 5, ExpiryDate = 31, ConditionOn = "Month", ConditionOperator = ">", ConditionValue = 5, ConditionExpiryInYears = 1, ConditionExpiryMonth = 5, ConditionExpiryDate = 31, CreatedById = User.SystemUser },
                // Legal Studies Refresher
                new SheriffStandardTraining { Id = 5, TrainingTypeId = 1716960, ExpiryInYears = 1, ExpiryMonth = 12, ExpiryDate = 31, CreatedById = User.SystemUser },
                // Bullying and Harassment (Respectful Workplace)
                new SheriffStandardTraining { Id = 6, TrainingTypeId = 1716951, ExpiryInYears = 1, ExpiryMonth = 12, ExpiryDate = 31, CreatedById = User.SystemUser },
                // First Aid
                new SheriffStandardTraining { Id = 7, TrainingTypeId = 1716955, ExpiryInYears = 2, ExpiryMonth = 12, ExpiryDate = 31, CreatedById = User.SystemUser },
                // CID - Critical Incident De-escalation
                new SheriffStandardTraining { Id = 8, TrainingTypeId = 1718650, ExpiryInYears = 2, ExpiryMonth = 12, ExpiryDate = 31, CreatedById = User.SystemUser },
                // Fraud Awareness and Prevention
                new SheriffStandardTraining { Id = 9, TrainingTypeId = 1716956, ExpiryInYears = 2, ExpiryMonth = 12, ExpiryDate = 31, CreatedById = User.SystemUser },
                // IM 117
                new SheriffStandardTraining { Id = 10, TrainingTypeId = 1716958, ExpiryInYears = 1, ExpiryMonth = 12, ExpiryDate = 31, CreatedById = User.SystemUser },
                // Diversity and Inclusion Essentials
                new SheriffStandardTraining { Id = 11, TrainingTypeId = 1716954, ExpiryInYears = 100, ExpiryMonth = 12, ExpiryDate = 31, CreatedById = User.SystemUser },
                // Indigenous Cultural Awareness (ICAP)
                new SheriffStandardTraining { Id = 12, TrainingTypeId = 1716959, ExpiryInYears = 100, ExpiryMonth = 12, ExpiryDate = 31, CreatedById = User.SystemUser }
            );

            base.Configure(builder);
        }
    }
}
