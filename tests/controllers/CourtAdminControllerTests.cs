using System;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Http;
using CAS.API.controllers.usermanagement;
using CAS.API.infrastructure.exceptions;
using CAS.API.Models.DB;
using CAS.API.models.dto;
using CAS.DB.models;
using CAS.DB.models.courtAdmin;
using tests.api.helpers;
using tests.api.Helpers;
using Xunit;
using CAS.API.models.dto.generated;
using CAS.API.services.usermanagement;
using Microsoft.Extensions.Logging;
using CAS.DB.models.scheduling;
using System.Linq;
using CAS.API.services.scheduling;

namespace tests.controllers
{
    public class CourtAdminControllerTests : WrapInTransactionScope
    {
        #region Variables
        private readonly CourtAdminController _controller;

        #endregion Variables

        public CourtAdminControllerTests() : base(false)
        {
            var environment = new EnvironmentBuilder("LocationServicesClient:Username", "LocationServicesClient:Password", "LocationServicesClient:Url");
            var httpContextAccessor = new HttpContextAccessor {HttpContext = HttpResponseTest.SetupHttpContext()};

            var courtAdminService = new CourtAdminService(Db, environment.Configuration, httpContextAccessor);
            var shiftService = new ShiftService(Db,courtAdminService, environment.Configuration);
            var dutyRosterService = new DutyRosterService(Db, environment.Configuration, shiftService, environment.LogFactory.CreateLogger<DutyRosterService>());
            _controller = new CourtAdminController(courtAdminService, dutyRosterService, shiftService,new UserService(Db), environment.Configuration, Db)
            {
                ControllerContext = HttpResponseTest.SetupMockControllerContext()
            };
        }

        [Fact]
        public async Task CreateCourtAdmin()
        {
            var newCourtAdmin = new CourtAdmin
            {
                FirstName = "Ted",
                LastName = "Tums",
                BadgeNumber = "555",
                Email = "Ted@Teddy.com",
                Gender = Gender.Female,
                IdirId = new Guid(),
                IdirName = "ted@fakeidir",
                HomeLocationId = null
            };

            var courtAdminDto = newCourtAdmin.Adapt<CourtAdminWithIdirDto>();
            //Debug.Write(JsonConvert.SerializeObject(courtAdminDto));

            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await _controller.AddCourtAdmin(courtAdminDto));
            var courtAdminResponse = response.Adapt<CourtAdmin>();

            Assert.NotNull(await Db.CourtAdmin.FindAsync(courtAdminResponse.Id));
        }

        [Fact]
        public async Task CreateCourtAdminSameIdir()
        {
            var newCourtAdmin = new CourtAdmin
            {
                FirstName = "Ted",
                LastName = "Tums",
                BadgeNumber = "556",
                Email = "Ted@Teddy.com",
                Gender = Gender.Female,
                IdirId = new Guid(),
                IdirName = "ted@fakeidir",
                HomeLocationId = null
            };

            var courtAdminDto = newCourtAdmin.Adapt<CourtAdminWithIdirDto>();
            //Debug.Write(JsonConvert.SerializeObject(courtAdminDto));

            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await _controller.AddCourtAdmin(courtAdminDto));
            var courtAdminResponse = response.Adapt<CourtAdmin>();

            Assert.NotNull(await Db.CourtAdmin.FindAsync(courtAdminResponse.Id));

            newCourtAdmin.BadgeNumber = "554";
            courtAdminDto = newCourtAdmin.Adapt<CourtAdminWithIdirDto>();
            //Debug.Write(JsonConvert.SerializeObject(courtAdminDto));

            BusinessLayerException ble = null;
            try
            {
                response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(
                    await _controller.AddCourtAdmin(courtAdminDto));
                courtAdminResponse = response.Adapt<CourtAdmin>();
            }
            catch (Exception e)
            {
                Assert.True(e is BusinessLayerException);
                ble = (BusinessLayerException) e;
            }

            Assert.Contains("has IDIR name", ble.Message);
        }


        [Fact]
        public async Task FindCourtAdmin()
        {
            var courtAdminObject = await CreateCourtAdminUsingDbContext();

            var controllerResult = await _controller.GetCourtAdminForTeam(courtAdminObject.Id);
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);
            var courtAdminResponse = response.Adapt<CourtAdmin>();

            //Compare CourtAdmin and CourtAdmin Object.
            Assert.Equal(courtAdminObject.FirstName, courtAdminResponse.FirstName);
            Assert.Equal(courtAdminObject.LastName, courtAdminResponse.LastName);
            Assert.Equal(courtAdminObject.BadgeNumber, courtAdminResponse.BadgeNumber);
            Assert.Equal(courtAdminObject.Email, courtAdminResponse.Email);
            Assert.Equal(courtAdminObject.Gender, courtAdminResponse.Gender);
        }

        [Fact]
        public async Task UpdateCourtAdmin()
        {
            var courtAdminObject = await CreateCourtAdminUsingDbContext();

            var newLocation = new Location { Name = "5", Id = 50005 , AgencyId = "645646464646464"};
            await Db.Location.AddAsync(newLocation);
            var newLocation2 = new Location { Name = "6", Id = 50006, AgencyId = "6456456464" };
            await Db.Location.AddAsync(newLocation2);
            await Db.SaveChangesAsync();

            Detach();

            var updateCourtAdmin = courtAdminObject.Adapt<CourtAdminWithIdirDto>();
            updateCourtAdmin.FirstName = "Al";
            updateCourtAdmin.LastName = "Hoyne";
            updateCourtAdmin.BadgeNumber = "55";
            updateCourtAdmin.Email = "hey@hey.com";
            updateCourtAdmin.Gender = Gender.Other;

            //This object is only used for fetching.
            //updateCourtAdmin.HomeLocation = new LocationDto { Name = "Als place2", Id = 5};
            updateCourtAdmin.HomeLocationId = newLocation.Id;

            Detach();

            var controllerResult = await _controller.UpdateCourtAdmin(updateCourtAdmin);
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);
            var courtAdminResponse = response.Adapt<CourtAdmin>();

            Assert.Equal(updateCourtAdmin.FirstName, courtAdminResponse.FirstName);
            Assert.Equal(updateCourtAdmin.LastName, courtAdminResponse.LastName);
            Assert.Equal(updateCourtAdmin.BadgeNumber, courtAdminResponse.BadgeNumber);
            Assert.Equal(updateCourtAdmin.Email, courtAdminResponse.Email);
            Assert.Equal(updateCourtAdmin.Gender, courtAdminResponse.Gender);

            //Shouldn't be able to update any of the navigation properties here.
            Assert.Empty(courtAdminResponse.AwayLocation);
            Assert.Empty(courtAdminResponse.Training);
            Assert.Empty(courtAdminResponse.Leave);
            //Set to invalid location, should be null.
            //WE didn't set the HomeLocationId here. 
            Assert.NotNull(courtAdminResponse.HomeLocation);

            Detach();

            updateCourtAdmin.HomeLocationId = newLocation2.Id;
            updateCourtAdmin.HomeLocation = newLocation2.Adapt<LocationDto>();
            controllerResult = await _controller.UpdateCourtAdmin(updateCourtAdmin);
            response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);

            Detach();

            var controllerResult2 = await _controller.GetCourtAdminForTeam(courtAdminResponse.Id);
            var response2 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);
            Assert.NotNull(response2.HomeLocation);
            Assert.Equal(newLocation2.Id, response.HomeLocation.Id);
        }



        [Fact]
        public async Task AddUpdateRemoveCourtAdminAwayLocation()
        {
            //Test permissions?
            var courtAdminObject = await CreateCourtAdminUsingDbContext();

            var newLocation = new Location { Name = "New PLace", AgencyId = "545325345353"};
            await Db.Location.AddAsync(newLocation);
            var newLocation2 = new Location { Name = "New PLace", AgencyId = "g435346346363"};
            await Db.Location.AddAsync(newLocation2);
            await Db.SaveChangesAsync();

            var courtAdminAwayLocation = new CourtAdminAwayLocationDto
            {
                CourtAdminId =  courtAdminObject.Id,
                LocationId = newLocation.Id,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddHours(3)
            };

            //Add
            var controllerResult = await _controller.AddCourtAdminAwayLocation(courtAdminAwayLocation);
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);

            Assert.Equal(courtAdminAwayLocation.LocationId, response.Location.Id);
            Assert.Equal(courtAdminAwayLocation.CourtAdminId, response.CourtAdminId);
            Assert.Equal(courtAdminAwayLocation.StartDate, response.StartDate);
            Assert.Equal(courtAdminAwayLocation.EndDate, response.EndDate);

            Detach();

            var controllerResult2 = await _controller.GetCourtAdminForTeam(courtAdminObject.Id);
            var response2 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult2);

            Assert.True(response2.AwayLocation.Count == 1);

            var updateCourtAdminAwayLocation = courtAdminAwayLocation.Adapt<CourtAdminAwayLocationDto>();
            updateCourtAdminAwayLocation.StartDate = DateTime.UtcNow.AddDays(5);
            updateCourtAdminAwayLocation.EndDate = DateTime.UtcNow.AddDays(5);
            updateCourtAdminAwayLocation.LocationId = newLocation2.Id;
            updateCourtAdminAwayLocation.Id = response.Id;

            Detach();

            //Update
            var controllerResult3 = await _controller.UpdateCourtAdminAwayLocation(updateCourtAdminAwayLocation);
            var response3 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult3);

            Assert.Equal(response3.StartDate, updateCourtAdminAwayLocation.StartDate);
            Assert.Equal(response3.EndDate, updateCourtAdminAwayLocation.EndDate);
            Assert.Equal(response3.CourtAdminId, updateCourtAdminAwayLocation.CourtAdminId);

            Detach();

            //Remove
            var controllerResult4 = await _controller.RemoveCourtAdminAwayLocation(response.Id, "hello");
            HttpResponseTest.CheckForNoContentResponse(controllerResult4);

            var controllerResult6 = await _controller.GetCourtAdminForTeam(courtAdminObject.Id);
            var response6 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult6);
            Assert.Empty(response6.AwayLocation);
        }

        [Fact]
        public async Task AddUpdateRemoveCourtAdminLeave()
        {
            var courtAdminObject = await CreateCourtAdminUsingDbContext();
            var newLocation = new Location { Name = "New PLace", AgencyId = "gfgdf43535345"};
            await Db.Location.AddAsync(newLocation);

            await Db.SaveChangesAsync();

            var lookupCode = new LookupCode
            {
                Code = "zz4",
                Description = "gg",
                LocationId = newLocation.Id
            };
            await Db.LookupCode.AddAsync(lookupCode);

            var lookupCode2 = new LookupCode
            {
                Code = "zz",
                Description = "gg",
                LocationId = newLocation.Id
            };

            await Db.LookupCode.AddAsync(lookupCode2);

            await Db.SaveChangesAsync();


            var entity = new CourtAdminLeaveDto
            {
                LeaveTypeId = lookupCode.Id,
                CourtAdminId = courtAdminObject.Id,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddHours(3)
            };

            //Add
            var controllerResult = await _controller.AddCourtAdminLeave(entity);
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);

            Assert.Equal(entity.LeaveTypeId, response.LeaveType.Id);
            Assert.Equal(entity.CourtAdminId, response.CourtAdminId);
            Assert.Equal(entity.StartDate, response.StartDate);
            Assert.Equal(entity.EndDate, response.EndDate);

            var controllerResult2 = await _controller.GetCourtAdminForTeam(courtAdminObject.Id);
            var response2 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult2);

            Assert.True(response2.Leave.Count == 1);

            Detach();

            var updateCourtAdminLeave = entity.Adapt<CourtAdminLeaveDto>();
            updateCourtAdminLeave.StartDate = DateTime.UtcNow.AddDays(5);
            updateCourtAdminLeave.EndDate = DateTime.UtcNow.AddDays(5);
            //updateCourtAdminLeave.LeaveTypeId = lookupCode2.Id;
           
            updateCourtAdminLeave.LeaveType = lookupCode2.Adapt<LookupCodeDto>();
            updateCourtAdminLeave.LeaveTypeId = lookupCode2.Id;
            updateCourtAdminLeave.Id = response.Id;

            //Update
            var controllerResult3 = await _controller.UpdateCourtAdminLeave(updateCourtAdminLeave);
            var response3 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult3);

            Assert.Equal(response3.StartDate, updateCourtAdminLeave.StartDate);
            Assert.Equal(response3.EndDate, updateCourtAdminLeave.EndDate);
            Assert.Equal(response3.CourtAdminId, updateCourtAdminLeave.CourtAdminId);
            Assert.Equal(response3.LeaveTypeId, updateCourtAdminLeave.LeaveTypeId);

            //Remove
            var controllerResult4 = await _controller.RemoveCourtAdminLeave(updateCourtAdminLeave.Id, "expired");
            HttpResponseTest.CheckForNoContentResponse(controllerResult4);

            var controllerResult5 = await _controller.GetCourtAdminForTeam(courtAdminObject.Id);
            var response5 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult5);
            Assert.Empty(response5.Leave);
        }
       
        [Fact]
        public async Task AddUpdateRemoveCourtAdminTraining()
        {
            var courtAdminObject = await CreateCourtAdminUsingDbContext();

            var newLocation = new Location { Name = "New PLace", AgencyId = "zfddf2342"};
            await Db.Location.AddAsync(newLocation);

            await Db.SaveChangesAsync();

            var lookupCode = new LookupCode
            {
                Code = "zz4",
                Description = "gg",
                LocationId = newLocation.Id
            };
            await Db.LookupCode.AddAsync(lookupCode);

            var lookupCode2 = new LookupCode
            {
                Code = "zz",
                Description = "gg",
                LocationId = newLocation.Id
            };

            await Db.LookupCode.AddAsync(lookupCode2);

            await Db.SaveChangesAsync();


            var entity = new CourtAdminTrainingDto
            {
                TrainingTypeId = lookupCode.Id,
                CourtAdminId = courtAdminObject.Id,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddHours(3)
            };

            //Add
            var controllerResult = await _controller.AddCourtAdminTraining(entity);
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);

            Detach();

            var updateCourtAdminTraining = entity.Adapt<CourtAdminTrainingDto>();
            updateCourtAdminTraining.StartDate = DateTime.UtcNow.AddDays(5);
            updateCourtAdminTraining.EndDate = DateTime.UtcNow.AddDays(5);
            updateCourtAdminTraining.TrainingTypeId = lookupCode2.Id;
            updateCourtAdminTraining.TrainingType = lookupCode2.Adapt<LookupCodeDto>();
            updateCourtAdminTraining.Id = response.Id;

            //Update
            var controllerResult3 = await _controller.UpdateCourtAdminTraining(updateCourtAdminTraining);
            var response3 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult3);

            Assert.Equal(response3.StartDate, updateCourtAdminTraining.StartDate);
            Assert.Equal(response3.EndDate, updateCourtAdminTraining.EndDate);
            Assert.Equal(response3.CourtAdminId, updateCourtAdminTraining.CourtAdminId);
            Assert.Equal(response3.TrainingTypeId, updateCourtAdminTraining.TrainingTypeId);

            //Remove
            var controllerResult4 = await _controller.RemoveCourtAdminTraining(updateCourtAdminTraining.Id, "expired");
            HttpResponseTest.CheckForNoContentResponse(controllerResult4);

            var controllerResult5 = await _controller.GetCourtAdminForTeam(courtAdminObject.Id);
            var response5 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult5);
            Assert.Empty(response5.Training);
        }

        [Fact]
        public async Task CourtAdminOverrideConflictRemove()
        {
            var courtAdminObject = await CreateCourtAdminUsingDbContext();

            var newLocation = new Location { Name = "New PLace", AgencyId = "zfddf2342" };
            await Db.Location.AddAsync(newLocation);
            await Db.SaveChangesAsync();

            var startDate = DateTimeOffset.UtcNow.Date.AddHours(1);
            var endDate = startDate.AddHours(8);

            var shift = new Shift
            {
                Id = 9000665,
                StartDate = startDate,
                EndDate = endDate,
                LocationId = newLocation.Id,
                Timezone = "America/Vancouver",
                CourtAdminId = courtAdminObject.Id
            };

            Db.Shift.Add(shift);

            var duty = new Duty
            {
                LocationId = newLocation.Id,
            };

            Db.Duty.Add(duty);

            await Db.SaveChangesAsync();

            var dutySlot = new DutySlot
            {
                CourtAdminId = courtAdminObject.Id,
                LocationId = newLocation.Id,
                StartDate = startDate,
                EndDate = endDate,
                DutyId = duty.Id
            };

            Db.DutySlot.Add(dutySlot);

            var lookupCode = new LookupCode
            {
                Code = "zz4",
                Description = "gg",
                LocationId = newLocation.Id
            };
            await Db.LookupCode.AddAsync(lookupCode);

            await Db.SaveChangesAsync();

            var entity = new CourtAdminLeaveDto
            {
                LeaveTypeId = lookupCode.Id,
                CourtAdminId = courtAdminObject.Id,
                StartDate = startDate,
                EndDate = endDate
            };

            Assert.True(Db.Shift.Any(s => s.ExpiryDate == null && s.Id == shift.Id));
            Assert.True(Db.DutySlot.Any(ds => ds.ExpiryDate == null && ds.Id == dutySlot.Id));

            await _controller.AddCourtAdminLeave(entity, true);

            Assert.False(Db.Shift.Any(s => s.ExpiryDate == null && s.Id == shift.Id));
            Assert.False(Db.DutySlot.Any(ds => ds.ExpiryDate == null && ds.Id == dutySlot.Id ));
        }

        [Fact]
        public async Task CourtAdminEventTimeTest()
        {
            await CourtAdminEventTimeTestHelper("2025-01-01", "2025-01-02", "2024-12-31");
        }

        [Fact]
        public async Task CourtAdminEventTimeTestDSTStart()
        {
            await CourtAdminEventTimeTestHelper("2025-03-08", "2025-03-09", "2025-03-07");
        }

        [Fact]
        public async Task CourtAdminEventTimeTestDSTEnd()
        {
            await CourtAdminEventTimeTestHelper("2025-11-02", "2025-11-03", "2025-11-01");
        }

        private async Task CourtAdminEventTimeTestHelper(string awayLocationDate, string trainingDate, string trainingDate2)
        {
            var courtAdminObject = await CreateCourtAdminUsingDbContext();

            var newLocation = new Location { Name = "New PLace", AgencyId = "zfddf2342" };
            await Db.Location.AddAsync(newLocation);
            var edmontonTimezoneLocation = new Location { Name = "CranbrookExample", AgencyId = "zfddf52342", Timezone = "America/Edmonton" };
            await Db.Location.AddAsync(edmontonTimezoneLocation);
            await Db.SaveChangesAsync();

            var courtAdminAwayLocation = new CourtAdminAwayLocation
            {
                Timezone = "America/Vancouver",
                StartDate = DateTimeOffset.Parse($"{awayLocationDate} 00:00:00 -8"),
                EndDate = DateTimeOffset.Parse($"{awayLocationDate} 23:59:00 -8"),
                CourtAdminId = courtAdminObject.Id,
                LocationId = edmontonTimezoneLocation.Id
            };

            var result0 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await _controller.AddCourtAdminAwayLocation(courtAdminAwayLocation.Adapt<CourtAdminAwayLocationDto>()));

            var courtAdminTraining = new CourtAdminTraining
            {
                Timezone = "America/Edmonton",
                StartDate = DateTimeOffset.Parse($"{trainingDate} 00:00:00 -7"),
                EndDate = DateTimeOffset.Parse($"{trainingDate} 23:59:00 -7"),
                CourtAdminId = courtAdminObject.Id
            };

            HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await _controller.AddCourtAdminTraining(courtAdminTraining.Adapt<CourtAdminTrainingDto>()));

            var courtAdminTraining2 = new CourtAdminTraining
            {
                Timezone = "America/Edmonton",
                StartDate = DateTimeOffset.Parse($"{trainingDate2} 00:00:00 -7"),
                EndDate = DateTimeOffset.Parse($"{trainingDate2} 23:59:00 -7"),
                CourtAdminId = courtAdminObject.Id
            };

            HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(
                await _controller.AddCourtAdminTraining(courtAdminTraining2.Adapt<CourtAdminTrainingDto>()));
        }


        #region Helpers


        private async Task<CourtAdmin> CreateCourtAdminUsingDbContext()
        {
            var newCourtAdmin = new CourtAdmin
            {
                FirstName = "Ted",
                LastName = "Tums",
                BadgeNumber = "555",
                Email = "Ted@Teddy.com",
                Gender = Gender.Female,
                IdirId = new Guid(),
                IdirName = "ted@fakeidir",
                HomeLocation =  new Location { Name = "Teds Place", AgencyId = "5555555435353535353535353"},
            };

            await Db.CourtAdmin.AddAsync(newCourtAdmin);
            await Db.SaveChangesAsync();

            Detach();
            
            return newCourtAdmin;
        }


        private async Task<CourtAdminAwayLocation> CreateCourtAdminAwayLocationUsingDbContext()
        {
            var courtAdminObject = await CreateCourtAdminUsingDbContext();

            var newLocation = new Location { Name = "New PLace" };
            await Db.Location.AddAsync(newLocation);
            await Db.SaveChangesAsync();

            Detach();

            var courtAdminAwayLocation = new CourtAdminAwayLocation
            {
                LocationId = newLocation.Id,
                CourtAdminId = courtAdminObject.Id,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddHours(3)
            };

            await Db.CourtAdminAwayLocation.AddAsync(courtAdminAwayLocation);
            await Db.SaveChangesAsync();

            Detach();

            return courtAdminAwayLocation;
        }

        //Upload Photo

        #endregion Helpers
    }
}
