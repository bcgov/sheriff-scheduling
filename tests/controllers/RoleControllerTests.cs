using Mapster;
using CAS.API.controllers.usermanagement;
using CAS.DB.models.auth;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CAS.API.models.dto;
using CAS.API.models.dto.generated;
using CAS.API.services.usermanagement;
using tests.api.helpers;
using tests.api.Helpers;
using Xunit;

namespace tests.controllers
{
    public class RoleControllerTests : WrapInTransactionScope
    {
        private readonly RoleController _controller;
        public RoleControllerTests() : base(false)
        {
            _controller = new RoleController(new RoleService(Db))
            {
                ControllerContext = HttpResponseTest.SetupMockControllerContext()
            };
        }

        [Fact]
        public async Task GetRoles()
        {
            var role = await CreateRole();

            var controllerResult = await _controller.Roles();
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);
       
            Assert.NotNull(response.FirstOrDefault(r => r.Id == role.Id));
        }

        [Fact]
        public async Task AddRole()
        {
            var roleDto = new RoleDto
            {
                Description = "Super Role",
                Name = "Super Super"
            };

            var addRoleDto = new AddRoleDto()
            {
                Role = roleDto,
                PermissionIds = new List<int> {1}
            };

            var controllerResult = await _controller.AddRole(addRoleDto);
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);

            Assert.Equal(roleDto.Description, response.Description);
            Assert.Equal(roleDto.Name, response.Name);
            Assert.NotEqual(roleDto.Id, response.Id);
        }
        
        [Fact]
        public async Task UpdateRoleAndAssignPermission()
        {
            var role = await CreateRole();
            role.Name = "hello";
            role.Description = "sugar";

            var permission = await CreatePermission();

            Detach();

            var updateRoleDto = new UpdateRoleDto 
            {
                    Role = role.Adapt<RoleDto>(),
                    PermissionIds = new List<int> { permission.Id }
            };

            var controllerResult = await _controller.UpdateRole(updateRoleDto.Adapt<UpdateRoleDto>());
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);

            Assert.Equal(role.Name, response.Name);
            Assert.Equal(role.Description, response.Description);
            Assert.Equal(role.Id, response.Id);

            var dbRole = await Db.Role.Include(r => r.RolePermissions).FirstOrDefaultAsync(r => r.Id == role.Id);
            Assert.NotEmpty(dbRole.RolePermissions);

            Detach();

            updateRoleDto = new UpdateRoleDto
            {
                Role = role.Adapt<RoleDto>(),
                PermissionIds = new List<int> { }
            };

            controllerResult = await _controller.UpdateRole(updateRoleDto.Adapt<UpdateRoleDto>());
            response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);

            Assert.Equal(role.Name, response.Name);
            Assert.Equal(role.Description, response.Description);
            Assert.Equal(role.Id, response.Id);

            dbRole = await Db.Role.Include(r => r.RolePermissions).FirstOrDefaultAsync(r => r.Id == role.Id);
            Assert.Empty(dbRole.RolePermissions);
        }

        [Fact]
        public async Task RemoveRole()
        {
            var role = await CreateRole();

            var controllerResult = await _controller.RemoveRole(role.Id);
            HttpResponseTest.CheckForNoContentResponse(controllerResult);

            Assert.Null((await Db.Role.FindAsync(role.Id)));
        }

        private async Task<Permission> CreatePermission()
        {
            var permission = new Permission {Name = "Good Perm", Description = "hello"};
            await Db.Permission.AddAsync(permission);
            await Db.SaveChangesAsync();
            return permission;
        }

        private async Task<Role> CreateRole()
        {
            var role = new Role {Name = "Super Role"};
            await Db.Role.AddAsync(role);
            await Db.SaveChangesAsync();
            return role;
        }
    }
}
