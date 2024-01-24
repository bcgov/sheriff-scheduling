using CAS.API.services;
using System;
using System.Threading.Tasks;
using CAS.API.controllers;
using CAS.API.Models.DB;
using CAS.API.models.dto;
using CAS.API.models.dto.generated;
using CAS.DB.models.lookupcodes;
using tests.api.helpers;
using tests.api.Helpers;
using Xunit;

namespace tests.controllers
{
    /// <summary>
    /// I've created one test here, because the manage types are very similar in controller / service structure. 
    /// </summary>
    public class ManageTypesControllerTests : WrapTransactionScopeInMemory
    {
        #region Variables
        private readonly ManageTypesController _controller;
        #endregion Variables

        public ManageTypesControllerTests() : base(true)
        {
            _controller = new ManageTypesController(new ManageTypesService(Db), Db)
            {
                ControllerContext = HttpResponseTest.SetupMockControllerContext()
            };
        }

        [Fact]
        public async Task AddLookup()
        {
            await Db.Database.EnsureDeletedAsync();
            var courtRole = new AddLookupCodeDto
            {
                Type = LookupTypes.CourtRole,
            };
            var controllerResult = await _controller.Add(courtRole);
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);
            Assert.True(response.Id > 0);
            await Db.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task GetLookupCodes()
        {
            await Db.Database.EnsureDeletedAsync();
            await AddCourtRole();
            var controllerResult = await _controller.GetAll(LookupTypes.CourtRole, 5);
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);
            Assert.True(response.Count >= 1);

            var controllerResult2 = await _controller.GetAll(LookupTypes.EscortRun, 5);
            var response2 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult2);
            Assert.True(response2.Count == 0);
        }

        [Fact]
        public async Task UpdateLookupCode()
        {
            await Db.Database.EnsureDeletedAsync();
            var newLocation = new Location { Name = "6", Id = 6 };
            await Db.Location.AddAsync(newLocation);
            await Db.SaveChangesAsync();

            Detach();

            var id = await AddCourtRole();
            var result = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await _controller.Find(id));

            Detach(); //We get an exception if we don't detach before the update. 

            result.Description = "test";
            result.EffectiveDate = DateTime.Now;
            result.Code = "gg";
            result.SubCode = "gg2";
            result.Type = LookupTypes.JailRole;
            result.LocationId = 6;
            result.SortOrderForLocation = new LookupSortOrderDto
            {
                LocationId = 6,
                LookupCodeId = 6
            };

            var controllerResult = await _controller.Update(result);
            HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);

            result = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await _controller.Find(id));

            Assert.Equal("test", result.Description);
            Assert.Equal(DateTimeOffset.Now.DateTime, result.EffectiveDate.Value.DateTime, TimeSpan.FromSeconds(10));
            Assert.Equal(LookupTypes.JailRole, result.Type);
            Assert.Equal("gg", result.Code);
            Assert.Equal("gg2", result.SubCode);
            Assert.Equal(6, result.LocationId);
        }

        [Fact]
        public async Task LocationTest()
        {
            await Db.Database.EnsureDeletedAsync();
            var newLocation = new Location { Name = "5", Id = 5 };
            await Db.Location.AddAsync(newLocation);
            await Db.SaveChangesAsync();

            Detach();

            var courtRole = new AddLookupCodeDto
            {
                Type = LookupTypes.CourtRole,
                LocationId = 66
            };
            var controllerResult = await _controller.Add(courtRole);
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);
            Assert.True(response.Id > 0);

            controllerResult = await _controller.Find(response.Id);
            response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);

            Assert.Null(response.Location);


            courtRole = new AddLookupCodeDto
            {
                Type = LookupTypes.CourtRole,
                LocationId = 5
            };
            controllerResult = await _controller.Add(courtRole);
            response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);
            Assert.True(response.Id > 0);

            controllerResult = await _controller.Find(response.Id);
            response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);

            Assert.Equal(5, response.LocationId);
        }

        [Fact]
        public async Task ExpireAndUnExpireLookup()
        {
            await Db.Database.EnsureDeletedAsync();
            var id = await AddCourtRole();
            var controllerResult = await _controller.Expire(id);
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);

            Assert.NotNull(response.ExpiryDate);

            controllerResult = await _controller.UnExpire(id);
            response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);

            Assert.Null(response.ExpiryDate);

        }

        #region Helpers
        private async Task<int> AddCourtRole()
        {
            var courtRole = new AddLookupCodeDto
            {
                Description = "test",
                Type = LookupTypes.CourtRole,
                LocationId = 5
            };
            var controllerResult = await _controller.Add(courtRole);
            var result = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);
            return result.Id;
        }

        #endregion Helpers
    }
}
