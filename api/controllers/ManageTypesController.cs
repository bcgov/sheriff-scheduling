using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using CAS.API.infrastructure.authorization;
using CAS.API.models.dto;
using CAS.API.models.dto.generated;
using CAS.API.services;
using CAS.DB.models;
using CAS.DB.models.auth;
using CAS.DB.models.lookupcodes;

namespace CAS.API.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManageTypesController : ControllerBase
    {
        public const string InvalidLookupCodeError = "Invalid LookupCode.";

        private ManageTypesService ManageTypesService { get; }
        private CourtAdminDbContext Db { get; }

        public ManageTypesController(ManageTypesService manageTypesService, CourtAdminDbContext db)
        {
            ManageTypesService = manageTypesService;
            Db = db;
        }
        
        [HttpGet]
        [Route("{id}")]
        [PermissionClaimAuthorize(perm: Permission.Login)]
        public async Task<ActionResult<LookupCodeDto>> Find(int id)
        {
            var entity = await ManageTypesService.Find(id);
            if (entity == null) return NotFound();

            return Ok(entity.Adapt<LookupCodeDto>());
        }

        [HttpGet]
        [PermissionClaimAuthorize(perm: Permission.Login)]
        public async Task<ActionResult<List<LookupCodeDto>>> GetAll(LookupTypes? codeType, int? locationId, bool showExpired = false)
        {
            var lookupCodesDtos = (await ManageTypesService.GetAll(codeType, locationId, showExpired)).Adapt<List<LookupCodeDto>>();
            return Ok(lookupCodesDtos);
        }

        [HttpPost]
        [PermissionClaimAuthorize(perm: Permission.EditTypes)]
        public async Task<ActionResult<LookupCodeDto>> Add(AddLookupCodeDto lookupCodeDto)
        {
            if (lookupCodeDto == null) return BadRequest(InvalidLookupCodeError);
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, lookupCodeDto.LocationId)) return Forbid();

            var lookupCode = await ManageTypesService.Add(lookupCodeDto);
            return Ok(lookupCode.Adapt<LookupCodeDto>());
        }

        [HttpPut("{id}/expire")]
        [PermissionClaimAuthorize(perm: Permission.ExpireTypes)]
        public async Task<ActionResult<LookupCodeDto>> Expire(int id)
        {
            var entity = await ManageTypesService.Find(id);
            if (entity == null) return NotFound();
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, entity.LocationId)) return Forbid();

            var lookupCode = await ManageTypesService.Expire(id);
            return Ok(lookupCode.Adapt<LookupCodeDto>());
        }

        [HttpPut("{id}/unexpire")]
        [PermissionClaimAuthorize(perm: Permission.ExpireTypes)]
        public async Task<ActionResult<LookupCodeDto>> UnExpire(int id)
        {
            var entity = await ManageTypesService.Find(id);
            if (entity == null) return NotFound();
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, entity.LocationId)) return Forbid();

            var lookupCode = await ManageTypesService.Unexpire(id);
            return Ok(lookupCode.Adapt<LookupCodeDto>());
        }

        [HttpPut]
        [PermissionClaimAuthorize(perm: Permission.EditTypes)]
        public async Task<ActionResult<LookupCodeDto>> Update(LookupCodeDto lookupCodeDto)
        {
            if (lookupCodeDto == null) return BadRequest(InvalidLookupCodeError);
            var entity = await ManageTypesService.Find(lookupCodeDto.Id);
            if (entity == null) return NotFound();
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, lookupCodeDto.LocationId)) return Forbid();

            var lookupCode = lookupCodeDto.Adapt<LookupCode>();
            var lookupCodeResult = await ManageTypesService.Update(lookupCode);
            return Ok(lookupCodeResult.Adapt<LookupCodeDto>());
        }

        [HttpPut("updateSort")]
        [PermissionClaimAuthorize(perm: Permission.EditTypes)]
        public async Task<ActionResult> UpdateSortOrders(SortOrdersDto sortOrdersDto)
        {
            if (sortOrdersDto == null) return BadRequest(InvalidLookupCodeError);
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, sortOrdersDto.SortOrderLocationId)) return Forbid();

            await ManageTypesService.UpdateSortOrders(sortOrdersDto);
            return NoContent();
        }

    }
}
