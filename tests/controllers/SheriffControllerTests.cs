using System;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Http;
using SS.Api.controllers.usermanagement;
using SS.Api.infrastructure.exceptions;
using SS.Api.Models.DB;
using SS.Api.models.dto;
using ss.db.models;
using SS.Db.models.sheriff;
using tests.api.helpers;
using tests.api.Helpers;
using Xunit;
using SS.Api.models.dto.generated;
using SS.Api.services.usermanagement;
using Microsoft.Extensions.Logging;
using SS.Db.models.scheduling;
using System.Linq;
using SS.Api.services.scheduling;
using SS.Api.services;

namespace tests.controllers
{
    public class SheriffControllerTests : WrapInTransactionScope
    {
        #region Variables
        private readonly SheriffController _controller;

        #endregion Variables

        public SheriffControllerTests() : base(false)
        {
            var environment = new EnvironmentBuilder("LocationServicesClient:Username", "LocationServicesClient:Password", "LocationServicesClient:Url");
            var httpContextAccessor = new HttpContextAccessor {HttpContext = HttpResponseTest.SetupHttpContext()};
            var managedTypesService = new ManageTypesService(Db);
            var trainingService = new TrainingService(managedTypesService, Db, environment.LogFactory.CreateLogger<TrainingService>());
            var sheriffService = new SheriffService(Db, environment.Configuration, httpContextAccessor);
            var shiftService = new ShiftService(Db,sheriffService, environment.Configuration);
            var dutyRosterService = new DutyRosterService(Db, environment.Configuration, shiftService, environment.LogFactory.CreateLogger<DutyRosterService>());
            _controller = new SheriffController(sheriffService, dutyRosterService, shiftService,new UserService(Db), trainingService, environment.Configuration, Db)
            {
                ControllerContext = HttpResponseTest.SetupMockControllerContext()
            };
        }

        [Fact]
        public async Task CreateSheriff()
        {
            var newSheriff = new Sheriff
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

            var sheriffDto = newSheriff.Adapt<SheriffWithIdirDto>();
            //Debug.Write(JsonConvert.SerializeObject(sheriffDto));

            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await _controller.AddSheriff(sheriffDto));
            var sheriffResponse = response.Adapt<Sheriff>();

            Assert.NotNull(await Db.Sheriff.FindAsync(sheriffResponse.Id));
        }

        [Fact]
        public async Task CreateSheriffSameIdir()
        {
            var newSheriff = new Sheriff
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

            var sheriffDto = newSheriff.Adapt<SheriffWithIdirDto>();
            //Debug.Write(JsonConvert.SerializeObject(sheriffDto));

            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await _controller.AddSheriff(sheriffDto));
            var sheriffResponse = response.Adapt<Sheriff>();

            Assert.NotNull(await Db.Sheriff.FindAsync(sheriffResponse.Id));

            newSheriff.BadgeNumber = "554";
            sheriffDto = newSheriff.Adapt<SheriffWithIdirDto>();
            //Debug.Write(JsonConvert.SerializeObject(sheriffDto));

            BusinessLayerException ble = null;
            try
            {
                response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(
                    await _controller.AddSheriff(sheriffDto));
                sheriffResponse = response.Adapt<Sheriff>();
            }
            catch (Exception e)
            {
                Assert.True(e is BusinessLayerException);
                ble = (BusinessLayerException) e;
            }

            Assert.Contains("has IDIR name", ble.Message);
        }


        [Fact]
        public async Task FindSheriff()
        {
            var sheriffObject = await CreateSheriffUsingDbContext();

            var controllerResult = await _controller.GetSheriffForTeam(sheriffObject.Id);
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);
            var sheriffResponse = response.Adapt<Sheriff>();

            //Compare Sheriff and Sheriff Object.
            Assert.Equal(sheriffObject.FirstName, sheriffResponse.FirstName);
            Assert.Equal(sheriffObject.LastName, sheriffResponse.LastName);
            Assert.Equal(sheriffObject.BadgeNumber, sheriffResponse.BadgeNumber);
            Assert.Equal(sheriffObject.Email, sheriffResponse.Email);
            Assert.Equal(sheriffObject.Gender, sheriffResponse.Gender);
        }

        [Fact]
        public async Task UpdateSheriff()
        {
            var sheriffObject = await CreateSheriffUsingDbContext();

            var newLocation = new Location { Name = "5", Id = 50005 , AgencyId = "645646464646464"};
            await Db.Location.AddAsync(newLocation);
            var newLocation2 = new Location { Name = "6", Id = 50006, AgencyId = "6456456464" };
            await Db.Location.AddAsync(newLocation2);
            await Db.SaveChangesAsync();

            Detach();

            var updateSheriff = sheriffObject.Adapt<SheriffWithIdirDto>();
            updateSheriff.FirstName = "Al";
            updateSheriff.LastName = "Hoyne";
            updateSheriff.BadgeNumber = "55";
            updateSheriff.Email = "hey@hey.com";
            updateSheriff.Gender = Gender.Other;

            //This object is only used for fetching.
            //updateSheriff.HomeLocation = new LocationDto { Name = "Als place2", Id = 5};
            updateSheriff.HomeLocationId = newLocation.Id;

            Detach();

            var controllerResult = await _controller.UpdateSheriff(updateSheriff);
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);
            var sheriffResponse = response.Adapt<Sheriff>();

            Assert.Equal(updateSheriff.FirstName, sheriffResponse.FirstName);
            Assert.Equal(updateSheriff.LastName, sheriffResponse.LastName);
            Assert.Equal(updateSheriff.BadgeNumber, sheriffResponse.BadgeNumber);
            Assert.Equal(updateSheriff.Email, sheriffResponse.Email);
            Assert.Equal(updateSheriff.Gender, sheriffResponse.Gender);

            //Shouldn't be able to update any of the navigation properties here.
            Assert.Empty(sheriffResponse.AwayLocation);
            Assert.Empty(sheriffResponse.Training);
            Assert.Empty(sheriffResponse.Leave);
            //Set to invalid location, should be null.
            //WE didn't set the HomeLocationId here. 
            Assert.NotNull(sheriffResponse.HomeLocation);

            Detach();

            updateSheriff.HomeLocationId = newLocation2.Id;
            updateSheriff.HomeLocation = newLocation2.Adapt<LocationDto>();
            controllerResult = await _controller.UpdateSheriff(updateSheriff);
            response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);

            Detach();

            var controllerResult2 = await _controller.GetSheriffForTeam(sheriffResponse.Id);
            var response2 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);
            Assert.NotNull(response2.HomeLocation);
            Assert.Equal(newLocation2.Id, response.HomeLocation.Id);
        }



        [Fact]
        public async Task AddUpdateRemoveSheriffAwayLocation()
        {
            //Test permissions?
            var sheriffObject = await CreateSheriffUsingDbContext();

            var newLocation = new Location { Name = "New PLace", AgencyId = "545325345353"};
            await Db.Location.AddAsync(newLocation);
            var newLocation2 = new Location { Name = "New PLace", AgencyId = "g435346346363"};
            await Db.Location.AddAsync(newLocation2);
            await Db.SaveChangesAsync();

            var sheriffAwayLocation = new SheriffAwayLocationDto
            {
                SheriffId =  sheriffObject.Id,
                LocationId = newLocation.Id,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddHours(3)
            };

            //Add
            var controllerResult = await _controller.AddSheriffAwayLocation(sheriffAwayLocation);
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);

            Assert.Equal(sheriffAwayLocation.LocationId, response.Location.Id);
            Assert.Equal(sheriffAwayLocation.SheriffId, response.SheriffId);
            Assert.Equal(sheriffAwayLocation.StartDate, response.StartDate);
            Assert.Equal(sheriffAwayLocation.EndDate, response.EndDate);

            Detach();

            var controllerResult2 = await _controller.GetSheriffForTeam(sheriffObject.Id);
            var response2 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult2);

            Assert.True(response2.AwayLocation.Count == 1);

            var updateSheriffAwayLocation = sheriffAwayLocation.Adapt<SheriffAwayLocationDto>();
            updateSheriffAwayLocation.StartDate = DateTime.UtcNow.AddDays(5);
            updateSheriffAwayLocation.EndDate = DateTime.UtcNow.AddDays(5);
            updateSheriffAwayLocation.LocationId = newLocation2.Id;
            updateSheriffAwayLocation.Id = response.Id;

            Detach();

            //Update
            var controllerResult3 = await _controller.UpdateSheriffAwayLocation(updateSheriffAwayLocation);
            var response3 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult3);

            Assert.Equal(response3.StartDate, updateSheriffAwayLocation.StartDate);
            Assert.Equal(response3.EndDate, updateSheriffAwayLocation.EndDate);
            Assert.Equal(response3.SheriffId, updateSheriffAwayLocation.SheriffId);

            Detach();

            //Remove
            var controllerResult4 = await _controller.RemoveSheriffAwayLocation(response.Id, "hello");
            HttpResponseTest.CheckForNoContentResponse(controllerResult4);

            var controllerResult6 = await _controller.GetSheriffForTeam(sheriffObject.Id);
            var response6 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult6);
            Assert.Empty(response6.AwayLocation);
        }

        [Fact]
        public async Task AddUpdateRemoveSheriffLeave()
        {
            var sheriffObject = await CreateSheriffUsingDbContext();
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


            var entity = new SheriffLeaveDto
            {
                LeaveTypeId = lookupCode.Id,
                SheriffId = sheriffObject.Id,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddHours(3)
            };

            //Add
            var controllerResult = await _controller.AddSheriffLeave(entity);
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);

            Assert.Equal(entity.LeaveTypeId, response.LeaveType.Id);
            Assert.Equal(entity.SheriffId, response.SheriffId);
            Assert.Equal(entity.StartDate, response.StartDate);
            Assert.Equal(entity.EndDate, response.EndDate);

            var controllerResult2 = await _controller.GetSheriffForTeam(sheriffObject.Id);
            var response2 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult2);

            Assert.True(response2.Leave.Count == 1);

            Detach();

            var updateSheriffLeave = entity.Adapt<SheriffLeaveDto>();
            updateSheriffLeave.StartDate = DateTime.UtcNow.AddDays(5);
            updateSheriffLeave.EndDate = DateTime.UtcNow.AddDays(5);
            //updateSheriffLeave.LeaveTypeId = lookupCode2.Id;
           
            updateSheriffLeave.LeaveType = lookupCode2.Adapt<LookupCodeDto>();
            updateSheriffLeave.LeaveTypeId = lookupCode2.Id;
            updateSheriffLeave.Id = response.Id;

            //Update
            var controllerResult3 = await _controller.UpdateSheriffLeave(updateSheriffLeave);
            var response3 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult3);

            Assert.Equal(response3.StartDate, updateSheriffLeave.StartDate);
            Assert.Equal(response3.EndDate, updateSheriffLeave.EndDate);
            Assert.Equal(response3.SheriffId, updateSheriffLeave.SheriffId);
            Assert.Equal(response3.LeaveTypeId, updateSheriffLeave.LeaveTypeId);

            //Remove
            var controllerResult4 = await _controller.RemoveSheriffLeave(updateSheriffLeave.Id, "expired");
            HttpResponseTest.CheckForNoContentResponse(controllerResult4);

            var controllerResult5 = await _controller.GetSheriffForTeam(sheriffObject.Id);
            var response5 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult5);
            Assert.Empty(response5.Leave);
        }
       
        [Fact]
        public async Task AddUpdateRemoveSheriffTraining()
        {
            var sheriffObject = await CreateSheriffUsingDbContext();

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


            var entity = new SheriffTrainingDto
            {
                TrainingTypeId = lookupCode.Id,
                SheriffId = sheriffObject.Id,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddHours(3)
            };

            //Add
            var controllerResult = await _controller.AddSheriffTraining(entity);
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);

            Detach();

            var updateSheriffTraining = entity.Adapt<SheriffTrainingDto>();
            updateSheriffTraining.StartDate = DateTime.UtcNow.AddDays(5);
            updateSheriffTraining.EndDate = DateTime.UtcNow.AddDays(5);
            updateSheriffTraining.TrainingTypeId = lookupCode2.Id;
            updateSheriffTraining.TrainingType = lookupCode2.Adapt<LookupCodeDto>();
            updateSheriffTraining.Id = response.Id;

            //Update
            var controllerResult3 = await _controller.UpdateSheriffTraining(updateSheriffTraining);
            var response3 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult3);

            Assert.Equal(response3.StartDate, updateSheriffTraining.StartDate);
            Assert.Equal(response3.EndDate, updateSheriffTraining.EndDate);
            Assert.Equal(response3.SheriffId, updateSheriffTraining.SheriffId);
            Assert.Equal(response3.TrainingTypeId, updateSheriffTraining.TrainingTypeId);

            //Remove
            var controllerResult4 = await _controller.RemoveSheriffTraining(updateSheriffTraining.Id, "expired");
            HttpResponseTest.CheckForNoContentResponse(controllerResult4);

            var controllerResult5 = await _controller.GetSheriffForTeam(sheriffObject.Id);
            var response5 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult5);
            Assert.Empty(response5.Training);
        }

        [Fact]
        public async Task SheriffOverrideConflictRemove()
        {
            var sheriffObject = await CreateSheriffUsingDbContext();

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
                SheriffId = sheriffObject.Id
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
                SheriffId = sheriffObject.Id,
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

            var entity = new SheriffLeaveDto
            {
                LeaveTypeId = lookupCode.Id,
                SheriffId = sheriffObject.Id,
                StartDate = startDate,
                EndDate = endDate
            };

            Assert.True(Db.Shift.Any(s => s.ExpiryDate == null && s.Id == shift.Id));
            Assert.True(Db.DutySlot.Any(ds => ds.ExpiryDate == null && ds.Id == dutySlot.Id));

            await _controller.AddSheriffLeave(entity, true);

            Assert.False(Db.Shift.Any(s => s.ExpiryDate == null && s.Id == shift.Id));
            Assert.False(Db.DutySlot.Any(ds => ds.ExpiryDate == null && ds.Id == dutySlot.Id ));
        }

        [Fact]
        public async Task SheriffEventTimeTest()
        {
            await SheriffEventTimeTestHelper("2025-01-01", "2025-01-02", "2024-12-31");
        }

        [Fact]
        public async Task SheriffEventTimeTestDSTStart()
        {
            await SheriffEventTimeTestHelper("2025-03-08", "2025-03-09", "2025-03-07");
        }

        [Fact]
        public async Task SheriffEventTimeTestDSTEnd()
        {
            await SheriffEventTimeTestHelper("2025-11-02", "2025-11-03", "2025-11-01");
        }

        private async Task SheriffEventTimeTestHelper(string awayLocationDate, string trainingDate, string trainingDate2)
        {
            var sheriffObject = await CreateSheriffUsingDbContext();

            var newLocation = new Location { Name = "New PLace", AgencyId = "zfddf2342" };
            await Db.Location.AddAsync(newLocation);
            var edmontonTimezoneLocation = new Location { Name = "CranbrookExample", AgencyId = "zfddf52342", Timezone = "America/Edmonton" };
            await Db.Location.AddAsync(edmontonTimezoneLocation);
            await Db.SaveChangesAsync();

            var sheriffAwayLocation = new SheriffAwayLocation
            {
                Timezone = "America/Vancouver",
                StartDate = DateTimeOffset.Parse($"{awayLocationDate} 00:00:00 -8"),
                EndDate = DateTimeOffset.Parse($"{awayLocationDate} 23:59:00 -8"),
                SheriffId = sheriffObject.Id,
                LocationId = edmontonTimezoneLocation.Id
            };

            var result0 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await _controller.AddSheriffAwayLocation(sheriffAwayLocation.Adapt<SheriffAwayLocationDto>()));

            var sheriffTraining = new SheriffTraining
            {
                Timezone = "America/Edmonton",
                StartDate = DateTimeOffset.Parse($"{trainingDate} 00:00:00 -7"),
                EndDate = DateTimeOffset.Parse($"{trainingDate} 23:59:00 -7"),
                SheriffId = sheriffObject.Id
            };

            HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await _controller.AddSheriffTraining(sheriffTraining.Adapt<SheriffTrainingDto>()));

            var sheriffTraining2 = new SheriffTraining
            {
                Timezone = "America/Edmonton",
                StartDate = DateTimeOffset.Parse($"{trainingDate2} 00:00:00 -7"),
                EndDate = DateTimeOffset.Parse($"{trainingDate2} 23:59:00 -7"),
                SheriffId = sheriffObject.Id
            };

            HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(
                await _controller.AddSheriffTraining(sheriffTraining2.Adapt<SheriffTrainingDto>()));
        }


        #region Helpers


        private async Task<Sheriff> CreateSheriffUsingDbContext()
        {
            var newSheriff = new Sheriff
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

            await Db.Sheriff.AddAsync(newSheriff);
            await Db.SaveChangesAsync();

            Detach();
            
            return newSheriff;
        }


        private async Task<SheriffAwayLocation> CreateSheriffAwayLocationUsingDbContext()
        {
            var sheriffObject = await CreateSheriffUsingDbContext();

            var newLocation = new Location { Name = "New PLace" };
            await Db.Location.AddAsync(newLocation);
            await Db.SaveChangesAsync();

            Detach();

            var sheriffAwayLocation = new SheriffAwayLocation
            {
                LocationId = newLocation.Id,
                SheriffId = sheriffObject.Id,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddHours(3)
            };

            await Db.SheriffAwayLocation.AddAsync(sheriffAwayLocation);
            await Db.SaveChangesAsync();

            Detach();

            return sheriffAwayLocation;
        }

        //Upload Photo

        #endregion Helpers
    }
}
