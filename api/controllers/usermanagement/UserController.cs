using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using CAS.API.infrastructure.authorization;
using CAS.API.models.dto;
using CAS.API.models.dto.generated;
using CAS.API.services.usermanagement;
using CAS.DB.models.auth;

namespace CAS.API.controllers.usermanagement
{
    /// <summary>
    /// This was made abstract, so it can be reused. The idea is you could take the User object and reuse with minimal changes in another project. 
    /// </summary>
    /// 
    public abstract class UserController : ControllerBase
    {
        private UserService UserService { get; }

        protected UserController(UserService userUserService)
        {
            UserService = userUserService;
        }

        [HttpPut]
        [Route("assignRoles")]
        [PermissionClaimAuthorize(perm: Permission.CreateAndAssignRoles)]
        public async Task<ActionResult> AssignRoles(List<AssignRoleDto> assignRole)
        {
            var entity = assignRole.Adapt<List<UserRole>>();
            await UserService.AssignRolesToUser(entity);
            return NoContent();
        }

        [HttpPut]
        [Route("unassignRoles")]
        [PermissionClaimAuthorize(perm: Permission.CreateAndAssignRoles)]
        public async Task<ActionResult> UnassignRoles(List<UnassignRoleDto> unassignRole)
        {
            var entity = unassignRole.Adapt<List<UserRole>>();
            await UserService.UnassignRoleFromUser(entity);
            return NoContent();
        }

        [HttpPut]
        [Route("{id}/enable")]
        [PermissionClaimAuthorize(perm: Permission.ExpireUsers)]
        public async Task<ActionResult<CourtAdminDto>> EnableUser(Guid id)
        {
            var user = await UserService.EnableUser(id);
            return Ok(user.Adapt<CourtAdminDto>());
        }

        [HttpPut]
        [Route("{id}/disable")]
        [PermissionClaimAuthorize(perm: Permission.ExpireUsers)]
        public async Task<ActionResult<CourtAdminDto>> DisableUser(Guid id)
        {
            var user = await UserService.DisableUser(id);
            return Ok(user.Adapt<CourtAdminDto>());
        }
    }
}
