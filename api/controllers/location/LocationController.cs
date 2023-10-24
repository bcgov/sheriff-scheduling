using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CAS.API.helpers.extensions;
using CAS.API.infrastructure.authorization;
using CAS.API.models.dto.generated;
using CAS.DB.models;
using CAS.DB.models.auth;

namespace CAS.API.controllers
{
    /// <summary>
    /// Used to fetch Locations, plus expire locations. These locations are inserted by JCDataUpdaterService.
    /// </summary>
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private CourtAdminDbContext Db { get; }

        public LocationController(CourtAdminDbContext dbContext)
        {
            Db = dbContext;
        }

        [HttpGet]
        [PermissionClaimAuthorize(perm: Permission.Login)]
        public async Task<ActionResult<List<LocationDto>>> Locations()
        {
            var locations = await Db.Location.ApplyPermissionFilters(User,Db).ToListAsync();
            return Ok(locations.Adapt<List<LocationDto>>());
        }

        [HttpGet]
        [Route("all")]
        [PermissionClaimAuthorize(perm: Permission.Login)]
        public async Task<ActionResult<List<LocationDto>>> AllLocations()
        {
            var locations = await Db.Location.ToListAsync();
            return Ok(locations.Adapt<List<LocationDto>>());
        }

        [HttpPut]
        [Route("{id}/enable")]
        [PermissionClaimAuthorize(perm: Permission.ExpireLocation)]
        public async Task<ActionResult> EnableLocation(int id)
        {
            var location = await Db.Location.FindAsync(id);
            location.ThrowBusinessExceptionIfNull($"Couldn't find location with id {id}");
            location.ExpiryDate = null;
            await Db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut]
        [Route("{id}/disable")]
        [PermissionClaimAuthorize(perm: Permission.ExpireLocation)]
        public async Task<ActionResult> DisableLocation(int id)
        {
            var location = await Db.Location.FindAsync(id);
            location.ThrowBusinessExceptionIfNull($"Couldn't find location with id {id}");
            location.ExpiryDate = DateTime.UtcNow;
            await Db.SaveChangesAsync();
            return NoContent();
        }
    }
}
