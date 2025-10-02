using CareSync.Application.Commands.Doctors;
using CareSync.Application.Queries.Doctors;
using CareSync.Application.DTOs.Doctors;
using CareSync.Application.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareSync.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorsController(IMediator mediator) : BaseApiController(mediator)
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DoctorDto>>> GetAllDoctors()
    {
        var result = await _mediator.Send(new GetAllDoctorsQuery());
        return OkOrProblem(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DoctorDto>> GetDoctorById(Guid id)
    {
        var result = await _mediator.Send(new GetDoctorByIdQuery(id));
        return OkOrNotFound(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(DoctorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DoctorDto>> CreateDoctor([FromBody] CreateDoctorDto createDoctorDto)
    {
        var result = await _mediator.Send(new CreateDoctorCommand(createDoctorDto));
        return CreatedOrBadRequest(result, nameof(GetDoctorById), new { id = result.Value?.Id });
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(DoctorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DoctorDto>> UpdateDoctor(Guid id, [FromBody] UpdateDoctorDto updateDoctorDto)
    {
        if (id != updateDoctorDto.Id) return BadRequest("ID mismatch between route and body");
        var result = await _mediator.Send(new UpdateDoctorCommand(updateDoctorDto));
        return UpdatedOrNotFound(result);
    }

    [HttpPut("upsert")]
    [ProducesResponseType(typeof(DoctorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DoctorDto>> UpsertDoctor([FromBody] UpsertDoctorDto upsertDoctorDto)
    {
        var result = await _mediator.Send(new UpsertDoctorCommand(upsertDoctorDto));
        return UpdatedOrNotFound(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteDoctor(Guid id)
    {
        var result = await _mediator.Send(new DeleteDoctorCommand(id));
        return NoContentOrNotFound(result);
    }
}
