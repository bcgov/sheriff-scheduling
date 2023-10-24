using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CAS.DB.models.auth;
using CAS.DB.Configuration;

namespace CAS.DB.configuration
{
    public class PermissionConfiguration : BaseEntityConfiguration<Permission>
    {
        public override void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.Property(b => b.Id).HasIdentityOptions(startValue: 200);

            builder.HasData(
                new Permission { Id = 1, Name = Permission.Login, Description = "Allows the user to login."},
                new Permission { Id = 5, Name = Permission.CreateUsers, Description = "Create Profile (User)" },
                new Permission { Id = 6, Name = Permission.ExpireUsers, Description = "Expire Profile (User)" },
                new Permission { Id = 7, Name = Permission.EditUsers, Description = "Edit Profile (User)" },
                new Permission { Id = 8, Name = Permission.ViewRoles, Description = "View all Roles" },
                new Permission { Id = 9, Name = Permission.CreateAndAssignRoles, Description = "Create and Assign Roles" },
                new Permission { Id = 10, Name = Permission.ExpireRoles, Description = "Expire Roles" },
                new Permission { Id = 11, Name = Permission.EditRoles, Description = "Edit Roles" },
                new Permission { Id = 13, Name = Permission.CreateTypes, Description = "Create Types" },
                new Permission { Id = 14, Name = Permission.EditTypes, Description = "Edit Types" },
                new Permission { Id = 15, Name = Permission.ExpireTypes, Description = "Expire Types" },
                new Permission { Id = 16, Name = Permission.ViewShifts, Description = "View shifts" },
                new Permission { Id = 19, Name = Permission.CreateAndAssignShifts, Description = "Create and Assign Shifts" },
                new Permission { Id = 20, Name = Permission.ExpireShifts, Description = "Expire Shifts" },
                new Permission { Id = 21, Name = Permission.EditShifts, Description = "Edit Shifts" },
                new Permission { Id = 22, Name = Permission.ViewDistributeSchedule, Description = "View Distribute Schedule" },
                new Permission { Id = 23, Name = Permission.ViewAssignedLocation, Description = "View Assigned Location" },
                new Permission { Id = 24, Name = Permission.ViewRegion, Description = "View Region (all locations within region)" },
                new Permission { Id = 25, Name = Permission.ViewProvince, Description = "View Province (all regions, all locations)" },
                new Permission { Id = 27, Name = Permission.ViewHomeLocation, Description = "View Home Location" },
                new Permission { Id = 28, Name = Permission.ImportShifts, Description = "Import Shifts" },
                new Permission { Id = 30, Name = Permission.CreateAssignments, Description = "Create Assignments" },
                new Permission { Id = 31, Name = Permission.EditAssignments, Description = "Edit Assignments" },
                new Permission { Id = 32, Name = Permission.ExpireAssignments, Description = "Expire Assignments" },
                new Permission { Id = 33, Name = Permission.ViewDutyRoster, Description = "View Duties" },
                new Permission { Id = 34, Name = Permission.CreateAndAssignDuties, Description = "Create Duties" },
                new Permission { Id = 35, Name = Permission.EditDuties, Description = "Edit Duties" },
                new Permission { Id = 36, Name = Permission.ExpireDuties, Description = "Expire Duties" },
                new Permission { Id = 37, Name = Permission.EditIdir, Description = "Edit Idir" },
                new Permission { Id = 38, Name = Permission.EditPastTraining, Description = "Edit Past Training" },
                new Permission { Id = 39, Name = Permission.RemovePastTraining, Description = "Remove Past Training" },
                new Permission { Id = 40, Name = Permission.ViewDutyRosterInFuture, Description = "View DutyRoster in the future" },
                new Permission { Id = 41, Name = Permission.ViewAllFutureShifts, Description = "View Shifts in the future (not time constrained)" },
                new Permission { Id = 42, Name = Permission.ViewOtherProfiles, Description = "View other profiles (beside their own)" },
                new Permission { Id = 43, Name = Permission.GenerateReports, Description = "Generate Reports based on Sheriff's activity" }
            );
            base.Configure(builder);
        }
    }
}
