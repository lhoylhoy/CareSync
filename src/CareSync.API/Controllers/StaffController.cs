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
    public async Task<ActionResult<IEnumerable<StaffDto>>> GetAllStaff()
    {
        var result = await _mediator.Send(new GetAllStaffQuery());
        return OkOrProblem(result);
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
