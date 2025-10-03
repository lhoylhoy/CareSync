using System;
using System.Collections.Generic;
using CareSync.Application.Commands.Staff;
using CareSync.Application.Common.Results;
using CareSync.Application.DTOs.Staff;
using CareSync.Application.Queries.Staff;
using CareSync.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareSync.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StaffController(IMediator mediator) : BaseApiController(mediator)
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    /// <summary>
    ///     Get all staff members
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllStaff(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 0,
        [FromQuery] string? search = null,
        [FromQuery] Dictionary<string, string?>? filters = null)
    {
        var hasQueryOverrides = pageSize > 0 || !string.IsNullOrWhiteSpace(search) || (filters?.Count > 0);

        if (!hasQueryOverrides)
        {
            var allResult = await _mediator.Send(new GetAllStaffQuery());
            var response = OkOrProblem(allResult);
            if (response.Result is IActionResult actionResult)
            {
                return actionResult;
            }
            return Ok(response.Value);
        }

        var effectivePageSize = pageSize > 0 ? pageSize : 25;
        var pagedResult = await _mediator.Send(new GetStaffPagedQuery(
            page <= 0 ? 1 : page,
            effectivePageSize,
            search,
            filters ?? new Dictionary<string, string?>()));

        var pagedResponse = OkOrProblem(pagedResult);
        if (pagedResponse.Result is IActionResult failure)
        {
            return failure;
        }

        return Ok(pagedResponse.Value);
    }

    /// <summary>
    ///     Get a staff member by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<StaffDto>> GetStaffById(Guid id)
    {
        var result = await _mediator.Send(new GetStaffByIdQuery(id));
        return OkOrNotFound(result);
    }

    /// <summary>
    ///     Get staff filtered by role
    /// </summary>
    /// <param name="role">Staff role enum value</param>
    [HttpGet("role/{role}")]
    public async Task<ActionResult<IEnumerable<StaffDto>>> GetStaffByRole(StaffRole role)
    {
        var result = await _mediator.Send(new GetStaffByRoleQuery(role));
        return OkOrProblem(result);
    }

    /// <summary>
    ///     Create a new staff member
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<StaffDto>> CreateStaff([FromBody] CreateStaffDto createStaffDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var result = await _mediator.Send(new CreateStaffCommand(createStaffDto));
        return CreatedOrBadRequest(result, nameof(GetStaffById), new { id = result.Value?.Id });
    }

    /// <summary>
    ///     Update a staff member
    /// </summary>
    /// <param name="id">Staff ID</param>
    /// <param name="updateStaff">Staff update data</param>
    /// <returns>Updated staff member</returns>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<StaffDto>> UpdateStaff(Guid id, [FromBody] UpdateStaffDto updateStaff)
    {
        if (id != updateStaff.Id)
            return BadRequest("ID mismatch between route and body");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var result = await _mediator.Send(new UpdateStaffCommand(updateStaff));
        return UpdatedOrNotFound(result);
    }

    /// <summary>
    ///     Upsert a staff member (create or update)
    /// </summary>
    [HttpPut("upsert")]
    public async Task<ActionResult<StaffDto>> UpsertStaff([FromBody] UpsertStaffDto upsertStaffDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var result = await _mediator.Send(new UpsertStaffCommand(upsertStaffDto));
        return UpdatedOrNotFound(result);
    }

    /// <summary>
    ///     Delete a staff member by ID
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteStaff(Guid id)
    {
        var result = await _mediator.Send(new DeleteStaffCommand(id));
        return NoContentOrNotFound(result);
    }
}
