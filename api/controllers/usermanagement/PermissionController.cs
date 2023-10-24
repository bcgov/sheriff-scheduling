using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CAS.API.infrastructure.authorization;
using CAS.API.models.dto.generated;
using CAS.DB.models;
using CAS.DB.models.auth;

namespace CAS.API.controllers.usermanagement
{
    /// <summary>
    /// This just fetches our permissions, the permissions need to be determined at compile time. 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [PermissionClaimAuthorize(perm: Permission.CreateAndAssignRoles)]
    public class PermissionController : ControllerBase
    {
        private CourtAdminDbContext Db { get; }

        public PermissionController(CourtAdminDbContext dbContext) {  Db = dbContext;  }

        [HttpGet]
        public async Task<ActionResult<List<PermissionDto>>> GetPermissions()
        {
            var permissions = await Db.Permission.AsNoTracking().ToListAsync();
            return Ok(permissions.Adapt<List<PermissionDto>>());
        }
    }
}
