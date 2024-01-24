using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CAS.API.controllers.scheduling;
using CAS.API.Models.DB;
using CAS.API.services.scheduling;
using CAS.DB.models;
using CAS.DB.models.scheduling;
using tests.api.helpers;
using tests.api.Helpers;
using Xunit;

namespace tests.controllers
{
    //Sequential, because there are issues with Adding Location (with a unique index) within a TransactionScope.
    [Collection("Sequential")]
    public class AssignmentControllerTests : WrapInTransactionScope
    {
        #region Fields
        private readonly AssignmentController _controller;
        #endregion Fields

        public AssignmentControllerTests() : base(false)
        {
            _controller = new AssignmentController(new AssignmentService(Db), Db)
            {
                ControllerContext = HttpResponseTest.SetupMockControllerContext()
            };
        }
        [Fact]
        public async Task TestExpireAssignment()
        {
            var newLocation = new Location {Id = 50000, AgencyId = "44"};
            var assignment = new Assignment
            {
                LookupCode =  new LookupCode { Id = 5000000 },
                Location = newLocation,
                Monday = true,
                Tuesday = true,
                Wednesday = true,
                Thursday = true,
                Friday = true,
                Saturday = true,
                Sunday = true,
                LocationId = newLocation.Id,
                AdhocStartDate = DateTimeOffset.UtcNow.AddDays(-1),
                AdhocEndDate = DateTimeOffset.UtcNow.AddDays(7),
                Timezone = "America/Edmonton"
            };

            await Db.Assignment.AddAsync(assignment);
            await Db.SaveChangesAsync();

            var duty = new Duty
            {
                Id = 1,
                AssignmentId = assignment.Id,
                ExpiryDate = null,
                LocationId = newLocation.Id,
                StartDate = DateTimeOffset.UtcNow.AddDays(1),
                DutySlots = new List<DutySlot>
                {
                    new DutySlot
                    {
                        LocationId = newLocation.Id,
                        ExpiryDate = null
                    }
                }
            };

            var duty2 = new Duty
            {
                Id = 2,
                AssignmentId = assignment.Id,
                ExpiryDate = null,
                LocationId = newLocation.Id,
                StartDate = DateTimeOffset.UtcNow,
                DutySlots = new List<DutySlot>
                    {
                        new DutySlot
                        {
                            LocationId = newLocation.Id,
                            ExpiryDate = null
                        }
                    }
            };

            await Db.Duty.AddAsync(duty);

            await Db.Duty.AddAsync(duty2);
            await Db.SaveChangesAsync();

            var controllerResult3 = await _controller.GetAssignments(newLocation.Id, DateTimeOffset.UtcNow, assignment.AdhocEndDate);
            var hasCount1 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult3);
            Assert.NotEmpty(hasCount1);

            HttpResponseTest.CheckForNoContentResponse(await _controller.ExpireAssignment(assignment.Id, "EXPIRED", null));
            var controllerResult = await _controller.GetAssignments(newLocation.Id, DateTimeOffset.UtcNow, assignment.AdhocEndDate);
            var zeroCount = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);
            Assert.Empty(zeroCount);

            var controllerResult2 = await _controller.GetAssignments(newLocation.Id, assignment.AdhocStartDate.Value.AddDays(-1), assignment.AdhocEndDate);
            var hasCount2 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult2);
            Assert.NotEmpty(hasCount2);
        }

    }
}
