using Mapster;
using CAS.API.controllers.scheduling;
using CAS.API.infrastructure.exceptions;
using CAS.API.models.dto.generated;
using CAS.API.Models.DB;
using CAS.API.services.scheduling;
using CAS.API.services.usermanagement;
using CAS.COMMON.helpers.extensions;
using CAS.DB.models.scheduling;
using CAS.DB.models.courtAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using tests.api.helpers;
using tests.api.Helpers;
using Xunit;

namespace tests.controllers
{
    //Sequential, because there are issues with Adding Location (with a unique index) within a TransactionScope.
    [Collection("Sequential")]
    public class DutyRosterControllerTests : WrapInTransactionScope
    {
        #region Fields
        private readonly DutyRosterController _controller;
        #endregion Fields

        public DutyRosterControllerTests() : base(false)
        {
            var environment = new EnvironmentBuilder("LocationServicesClient:Username", "LocationServicesClient:Password", "LocationServicesClient:Url");
            var shiftService = new ShiftService(Db, new CourtAdminService(Db, environment.Configuration),
                environment.Configuration);
            var dutyRosterService = new DutyRosterService(Db, environment.Configuration,
                 shiftService, environment.LogFactory.CreateLogger<DutyRosterService>());

            _controller = new DutyRosterController(dutyRosterService, Db, environment.Configuration)
            {
                ControllerContext = HttpResponseTest.SetupMockControllerContext()
            };
        }

        [Fact]
        public async Task GetDuties()
        {
            var locationId = await CreateLocation();
            var startDate = DateTimeOffset.UtcNow;
            var addDuty = new Duty
            {
                Id = 1,
                StartDate = startDate,
                EndDate = startDate.AddDays(5),
                LocationId = locationId
            };

            var addDutyExpiryDate = new Duty
            {
                Id = 2,
                StartDate = startDate,
                EndDate = startDate.AddDays(5),
                ExpiryDate = startDate,
                LocationId = locationId
            };

            await Db.Duty.AddAsync(addDuty);
            await Db.Duty.AddAsync(addDutyExpiryDate);
            await Db.SaveChangesAsync();

            var controllerResult = await _controller.GetDuties(locationId, startDate, startDate.AddDays(1));
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);
            Assert.Single(response);
            Assert.Equal(1, response.First().Id);
        }

        [Fact]
        public async Task AddDuty()
        {
            var locationId = await CreateLocation();
            var startDate = DateTimeOffset.UtcNow;
            var addDuty = new AddDutyDto
            {
                StartDate = startDate,
                EndDate = startDate.AddDays(5),
                LocationId = locationId,
                Timezone = "America/Vancouver"
            };
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await _controller.AddDuties( new List<AddDutyDto> {addDuty}));
            Assert.NotNull(response);
            Assert.Equal(addDuty.StartDate, response.First().StartDate);
            Assert.Equal(addDuty.EndDate, response.First().EndDate);
        }

        [Fact]
        public async Task ExpireDuty()
        {
            var locationId = await CreateLocation();
            var startDate = DateTimeOffset.UtcNow.AddYears(5);
            var addDuty = new Duty
            {
                Id = 1,
                StartDate = startDate,
                EndDate = startDate.AddDays(5),
                LocationId = locationId
            };
            await Db.Duty.AddAsync(addDuty);
            await Db.SaveChangesAsync();

            HttpResponseTest.CheckForNoContentResponse(await _controller.ExpireDuties(new List<int> {1}));

            var controllerResult = await _controller.GetDuties(locationId, startDate, startDate.AddDays(1));
            var getDuties = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);
            Assert.Empty(getDuties);
        }

        [Fact]
        public async Task DutySlotOverlapSelf()
        {
            var locationId = await CreateLocation();
            var newCourtAdminId = await CreateCourtAdmin(locationId);

            var startDate = DateTimeOffset.UtcNow.AddYears(5);
            var addDuty = new AddDutyDto
            {
                LocationId = locationId,
                StartDate = startDate, 
                EndDate = startDate.AddDays(5),
                Timezone = "America/Vancouver"
            };
            HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await _controller.AddDuties(new List<AddDutyDto> {addDuty } ));

            var controllerResult = await _controller.GetDuties(locationId, startDate, startDate.AddDays(5));
            var getDuties = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);

            Assert.Single(getDuties);
            var duty = getDuties.First().Adapt<UpdateDutyDto>();

            duty.DutySlots = new List<UpdateDutySlotDto>()
            {
                new UpdateDutySlotDto
                {
                    StartDate = startDate,
                    EndDate = startDate.AddDays(5),
                    CourtAdminId = newCourtAdminId
                },
                new UpdateDutySlotDto
                {
                    StartDate = startDate,
                    EndDate = startDate.AddDays(5),
                    CourtAdminId = newCourtAdminId
                }
            };

            //Should throw self conflicting exception.
            await Assert.ThrowsAsync<BusinessLayerException>(async () => await  _controller.UpdateDuties(new List<UpdateDutyDto> { duty }));

            duty.DutySlots = new List<UpdateDutySlotDto>()
            {
                new UpdateDutySlotDto
                {
                    StartDate = startDate,
                    EndDate = startDate.AddDays(5),
                    CourtAdminId = newCourtAdminId
                },
                new UpdateDutySlotDto
                {
                    StartDate = startDate.AddDays(5),
                    EndDate = startDate.AddDays(6),
                    CourtAdminId = newCourtAdminId
                }
            };

            //Shouldn't overlap. 
            var controllerResult2 = await _controller.UpdateDuties(new List<UpdateDutyDto> { duty });
            var getDuties2 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult2);
        }

        [Fact]
        public async Task DutySlotOverlap()
        {
            var locationId = await CreateLocation();

            var newCourtAdminId = await CreateCourtAdmin(locationId);

            var startDate = DateTimeOffset.UtcNow.AddYears(5);

            var duty = new Duty
            {
                Id = 1,
                LocationId = locationId,
                StartDate = startDate,
                EndDate = startDate.AddDays(5),
                Timezone = "America/Vancouver"
            };

            var duty2 = new Duty
            {
                Id = 2,
                LocationId = locationId,
                StartDate = startDate,
                EndDate = startDate.AddDays(5),
                Timezone = "America/Vancouver"
            };

            await Db.Duty.AddAsync(duty);
            await Db.Duty.AddAsync(duty2);
            await Db.SaveChangesAsync();

            var controllerResult = await _controller.GetDuties(locationId, startDate, startDate.AddDays(5));
            var getDuties = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);
            Assert.NotNull(getDuties);
            Assert.NotNull(getDuties.FirstOrDefault(d => d.Id == 1));
            Assert.NotNull(getDuties.FirstOrDefault(d => d.Id == 2));

            var updateDuty = getDuties.FirstOrDefault(d => d.Id == 1)!.Adapt<UpdateDutyDto>();
            updateDuty.DutySlots = new List<UpdateDutySlotDto>
            {
                new UpdateDutySlotDto
                {
                    StartDate = startDate,
                    EndDate = startDate.AddDays(5),
                    CourtAdminId = newCourtAdminId
                }
            };

            var controllerResult2 = await _controller.UpdateDuties(new List<UpdateDutyDto> {updateDuty} );
            HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult2);

            var updateDuty2 = getDuties.FirstOrDefault(d => d.Id == 2)!.Adapt<UpdateDutyDto>();
            updateDuty2.DutySlots = new List<UpdateDutySlotDto>
            {
                new UpdateDutySlotDto
                {
                    StartDate = startDate,
                    EndDate = startDate.AddDays(5),
                    CourtAdminId = newCourtAdminId
                }
            };

            //Should conflict. 
            await Assert.ThrowsAsync<BusinessLayerException>(async () => await _controller.UpdateDuties(new List<UpdateDutyDto> { updateDuty2 }));
        }

        [Fact]
        public async Task AddUpdateRemoveDutySlots()
        {
            var locationId = await CreateLocation();
            var newCourtAdminId = await CreateCourtAdmin(locationId);

            var startDate = DateTimeOffset.UtcNow.AddYears(5);

            var duty = new Duty
            {
                Id = 1,
                LocationId = locationId,
                StartDate = startDate,
                EndDate = startDate.AddDays(5),
                Timezone = "America/Vancouver"
            };

            await Db.Duty.AddAsync(duty);
            await Db.SaveChangesAsync();

            var controllerResult = await _controller.GetDuties(locationId, startDate, startDate.AddDays(5));
            var getDuties = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);
            Assert.NotEmpty(getDuties);

            
            var updateDuty = getDuties.FirstOrDefault(d => d.Id == 1)!.Adapt<UpdateDutyDto>();
            updateDuty.DutySlots = new List<UpdateDutySlotDto>
            {
                //Add.
                new UpdateDutySlotDto
                {
                    StartDate = startDate,
                    EndDate = startDate.AddDays(5),
                    CourtAdminId = newCourtAdminId
                },
                //Add.
                new UpdateDutySlotDto
                {
                    StartDate = startDate.AddDays(5),
                    EndDate = startDate.AddDays(10),
                    CourtAdminId = newCourtAdminId
                },
            };

            var controllerResult2 = await _controller.UpdateDuties(new List<UpdateDutyDto> { updateDuty });
            var updateDuty2 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult2);

            updateDuty.DutySlots = new List<UpdateDutySlotDto>
            {
                //Update
                new UpdateDutySlotDto
                {
                    Id = updateDuty2.First().DutySlots.First().Id,
                    StartDate = startDate.AddDays(10),
                    EndDate = startDate.AddDays(15),
                    CourtAdminId = newCourtAdminId
                },
                //Add
                new UpdateDutySlotDto
                {
                    StartDate = startDate,
                    EndDate = startDate.AddDays(5),
                    CourtAdminId = newCourtAdminId
                },
                //Implicit remove of one. 
            };

            var controllerResult3 = await _controller.UpdateDuties(new List<UpdateDutyDto> { updateDuty });
            HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult3);
        }
       
        [Fact]
        public async Task MoveDuty()
        {
            var locationId = await CreateLocation();
            var newCourtAdminId = await CreateCourtAdmin(locationId);

            //Create a duty from 9 -> 5 pm
            var startDate = DateTimeOffset.UtcNow.AddYears(5).ConvertToTimezone("America/Vancouver");
            startDate = startDate.Date.AddHours(9);
            var endDate = startDate.Date.AddHours(9 + 8);

            var fromDutySlot = new DutySlot
            {
                Id = 50000,
                DutyId = 50000,
                StartDate = startDate,
                EndDate = endDate,
                CourtAdminId = newCourtAdminId,
                LocationId = locationId
            };

            var fromDuty = new Duty
            {
                Id = 50000,
                LocationId = locationId,
                StartDate = startDate,
                EndDate = endDate,
                Timezone = "America/Vancouver",
                DutySlots = new List<DutySlot>
                {
                   fromDutySlot
                }
            };

            var shift = new Shift
            {
                Id = 50000,
                LocationId = locationId,
                StartDate = startDate,
                EndDate = startDate.Date.AddHours(20),
                Timezone = "America/Vancouver",
                CourtAdminId = newCourtAdminId
            };

            await Db.Duty.AddAsync(fromDuty);
            await Db.Shift.AddAsync(shift);
            await Db.SaveChangesAsync();

            //It ends early and you'd like to move someone from 3pm -> 5pm into another duty

            //CASE works perfectly, Duty has no slots
            var toDuty = new Duty
            {
                Id = 50001,
                LocationId = locationId,
                StartDate = startDate,
                EndDate = endDate,
                Timezone = "America/Vancouver"
            };

            await Db.Duty.AddAsync(toDuty);
            await Db.SaveChangesAsync();

            var controllerResult = await _controller.MoveCourtAdminFromDutySlot(fromDutySlot.Id, toDuty.Id, startDate.AddHours(6));
            var newDuty = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);

            Assert.Equal(startDate.AddHours(6),newDuty.DutySlots.FirstOrDefault().StartDate);
            Assert.Equal(startDate.AddHours(8),newDuty.DutySlots.FirstOrDefault().EndDate);

            Db.Duty.Remove(toDuty);
            Db.Duty.Remove(fromDuty);
            await Db.SaveChangesAsync();

            fromDuty.DutySlots.First().EndDate = endDate;
            await Db.Duty.AddAsync(fromDuty);
            await Db.SaveChangesAsync();

            //CASE Duty has a blank slot (take over the slot)
            toDuty = new Duty
            {
                Id = 50001,
                LocationId = locationId,
                StartDate = startDate,
                EndDate = endDate,
                Timezone = "America/Vancouver",
                DutySlots = new List<DutySlot>
                {
                    new DutySlot
                    {
                        Id = 50001,
                        DutyId = 50001,
                        StartDate = startDate,
                        EndDate = endDate,
                        LocationId = locationId
                    }
                }
            };
            await Db.Duty.AddAsync(toDuty);
            await Db.SaveChangesAsync();
            
            controllerResult = await _controller.MoveCourtAdminFromDutySlot(fromDutySlot.Id, toDuty.Id, startDate.AddHours(6));
            newDuty = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);

            Assert.Equal(startDate.AddHours(6), newDuty.DutySlots.FirstOrDefault().StartDate);
            Assert.Equal(startDate.AddHours(8), newDuty.DutySlots.FirstOrDefault().EndDate);

            Db.Duty.Remove(toDuty);
            Db.Duty.Remove(fromDuty);
            await Db.SaveChangesAsync();

            fromDuty.DutySlots.First().EndDate = endDate;
            await Db.Duty.AddAsync(fromDuty);
            await Db.SaveChangesAsync();

            //CASE A slot conflicts at 4 pm, so it should be moved from 3 -> 4pm 
            toDuty = new Duty
            {
                Id = 50001,
                LocationId = locationId,
                StartDate = startDate,
                EndDate = endDate,
                Timezone = "America/Vancouver",
                DutySlots = new List<DutySlot>
                {
                    new DutySlot
                    {
                        Id = 50001,
                        DutyId = 50001,
                        StartDate = startDate.Date.AddHours(16),
                        EndDate = startDate.Date.AddHours(17),
                        CourtAdminId = newCourtAdminId,
                        LocationId = locationId
                    }
                }
            };
            await Db.Duty.AddAsync(toDuty);
            await Db.SaveChangesAsync();

            controllerResult = await _controller.MoveCourtAdminFromDutySlot(fromDutySlot.Id, toDuty.Id, startDate.AddHours(6));
            newDuty = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);

            Assert.Equal(startDate.AddHours(6), newDuty.DutySlots.FirstOrDefault(ds => ds.Id != 50001).StartDate);
            Assert.Equal(startDate.AddHours(7), newDuty.DutySlots.FirstOrDefault(ds => ds.Id != 50001).EndDate);

            Db.Duty.Remove(toDuty);
            Db.Duty.Remove(fromDuty);
            await Db.SaveChangesAsync();

            fromDuty.DutySlots.First().EndDate = endDate;
            await Db.Duty.AddAsync(fromDuty);
            await Db.SaveChangesAsync();

            //CASE Duty ends early so it should be only created from 3 -> 4 pm
            toDuty = new Duty
            {
                Id = 50001,
                LocationId = locationId,
                StartDate = startDate,
                EndDate = startDate.Date.AddHours(16),
                Timezone = "America/Vancouver",
            };
            await Db.Duty.AddAsync(toDuty);
            await Db.SaveChangesAsync();

            controllerResult = await _controller.MoveCourtAdminFromDutySlot(fromDutySlot.Id, toDuty.Id, startDate.AddHours(6));
            newDuty = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);

            Assert.Equal(startDate.AddHours(6), newDuty.DutySlots.FirstOrDefault().StartDate);
            Assert.Equal(startDate.AddHours(7), newDuty.DutySlots.FirstOrDefault().EndDate);

            Db.Duty.Remove(toDuty);
            Db.Duty.Remove(fromDuty);
            await Db.SaveChangesAsync();

            fromDuty.DutySlots.First().EndDate = endDate;
            await Db.Duty.AddAsync(fromDuty);
            await Db.SaveChangesAsync();

            //CASE Duty impossible to move, already someone scheduled for duty/dutyslots.
            toDuty = new Duty
            {
                Id = 50001,
                LocationId = locationId,
                StartDate = startDate,
                EndDate = endDate,
                Timezone = "America/Vancouver",
                DutySlots = new List<DutySlot>
                {
                    new DutySlot
                    {
                        Id = 50001,
                        DutyId = 50001,
                        StartDate = startDate,
                        EndDate = endDate,
                        CourtAdminId = newCourtAdminId,
                        LocationId = locationId
                    }
                }
            };
            await Db.Duty.AddAsync(toDuty);
            await Db.SaveChangesAsync();

            await Assert.ThrowsAsync<BusinessLayerException>(async () => await _controller.MoveCourtAdminFromDutySlot(fromDutySlot.Id, toDuty.Id, startDate.AddHours(6)));
        }
        private async Task<Guid> CreateCourtAdmin(int locationId)
        {
            var newCourtAdmin = new CourtAdmin
            {
                FirstName = "Ted",
                LastName = "Tums",
                HomeLocationId = locationId,
                IsEnabled = true
            };

            await Db.CourtAdmin.AddAsync(newCourtAdmin);
            await Db.SaveChangesAsync();
            return newCourtAdmin.Id;
        }

        private async Task<int> CreateLocation()
        {
            var location = new Location { Id = 50000, AgencyId = "5555", Name = "dfd" };
            await Db.Location.AddAsync(location);
            await Db.SaveChangesAsync();
            return location.Id;
        }
    }
}
