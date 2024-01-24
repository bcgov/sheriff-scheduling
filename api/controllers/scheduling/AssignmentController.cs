using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using CAS.API.infrastructure.authorization;
using CAS.API.models.dto.generated;
using CAS.API.services.scheduling;
using CAS.DB.models;
using CAS.DB.models.auth;
using CAS.DB.models.scheduling;

namespace CAS.API.controllers.scheduling
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        public const string InvalidAssignmentError = "Invalid Assignment.";
        private AssignmentService AssignmentService { get; }
        private CourtAdminDbContext Db { get; }

        public AssignmentController(AssignmentService assignmentService, CourtAdminDbContext db)
        {
            AssignmentService = assignmentService;
            Db = db;
        }

        /// <summary>
        /// This is used as part of the DutyRoster screen, on the left hand side. 
        /// </summary>
        [HttpGet]
        [PermissionClaimAuthorize(perm: Permission.ViewDutyRoster)]
        public async Task<ActionResult<List<AssignmentDto>>> GetAssignments(int locationId, DateTimeOffset? start, DateTimeOffset? end)
        {
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, locationId)) return Forbid();

            var assignments = await AssignmentService.GetAssignments(locationId, start, end);
            return Ok(assignments.Adapt<List<AssignmentDto>>());
        }

        /// <summary>
        /// Adhoc or Default assignments can be created here. 
        /// </summary>
        [HttpPost]
        [PermissionClaimAuthorize(perm: Permission.CreateAssignments)]
        public async Task<ActionResult<AssignmentDto>> AddAssignment(AddAssignmentDto assignmentDto)
        {
            if (assignmentDto == null) return BadRequest(InvalidAssignmentError);
            if (assignmentDto.Start >= assignmentDto.End) return BadRequest("Start time was on or after end time.");
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, assignmentDto.LocationId)) return Forbid();

            var assignment = assignmentDto.Adapt<Assignment>();
            var createdAssignment = await AssignmentService.CreateAssignment(assignment);
            return Ok(createdAssignment.Adapt<AssignmentDto>());
        }

        [HttpPut]
        [PermissionClaimAuthorize(perm: Permission.EditAssignments)]
        public async Task<ActionResult<AssignmentDto>> UpdateAssignment(UpdateAssignmentDto assignmentDto)
        {
            if (assignmentDto == null) return BadRequest(InvalidAssignmentError);
            if (assignmentDto.Start >= assignmentDto.End) return BadRequest("Start time was on or after end time.");
            var savedAssignment = await AssignmentService.GetAssignment(assignmentDto.Id);
            if (savedAssignment == null) return NotFound();
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, savedAssignment.LocationId)) return Forbid();

            var updateAssignment = assignmentDto.Adapt<Assignment>();
            var updatedAssignment = await AssignmentService.UpdateAssignment(updateAssignment);
            return Ok(updatedAssignment.Adapt<AssignmentDto>());
        }

        [HttpDelete]
        [PermissionClaimAuthorize(perm: Permission.ExpireAssignments)]
        public async Task<ActionResult> ExpireAssignment(int id, string expiryReason, DateTimeOffset? expiryDate = null)
        {
            var savedAssignment = await AssignmentService.GetAssignment(id);
            if (savedAssignment == null) return NotFound();
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, savedAssignment.LocationId)) return Forbid();

            await AssignmentService.ExpireAssignment(id, expiryReason, expiryDate);
            return NoContent();
        }
    }
}
