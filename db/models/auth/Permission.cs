using System.ComponentModel.DataAnnotations;
using db.models;
using Mapster;

namespace SS.Db.models.auth
{
    [AdaptTo("[name]Dto")]
    public class Permission : BaseEntity
    {
        public const string Login = nameof(Login);
        public const string CreateUsers = nameof(CreateUsers);
        public const string ExpireUsers = nameof(ExpireUsers);
        public const string EditUsers = nameof(EditUsers);
        public const string ViewRoles = nameof(ViewRoles);
        public const string CreateAndAssignRoles = nameof(CreateAndAssignRoles);
        public const string ExpireRoles = nameof(ExpireRoles);
        public const string EditRoles = nameof(EditRoles);
        public const string CreateTypes = nameof(CreateTypes);
        public const string EditTypes = nameof(EditTypes);
        public const string ExpireTypes = nameof(ExpireTypes);
        public const string ViewShifts = nameof(ViewShifts);
        public const string CreateAndAssignShifts = nameof(CreateAndAssignShifts);
        public const string ExpireShifts = nameof(ExpireShifts);
        public const string EditShifts = nameof(EditShifts);
        public const string ImportShifts = nameof(ImportShifts);
        public const string ViewDistributeSchedule = nameof(ViewDistributeSchedule);
        public const string ViewHomeLocation = nameof(ViewHomeLocation);
        public const string ViewAssignedLocation = nameof(ViewAssignedLocation);
        public const string ViewRegion = nameof(ViewRegion);
        public const string ViewProvince = nameof(ViewProvince);
        public const string ExpireLocation = nameof(ExpireLocation);
        public const string CreateAssignments = nameof(CreateAssignments);
        public const string EditAssignments = nameof(EditAssignments);
        public const string ExpireAssignments = nameof(ExpireAssignments);
        public const string ViewDutyRoster = nameof(ViewDutyRoster);
        public const string CreateAndAssignDuties = nameof(CreateAndAssignDuties);
        public const string EditDuties = nameof(EditDuties);
        public const string ExpireDuties = nameof(ExpireDuties);
        public const string EditIdir = nameof(EditIdir);
        public const string EditPastTraining = nameof(EditPastTraining);
        public const string RemovePastTraining = nameof(RemovePastTraining);
        public const string ViewDutyRosterInFuture = nameof(ViewDutyRosterInFuture);
        public const string ViewAllFutureShifts = nameof(ViewAllFutureShifts);
        public const string ViewOtherProfiles = nameof(ViewOtherProfiles);
        public const string GenerateReports = nameof(GenerateReports);
        public const string AdjustTrainingExpiry = nameof(AdjustTrainingExpiry);
        public const string ExemptFromTraining = nameof(ExemptFromTraining);

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
