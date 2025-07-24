using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SS.Api.controllers.usermanagement;
using SS.Api.models.dto;
using SS.Api.services.scheduling;
using SS.Api.services.usermanagement;
using SS.Db.models.auth;
using tests.api.helpers;
using tests.api.Helpers;
using Xunit;
using Microsoft.Extensions.Logging;
using SS.Api.services;

namespace tests.controllers
{
    /// <summary>
    /// This uses SheriffController which is derived from UserController.
    /// This tests the more general user operations.
    /// </summary>
    public class UserControllerTests : WrapInTransactionScope
    {
        #region Variables
        private readonly SheriffController _controller;
        #endregion Variables

        public UserControllerTests() : base (false)
        {
            var environment = new EnvironmentBuilder("LocationServicesClient:Username", "LocationServicesClient:Password", "LocationServicesClient:Url");
            var httpContextAccessor = new HttpContextAccessor { HttpContext = HttpResponseTest.SetupHttpContext() };
            var managedTypesService = new ManageTypesService(Db);
            var trainingService = new TrainingService(managedTypesService, Db, environment.LogFactory.CreateLogger<TrainingService>());
            var sheriffService = new SheriffService(Db, environment.Configuration, httpContextAccessor);
            var shiftService = new ShiftService(Db, sheriffService, environment.Configuration);
            var dutyRosterService = new DutyRosterService(Db, environment.Configuration,
                shiftService, environment.LogFactory.CreateLogger<DutyRosterService>());
            _controller = new SheriffController(sheriffService, dutyRosterService, shiftService, new UserService(Db), trainingService, environment.Configuration, Db)
            {
                ControllerContext = HttpResponseTest.SetupMockControllerContext()
            };
        }

        [Fact]
        public async Task AssignAndUnassignRoles()
        {
            var user = await CreateUser();
            var role = await CreateRole();

            var controllerResult = await _controller.AssignRoles(new List<AssignRoleDto> {new AssignRoleDto { UserId = user.Id, RoleId = role.Id, EffectiveDate = DateTimeOffset.UtcNow }});
            HttpResponseTest.CheckForNoContentResponse(controllerResult);

            var entity = await Db.User.FindAsync(user.Id);
            Assert.True(entity.UserRoles.Count > 0);

            controllerResult = await _controller.UnassignRoles(new List<UnassignRoleDto> { new UnassignRoleDto { UserId = user.Id, RoleId = role.Id } });
            HttpResponseTest.CheckForNoContentResponse(controllerResult);

            entity = await Db.User.FindAsync(user.Id);
            Assert.True(entity.UserRoles.Count > 0);
            Assert.NotNull(entity.UserRoles.FirstOrDefault().ExpiryDate);
        }


        [Fact]
        public async Task EnableUser()
        {
            var userObject = await CreateUser();

            var controllerResult = await _controller.EnableUser(userObject.Id);
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);


            var dbSheriff = await Db.User.FindAsync(userObject.Id);
            Assert.NotNull(dbSheriff);
            Assert.True(dbSheriff.IsEnabled);
        }

        [Fact]
        public async Task DisableUser()
        {
            var userObject = await CreateUser();

            var controllerResult = await _controller.DisableUser(userObject.Id);
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);

            var dbSheriff = await Db.User.FindAsync(response.Id);
            Assert.NotNull(dbSheriff);
            Assert.False(dbSheriff.IsEnabled);
        }

        private async Task<Role> CreateRole()
        {
            var newRole = new Role
            {
                Description = "The big boss",
                Name = "BigBoss"
            };
            await Db.Role.AddAsync(newRole);
            await Db.SaveChangesAsync();
            return newRole;
        }

        private async Task<User> CreateUser()
        {
            var newUser = new User
            {
                FirstName = "Ted",
                LastName = "Tums",
                Email = "Ted@Teddy.com",
                IdirId = new Guid(),
                IdirName = "ted@fakeidir"
            };

            await Db.User.AddAsync(newUser);
            await Db.SaveChangesAsync();
            return newUser;
        }

    }
}
