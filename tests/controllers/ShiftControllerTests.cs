using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using CAS.API.controllers.scheduling;
using CAS.API.infrastructure.exceptions;
using CAS.API.Models.DB;
using CAS.API.models.dto.generated;
using CAS.API.services.scheduling;
using CAS.API.services.usermanagement;
using CAS.COMMON.helpers.extensions;
using CAS.DB.models;
using CAS.DB.models.scheduling;
using CAS.DB.models.scheduling.notmapped;
using Microsoft.Extensions.Logging;
using CAS.DB.models.courtAdmin;
using tests.api.helpers;
using tests.api.Helpers;
using Xunit;

namespace tests.controllers
{
    //Sequential, because there are issues with Adding Location (with a unique index) within a TransactionScope.
    [Collection("Sequential")]
    public class ShiftControllerTests : WrapInTransactionScope
    {
        private ShiftController ShiftController { get; }
        public ShiftControllerTests() : base(false)
        {
            var environment = new EnvironmentBuilder("LocationServicesClient:Username", "LocationServicesClient:Password", "LocationServicesClient:Url");
            var shiftService = new ShiftService(Db, new CourtAdminService(Db, environment.Configuration),
                environment.Configuration);
            var dutyRosterService = new DutyRosterService(Db, environment.Configuration,
                shiftService, environment.LogFactory.CreateLogger<DutyRosterService>());
            ShiftController = new ShiftController(shiftService, dutyRosterService, Db, environment.Configuration)
            {
                ControllerContext = HttpResponseTest.SetupMockControllerContext()
            };
        }

        [Fact]
        public async Task AddShiftConflicts()
        {
            var sheriffId = Guid.NewGuid();
            var location = new Location {Id = 50000, AgencyId = "zz"};
            await Db.Location.AddAsync(location);
            await Db.CourtAdmin.AddAsync(new CourtAdmin { Id = sheriffId, IsEnabled = true, HomeLocationId = location.Id});
            await Db.SaveChangesAsync();

            Detach();

            //Case where shifts conflict with themselves.
            var shiftOne = new Shift
            {
                StartDate = DateTimeOffset.UtcNow.Date,
                EndDate = DateTimeOffset.UtcNow.Date.AddHours(1),
                LocationId = location.Id,
                Timezone = "America/Vancouver",
                SheriffId = sheriffId
            }.Adapt<AddShiftDto>();

            var shiftTwo = new Shift
            {
                StartDate = DateTimeOffset.UtcNow.Date.AddHours(1),
                EndDate = DateTimeOffset.UtcNow.Date.AddHours(2),
                LocationId = location.Id,
                Timezone = "America/Vancouver",
                SheriffId = sheriffId
            }.Adapt<AddShiftDto>();

            var shiftThree = new Shift
            {
                Id = 3,
                StartDate = DateTimeOffset.UtcNow.Date,
                EndDate = DateTimeOffset.UtcNow.Date.AddHours(1),
                LocationId = location.Id,
                Timezone = "America/Vancouver",
                SheriffId = sheriffId
            }.Adapt<AddShiftDto>();

            var shiftFour = new Shift
            {
                Id = 4,
                StartDate = DateTimeOffset.UtcNow.Date.AddHours(1),
                EndDate = DateTimeOffset.UtcNow.Date.AddHours(2),
                LocationId = location.Id,
                Timezone = "America/Vancouver",
                SheriffId = sheriffId
            }.Adapt<AddShiftDto>();

            var shiftFive = new Shift
            {
                StartDate = DateTimeOffset.UtcNow.Date,
                EndDate = DateTimeOffset.UtcNow.Date.AddHours(2),
                LocationId = location.Id,
                Timezone = "America/Vancouver",
                SheriffId = sheriffId
            }.Adapt<AddShiftDto>();

            var shiftSix = new Shift
            {
                Id = 6,
                StartDate = DateTimeOffset.UtcNow.Date.AddHours(2),
                EndDate = DateTimeOffset.UtcNow.Date.AddHours(3),
                LocationId = location.Id,
                Timezone = "America/Vancouver",
                SheriffId = sheriffId
            }.Adapt<AddShiftDto>();

            var shiftSeven = new Shift
            {
                Id = 7,
                StartDate = DateTimeOffset.UtcNow.Date.AddHours(-1),
                EndDate = DateTimeOffset.UtcNow.Date,
                LocationId = location.Id,
                Timezone = "America/Vancouver",
                SheriffId = sheriffId
            }.Adapt<AddShiftDto>();


            await Assert.ThrowsAsync<BusinessLayerException>(async () => await ShiftController.AddShifts( new List<AddShiftDto> { shiftOne, shiftFive } ));
            var sheriffShifts = Db.Shift.AsNoTracking().Where(s => s.SheriffId == sheriffId);
            Assert.Empty(sheriffShifts);

            //Two shifts no conflicts.
            var shifts = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.AddShifts(new List<AddShiftDto> { shiftOne, shiftTwo }));
            sheriffShifts = Db.Shift.AsNoTracking().Where(s => s.SheriffId == sheriffId);
            Assert.All(sheriffShifts, s => new List<int> { 1, 2 }.Contains(s.Id));

            //Already assigned to two shifts: Two new shifts, should conflict now. 
            await Assert.ThrowsAsync<BusinessLayerException>(async () => await ShiftController.AddShifts(new List<AddShiftDto> { shiftThree, shiftFour }));
            sheriffShifts = Db.Shift.AsNoTracking().Where(s => s.SheriffId == sheriffId);
            Assert.All(sheriffShifts, s => new List<int> { 1, 2 }.Contains(s.Id));

            //Schedule two more shifts, on the outside of 3 and 4. 
            HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.AddShifts(new List<AddShiftDto> { shiftSix, shiftSeven }));
            sheriffShifts = Db.Shift.AsNoTracking().Where(s => s.SheriffId == sheriffId);
            Assert.All(sheriffShifts, s => new List<int> { 3, 4, 6, 7 }.Contains(s.Id));
        }


        [Fact]
        public async Task GetAvailability()
        {
            var location1 = new Location {Id = 50000, AgencyId = "zz", Name = "Location 1"};
            var location2 = new Location {Id = 50002, AgencyId = "5555", Name = "Location 2"};
            await Db.Location.AddAsync(location1);
            await Db.Location.AddAsync(location2);
            await Db.SaveChangesAsync();

            var startDate = DateTimeOffset.UtcNow.ConvertToTimezone("America/Edmonton");
            var endDate = DateTimeOffset.UtcNow.TranslateDateForDaylightSavings("America/Edmonton", 7);

            //On awayLocation.
            var awayLocationSheriff = Guid.NewGuid();
            await Db.CourtAdmin.AddAsync(new CourtAdmin
            {
                HomeLocationId = location1.Id,
                Id = awayLocationSheriff,
                IsEnabled = true,
                AwayLocation = new List<CourtAdminAwayLocation>
                {
                    new CourtAdminAwayLocation
                    {
                        StartDate = startDate,
                        EndDate = startDate.AddDays(1),
                        LocationId = location2.Id
                    }
                }
            });

            //On training.
            var trainingSheriff = Guid.NewGuid();
            await Db.CourtAdmin.AddAsync(new CourtAdmin
            {
                HomeLocationId = location1.Id,
                Id = trainingSheriff,
                IsEnabled = true,
                Training = new List<CourtAdminTraining>
                {
                    new CourtAdminTraining
                    {
                        StartDate = startDate.AddDays(1),
                        EndDate = startDate.AddDays(2)
                    }
                }
            });

            //On leave.
            var leaveSheriff = Guid.NewGuid();
            await Db.CourtAdmin.AddAsync(new CourtAdmin
            {
                HomeLocationId = location1.Id,
                Id = leaveSheriff,
                IsEnabled = true,
                Leave = new List<CourtAdminLeave>
                {
                    new CourtAdminLeave
                    {
                        StartDate = startDate.AddDays(1),
                        EndDate = startDate.AddDays(2)
                    }
                }
            });

            //Already scheduled.
            var scheduledSheriff = Guid.NewGuid();
            await Db.CourtAdmin.AddAsync(new CourtAdmin
            {
                HomeLocationId = location1.Id,
                Id = scheduledSheriff,
                IsEnabled = true
            });

            await Db.Shift.AddAsync(new Shift
            {
                Id = 1,
                StartDate = startDate.AddDays(2),
                EndDate = startDate.AddDays(3),
                LocationId = location1.Id,
                SheriffId = scheduledSheriff,
                Timezone = "America/Vancouver"
            });


            //Already scheduled different location.
            var scheduledDifferentLocationSheriff = Guid.NewGuid();
            await Db.CourtAdmin.AddAsync(new CourtAdmin
            {
                HomeLocationId = location1.Id,
                Id = scheduledDifferentLocationSheriff,
                IsEnabled = true
            });

            await Db.Shift.AddAsync(new Shift
            {
                Id = 2,
                StartDate = startDate.AddDays(2),
                EndDate = startDate.AddDays(3),
                LocationId = location2.Id,
                SheriffId = scheduledDifferentLocationSheriff,
                Timezone = "America/Vancouver"
            });

            //Expired Leave, Expired Training, Expired Away Location, Expired Shift.
            var expiredEventsAndShiftSheriff = Guid.NewGuid();
            await Db.CourtAdmin.AddAsync(new CourtAdmin
            {
                HomeLocationId = location1.Id,
                Id = expiredEventsAndShiftSheriff,
                IsEnabled = true,
                Leave = new List<CourtAdminLeave>
                {
                    new CourtAdminLeave
                    {
                        StartDate = startDate.AddDays(1),
                        EndDate = startDate.AddDays(2),
                        ExpiryDate = DateTimeOffset.UtcNow
                    }
                },
                Training = new List<CourtAdminTraining>
                {
                    new CourtAdminTraining
                    {
                        StartDate = startDate.AddDays(1),
                        EndDate = startDate.AddDays(2),
                        ExpiryDate = DateTimeOffset.UtcNow
                    }
                },
                AwayLocation = new List<CourtAdminAwayLocation>
                {
                    new CourtAdminAwayLocation
                    {
                        StartDate = startDate,
                        EndDate = startDate.AddDays(1),
                        LocationId = location2.Id,
                        ExpiryDate = DateTimeOffset.UtcNow
                    }
                }
            });

            await Db.Shift.AddAsync(new Shift
            {
                Id = 3,
                StartDate = startDate.AddDays(2),
                EndDate = startDate.AddDays(3),
                LocationId = location2.Id,
                SheriffId = expiredEventsAndShiftSheriff,
                Timezone = "America/Vancouver",
                ExpiryDate = DateTimeOffset.UtcNow
            });

            //Expired Sheriff.
            var expiredSheriff = Guid.NewGuid();
            await Db.CourtAdmin.AddAsync(new CourtAdmin
            {
                HomeLocationId = location1.Id,
                Id = expiredSheriff,
                FirstName = "Expired",
                LastName = "Expired Sheriff",
                IsEnabled = false
            });

            await Db.SaveChangesAsync();

            //Loaned in.
            var loanedInSheriff = Guid.NewGuid();
            await Db.CourtAdmin.AddAsync(new CourtAdmin
            {
                HomeLocationId = location2.Id,
                FirstName = "Loaned In",
                LastName = "Loaned In",
                Id = loanedInSheriff,
                IsEnabled = true,
                AwayLocation = new List<CourtAdminAwayLocation>
                {
                    new CourtAdminAwayLocation
                    {
                        StartDate = startDate,
                        EndDate = startDate.AddDays(1),
                        LocationId = location1.Id
                    }
                }
            });

            await Db.SaveChangesAsync();

            var shiftConflicts = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(
                await ShiftController.GetAvailability(location1.Id, startDate, endDate));

            //Postgres stores ticks as 1/1,000,000 vs .NET uses 1/10,000,000
            var awayLocationsSheriffConflicts = shiftConflicts.FirstOrDefault(sc => sc.SheriffId == awayLocationSheriff);
            Assert.NotNull(awayLocationsSheriffConflicts); 
            Assert.Contains(awayLocationsSheriffConflicts.Conflicts, c =>
                c.Conflict == ShiftConflictType.AwayLocation && c.LocationId == location2.Id && (startDate - c.Start).TotalSeconds <= 1 && (startDate.AddDays(1) - c.End).TotalSeconds <= 1);

            var trainingSheriffConflicts = shiftConflicts.FirstOrDefault(sc => sc.SheriffId == trainingSheriff);
            Assert.NotNull(trainingSheriffConflicts);
            Assert.Contains(trainingSheriffConflicts.Conflicts, c =>
                c.Conflict == ShiftConflictType.Training);

            var leaveSheriffConflicts = shiftConflicts.FirstOrDefault(sc => sc.SheriffId == leaveSheriff);
            Assert.NotNull(leaveSheriffConflicts);
            Assert.Contains(leaveSheriffConflicts.Conflicts, c =>
                c.Conflict == ShiftConflictType.Leave);

            var scheduledSheriffConflicts = shiftConflicts.FirstOrDefault(sc => sc.SheriffId == scheduledSheriff);
            Assert.NotNull(scheduledSheriffConflicts);
            Assert.Contains(scheduledSheriffConflicts.Conflicts, c =>
                c.Conflict == ShiftConflictType.Scheduled && (startDate.AddDays(2) - c.Start).TotalSeconds <= 1 && (startDate.AddDays(3) - c.End).TotalSeconds <= 1  && c.LocationId == location1.Id);

            var scheduledDifferentLocationSheriffConflicts = shiftConflicts.FirstOrDefault(sc => sc.SheriffId == scheduledDifferentLocationSheriff);
            Assert.NotNull(scheduledDifferentLocationSheriffConflicts);
            Assert.Contains(scheduledDifferentLocationSheriffConflicts.Conflicts, c =>
                c.Conflict == ShiftConflictType.Scheduled && (startDate.AddDays(2) - c.Start).TotalSeconds <= 1 && (startDate.AddDays(3) - c.End).TotalSeconds <= 1 && c.LocationId == location2.Id);

            var expiredEventsAndShiftSheriffConflict = shiftConflicts.FirstOrDefault(sc => sc.SheriffId == expiredEventsAndShiftSheriff);
            Assert.NotNull(expiredEventsAndShiftSheriffConflict);
            Assert.Empty(expiredEventsAndShiftSheriffConflict.Conflicts);

            Assert.True(shiftConflicts.All(sc => sc.SheriffId != expiredSheriff));

            var loanedInSheriffConflicts = shiftConflicts.FirstOrDefault(sc => sc.SheriffId == loanedInSheriff);
            Assert.NotNull(loanedInSheriffConflicts);
            Assert.Contains(loanedInSheriffConflicts.Conflicts, c =>
                c.Conflict == ShiftConflictType.AwayLocation && c.LocationId == location1.Id && (startDate - c.Start).TotalSeconds <= 1 && (startDate.AddDays(1) - c.End).TotalSeconds <= 1);

        }

        [Fact]
        public async Task GetShifts()
        {
            var edmontonTz = DateTimeZoneProviders.Tzdb["America/Edmonton"];
            var startDateNowEdmonton = SystemClock.Instance.GetCurrentInstant().InZone(edmontonTz);
            var endDateNowEdmonton = SystemClock.Instance.GetCurrentInstant().InZone(edmontonTz).PlusHours(24*5);

            var startTimeOffset = startDateNowEdmonton.ToDateTimeOffset();
            var endTimeOffset = endDateNowEdmonton.ToDateTimeOffset();

            await Db.Shift.AddAsync(new Shift
            {
                Id = 1, 
                StartDate = startTimeOffset,
                EndDate = endTimeOffset,
                Sheriff = new CourtAdmin { Id = Guid.NewGuid(), LastName = "hello" },
                AnticipatedAssignment = new Assignment {Id = 1, Name = "Super assignment", Location = new Location { Id = 50000, AgencyId = "zz"}, LookupCode = new LookupCode() {Id = 900000}},
                LocationId = 50000
            });

            await Db.SaveChangesAsync();

            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.GetShifts(50000, startTimeOffset, endTimeOffset));
            Assert.NotEmpty(response);
            Assert.NotNull(response[0].Sheriff);
            Assert.NotNull(response[0].AnticipatedAssignment);
            Assert.NotNull(response[0].Location);
        }

        [Fact]
        public async Task AddShift()
        {
            var shiftDto = await CreateShift();
            var shiftDtos = new List<AddShiftDto> { shiftDto.Adapt<AddShiftDto>() };
            var shift = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.AddShifts(shiftDtos));

            var locationId = shiftDto.LocationId;

            var stDate = DateTimeOffset.UtcNow.AddDays(20);

            var sheriffId = Guid.NewGuid();
            var sheriffIdAway = Guid.NewGuid();
            await Db.CourtAdmin.AddAsync(new CourtAdmin
            {
                Id = sheriffId,
                FirstName = "Hello",
                LastName = "There",
                IsEnabled = true,
                HomeLocationId = locationId
            });
            await Db.CourtAdmin.AddAsync(new CourtAdmin
            {
                Id = sheriffIdAway,
                FirstName = "Hello2",
                LastName = "There2",
                IsEnabled = true,
                HomeLocationId = locationId,
                AwayLocation = new List<CourtAdminAwayLocation>
                {
                    new CourtAdminAwayLocation
                    {
                        Id = 1,
                        LocationId = locationId,
                        StartDate = stDate,
                        EndDate = stDate.AddDays(5)
                    }
                }
            });
            await Db.Assignment.AddAsync(new Assignment
            {
                Id = 5,
                LocationId = locationId,
                LookupCode = new LookupCode()
                {
                    Id = 9000
                }
            });
            await Db.SaveChangesAsync();

            //Add Shift, this also tests UpdateShift's validation. 
            //Tests a conflict. 

            var addShifts = new List<AddShiftDto>
            {
                new AddShiftDto
                {
                    SheriffId = sheriffId,
                    StartDate = stDate,
                    EndDate = stDate.AddHours(5),
                    LocationId = locationId,
                    Timezone = "America/Edmonton"
                }
            };

            HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.AddShifts(addShifts));

            addShifts = new List<AddShiftDto>
            {
                new AddShiftDto
                {
                    SheriffId = sheriffId,
                    StartDate = stDate.AddHours(2),
                    EndDate = stDate.AddHours(3),
                    LocationId = locationId,
                    Timezone = "America/Edmonton"
                }
            };

            await Assert.ThrowsAsync<BusinessLayerException>(() => ShiftController.AddShifts(addShifts));

            //Schedule a sheriff who has an AwayLocation row, with the same ID as the location scheduled for. 

            addShifts = new List<AddShiftDto>
            {
                new AddShiftDto
                {
                    SheriffId = sheriffIdAway,
                    StartDate = stDate.AddHours(2),
                    EndDate = stDate.AddHours(3),
                    LocationId = locationId,
                    Timezone = "America/Edmonton"
                }
            };

            HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.AddShifts(addShifts));
        }


        [Fact]
        public async Task AddShiftSheriffEventConflict()
        {
            var location2 = new Location {Id = 50002, AgencyId = "5555", Name = "Location 2"};
            await Db.Location.AddAsync(location2);
            var shiftDto = await CreateShift();

            var locationId1 = shiftDto.LocationId;
            var locationId2 = location2.Id;

            var startDate = shiftDto.StartDate;
            var sheriffId = Guid.NewGuid();
            shiftDto.SheriffId = sheriffId;

            var shiftDtos = new List<ShiftDto> { shiftDto }.Adapt<List<AddShiftDto>>();

            await Db.CourtAdmin.AddAsync(new CourtAdmin
            {
                Id = sheriffId, HomeLocationId = locationId1, FirstName = "First", LastName = "Sheriff", IsEnabled = true,
                Leave = new List<CourtAdminLeave>
                {
                    new CourtAdminLeave
                    {
                        StartDate = startDate,
                        EndDate = startDate.AddDays(2)
                    }
                },
                Training = new List<CourtAdminTraining>
                {
                    new CourtAdminTraining
                    {
                        StartDate = startDate,
                        EndDate = startDate.AddDays(2)
                    }
                },
                AwayLocation = new List<CourtAdminAwayLocation>
                {
                    new CourtAdminAwayLocation
                    {
                        StartDate = startDate,
                        EndDate = startDate.AddDays(1),
                        LocationId = locationId2
                    }
                }
            });
            await Db.SaveChangesAsync();

            //Three conflicts should come back. 
            await Assert.ThrowsAsync<BusinessLayerException>(async () => await ShiftController.AddShifts(shiftDtos));
        }


        [Fact]
        public async Task UpdateShift()
        {
            var shiftDto = await CreateShift();
            var shiftDtos = new List<ShiftDto> {shiftDto};
            var sheriffId = Guid.NewGuid();
            var locationId1 = shiftDto.LocationId;

            await Db.CourtAdmin.AddAsync(new CourtAdmin { Id = sheriffId, FirstName = "Hello", LastName = "There", IsEnabled = true, HomeLocationId = locationId1 });
            await Db.Assignment.AddAsync(new Assignment { Id = 5, LocationId = locationId1, LookupCode = new LookupCode() { Id = 9000 }});
            await Db.SaveChangesAsync();

            var shifts = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.AddShifts(shiftDtos.Adapt<List<AddShiftDto>>()));
            var shift = shifts.First();
            shift.StartDate = DateTimeOffset.UtcNow.AddDays(5).Date;
            shift.EndDate = DateTimeOffset.UtcNow.AddDays(6).Date;
            shift.LocationId = locationId1; // This shouldn't change 
            shift.ExpiryDate = DateTimeOffset.UtcNow; // this shouldn't change
            shift.SheriffId = sheriffId;
            shift.Sheriff = new SheriffDto(); // shouldn't change
            shift.AnticipatedAssignment = new AssignmentDto(); //this shouldn't create new. 
            shift.AnticipatedAssignmentId = 5;
            shift.Location = new LocationDto(); // shouldn't change

            var updatedShifts = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.UpdateShifts(shifts.Adapt<List<UpdateShiftDto>>()));
            var updatedShift = updatedShifts.First();

            Assert.Equal(shiftDto.LocationId,updatedShift.LocationId);
            Assert.Null(updatedShift.ExpiryDate);
            Assert.Equal(5, updatedShift.AnticipatedAssignmentId);
            Assert.Equal(sheriffId, updatedShift.SheriffId);

            //Create the same shift, without sheriff, should conflict.
            shiftDto.SheriffId = sheriffId;
            shiftDto.StartDate = DateTimeOffset.UtcNow.AddDays(5).Date;
            shiftDto.EndDate = DateTimeOffset.UtcNow.AddDays(6).Date;
            shifts = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.AddShifts(shiftDtos.Adapt<List<AddShiftDto>>()));
            shift = shifts.First();

            shift.SheriffId = sheriffId;
            await Assert.ThrowsAsync<BusinessLayerException>(() => ShiftController.UpdateShifts(shifts.Adapt<List<UpdateShiftDto>>()));

            //Create a shift that sits side by side, without sheriff, shouldn't conflict.
            shiftDto.SheriffId = sheriffId;
            shiftDto.StartDate = DateTimeOffset.UtcNow.AddDays(4).Date;
            shiftDto.EndDate = DateTimeOffset.UtcNow.AddDays(5).Date;
            shifts = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.AddShifts(shiftDtos.Adapt<List<AddShiftDto>>()));
            shift = shifts.First();

            shift.SheriffId = sheriffId;
            updatedShifts = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.UpdateShifts(shifts.Adapt<List<UpdateShiftDto>>()));

            var firstShift = updatedShifts.OrderBy(s => s.StartDate).First();
            var lastShift = updatedShifts.OrderByDescending(s => s.EndDate).First();

            Assert.Equal(shiftDto.StartDate, firstShift.StartDate);
            Assert.Equal(shiftDto.EndDate, lastShift.EndDate);
        }



        [Fact]
        public async Task RemoveShift()
        {
            var shiftDto = await CreateShift();
            var shiftDtos = new List<ShiftDto> {shiftDto}.Adapt<List<AddShiftDto>>();
            var shifts = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.AddShifts(shiftDtos));
            var shift = shifts.First();

            HttpResponseTest.CheckForNoContentResponse(await ShiftController.ExpireShifts(new List<int> {shift.Id}));

            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.GetShifts(1, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddHours(5)));
            Assert.Empty(response);
        }

        [Fact]
        public async Task ImportWeeklyShifts()
        {
            var sheriffId = Guid.NewGuid();
            var sheriffId2 = Guid.NewGuid();
            var shiftDto = await CreateShift();
            await Db.CourtAdmin.AddAsync(new CourtAdmin {Id = sheriffId, IsEnabled = true, HomeLocationId = 50000 });
            await Db.Location.AddAsync(new Location { Id = 50002, AgencyId = "3z", Timezone = "America/Vancouver" });
            await Db.CourtAdmin.AddAsync(new CourtAdmin { Id = sheriffId2, IsEnabled = true, HomeLocationId = 50002 });
            await Db.SaveChangesAsync();
            shiftDto.LocationId = 50000;
            shiftDto.StartDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).AddYears(5); //Last week monday
            shiftDto.EndDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 5).AddYears(5); //Last week tuesday
            shiftDto.SheriffId = sheriffId;

            var shiftDtos = new List<ShiftDto> {shiftDto};
            var shift = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.AddShifts(shiftDtos.Adapt<List<AddShiftDto>>()));

            //Add in shift at a location, with a Sheriff that has no away location or home location.. to simulate a move. This shift shouldn't be picked up. 
            shiftDto.LocationId = 50000;
            shiftDto.StartDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).AddYears(5); //Last week monday
            shiftDto.EndDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 5).AddYears(5); //Last week tuesday
            shiftDto.SheriffId = sheriffId2;

            var importedShiftsResponse = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(
                await ShiftController.ImportWeeklyShifts(50000, shiftDto.StartDate));

            var importedShifts = importedShiftsResponse.Shifts.OrderBy(s => s.StartDate).ToList();
            Assert.NotNull(importedShifts);
            Assert.True(importedShifts.Count == 3); // 3 due to splitting
            Assert.Equal(shiftDto.StartDate.AddDays(7).DateTime, importedShifts.First().StartDate.DateTime, TimeSpan.FromSeconds(10)); //This week monday
            Assert.Equal(shiftDto.EndDate.AddDays(7).DateTime, importedShifts.Last().EndDate.DateTime, TimeSpan.FromSeconds(10)); //This week monday
            Assert.Equal(sheriffId, importedShifts.First().SheriffId);
        }

        [Fact]
        public async Task ShiftOvertime()
        {
            //Create Location, shift.. get 
            var locationId = await CreateLocation();
            var sheriffId = Guid.NewGuid();
            await Db.CourtAdmin.AddAsync(new CourtAdmin { Id = sheriffId, IsEnabled = true, HomeLocationId = locationId });
            await Db.SaveChangesAsync();

            var shiftStartDate = DateTimeOffset.UtcNow.AddYears(5).ConvertToTimezone("America/Vancouver").DateOnly();
            var addShifts = new List<Shift>
            {
                new Shift
                {
                    LocationId = locationId,
                    StartDate = shiftStartDate.AddHours(24),
                    EndDate = shiftStartDate.AddHours(25),
                    ExpiryDate = null,
                    SheriffId = sheriffId,
                    Timezone = "America/Vancouver"
                },
                new Shift
                {
                    LocationId = locationId,
                    StartDate = shiftStartDate,
                    EndDate = shiftStartDate.AddHours(5),
                    ExpiryDate = null,
                    SheriffId = sheriffId,
                    Timezone = "America/Vancouver"
                },
                new Shift
                {
                    LocationId = locationId,
                    StartDate = shiftStartDate.AddHours(5),
                    EndDate = shiftStartDate.AddHours(6),
                    SheriffId = sheriffId,
                    Timezone = "America/Vancouver"
                },
                new Shift
                {
                    LocationId = locationId,
                    StartDate = shiftStartDate.AddHours(9),
                    EndDate = shiftStartDate.AddHours(18),
                    SheriffId = sheriffId,
                    Timezone = "America/Vancouver"
                }
            };
         
            var controllerResult = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.AddShifts(addShifts.Adapt<List<AddShiftDto>>()));

            var shiftConflicts = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(
                await ShiftController.GetAvailability(locationId, shiftStartDate, shiftStartDate.AddDays(1)));

            var conflicts = shiftConflicts.SelectMany(s => s.Conflicts);
            var conflict = conflicts.FirstOrDefault(s => s.End == shiftStartDate.AddHours(18));
            Assert.NotNull(conflict);
            //SHould have 7 hours of overtime.  12 -> 5 -> 6 <--> 9 -> 18 = 15 hours - 8 hours regular shift = 7 hours overtime. 
            Assert.Equal(7.0, conflicts.Sum(s=> s.OvertimeHours));
        }

        private async Task<int> CreateLocation()
        {
            var location = new Location { Id = 50000, AgencyId = "5555", Name = "dfd" };
            await Db.Location.AddAsync(location);
            await Db.SaveChangesAsync();
            return location.Id;
        }

        private async Task<ShiftDto> CreateShift()
        {
            var sheriffId = Guid.NewGuid();
            await Db.Location.AddAsync(new Location { Id = 50000, AgencyId = "zz", Timezone = "America/Vancouver"});
            await Db.CourtAdmin.AddAsync(new CourtAdmin { Id = sheriffId, HomeLocationId = 50000, FirstName = "First", LastName = "Sheriff", IsEnabled = true });
            await Db.SaveChangesAsync();

            var shiftDto = new ShiftDto
            {
                ExpiryDate = DateTimeOffset.UtcNow, // should be null.
                SheriffId = sheriffId, // should be null.
                StartDate = DateTimeOffset.UtcNow,
                EndDate = DateTimeOffset.UtcNow.AddHours(5),
                Sheriff = new SheriffDto(),
                AnticipatedAssignment = null,
                Location = new LocationDto { Id = 55, AgencyId = "55" },
                LocationId = 50000,
                Timezone = "America/Edmonton"
            };
            return shiftDto;
        }
    }
}
