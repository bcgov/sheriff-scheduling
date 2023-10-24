using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CAS.API.models.dto.generated;
using CAS.DB.models;

namespace CAS.API.controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class RegionController : ControllerBase
    {
        private CourtAdminDbContext Db { get; }

        public RegionController(CourtAdminDbContext dbContext) {  Db = dbContext; }

        [HttpGet]
        public async Task<ActionResult<List<RegionDto>>> Regions()
        {
            var locations = await Db.Region.ToListAsync();
            return Ok(locations.Adapt<List<RegionDto>>());
        }
    }
}
