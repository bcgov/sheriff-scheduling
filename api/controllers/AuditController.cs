using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CAS.API.infrastructure.authorization;
using CAS.API.models.dto;
using CAS.API.models.dto.generated;
using CAS.API.services.usermanagement;
using CAS.DB.models;
using CAS.DB.models.auth;

namespace CAS.API.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditController : ControllerBase
    {
        public CourtAdminService CourtAdminService { get; }
        public CourtAdminDbContext Db { get; }
        public const string CouldNotFindCourtAdminError = "Couldn't find court admin.";

        public AuditController(CourtAdminService courtAdminService, CourtAdminDbContext db)
        {
            CourtAdminService = courtAdminService;
            Db = db;
        }

        [HttpGet("roleHistory")]
        [PermissionClaimAuthorize(perm: Permission.CreateAndAssignRoles)]
        public async Task<ActionResult<List<AuditDto>>> ViewRoleHistory(Guid courtAdminId)
        {
            var courtAdmin = await CourtAdminService.GetCourtAdmin(courtAdminId, null);
            if (courtAdmin == null) return NotFound(CouldNotFindCourtAdminError);
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, courtAdmin.HomeLocationId)) return Forbid();

            var userRoleIds = Db.UserRole.AsNoTracking().Where(ur => ur.UserId == courtAdminId).Select(ur => ur.Id);
            var roleHistory = Db.Audit.AsNoTracking().Include(a => a.CreatedBy).Where(e => e.TableName == "UserRole" &&
                                                  userRoleIds.Contains(e.KeyValues.RootElement.GetProperty("Id")
                                                      .GetInt32()))
                .ToList();

            //Have to select, because we have adapt ignore on these properties. 
            return Ok(roleHistory.Select(s =>
            {
                var audit = s.Adapt<AuditDto>();
                audit.CreatedBy = s.CreatedBy.Adapt<CourtAdminDto>();
                audit.CreatedOn = s.CreatedOn;
                audit.CreatedById = s.CreatedById;
                return audit;
            }));
        }
    }
}
