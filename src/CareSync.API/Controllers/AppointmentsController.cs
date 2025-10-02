using CareSync.Application.Commands.Appointments;
using CareSync.Application.Queries.Appointments;
using CareSync.Application.DTOs.Appointments;
using CareSync.Application.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareSync.API.Controllers;

#if DEBUG
[AllowAnonymous]
#else
[Authorize(Policy = "CanManageAppointments")]
#endif
[ApiController]
[Route("api/[controller]")]
public class AppointmentsController(IMediator mediator) : BaseApiController(mediator)
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAllAppointments()
    {
        var result = await _mediator.Send(new GetAllAppointmentsQuery());
        return OkOrProblem(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentDto>> GetAppointmentById(Guid id)
    {
        var byId = await _mediator.Send(new GetAppointmentByIdQuery(id));
        return OkOrNotFound(byId);
    }

    [HttpGet("patient/{patientId}")]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointmentsByPatient(Guid patientId)
    {
        var byPatient = await _mediator.Send(new GetAppointmentsByPatientQuery(patientId));
        return OkOrProblem(byPatient);
    }

    [HttpGet("doctor/{doctorId}")]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointmentsByDoctor(Guid doctorId)
    {
        var byDoctor = await _mediator.Send(new GetAppointmentsByDoctorQuery(doctorId));
        return OkOrProblem(byDoctor);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AppointmentDto>> CreateAppointment([FromBody] CreateAppointmentDto createAppointmentDto)
    {
        var created = await _mediator.Send(new CreateAppointmentCommand(createAppointmentDto));
        return CreatedOrBadRequest(created, nameof(GetAppointmentById), new { id = created.Value?.Id });
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AppointmentDto>> UpdateAppointment(Guid id, [FromBody] UpdateAppointmentDto updateAppointmentDto)
    {
        if (id != updateAppointmentDto.Id) return BadRequest("ID mismatch between route and body");
        var updated = await _mediator.Send(new UpdateAppointmentCommand(updateAppointmentDto));
        return UpdatedOrNotFound(updated);
    }

    [HttpPut("upsert")]
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AppointmentDto>> UpsertAppointment([FromBody] UpsertAppointmentDto upsertAppointmentDto)
    {
        var upserted = await _mediator.Send(new UpsertAppointmentCommand(upsertAppointmentDto));
        return UpsertOkOrBadRequest(upserted);
    }

    [HttpPut("{id}/cancel")]
#if !DEBUG
    [Authorize(Policy = "CanMoveAppointmentLifecycle")]
#endif
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AppointmentDto>> CancelAppointment(Guid id, [FromBody] CancelAppointmentDto cancelAppointmentDto)
    {
        if (id != cancelAppointmentDto.Id) return BadRequest("Appointment ID mismatch");
        var canceled = await _mediator.Send(new CancelAppointmentCommand(cancelAppointmentDto));
        return UpdatedOrNotFound(canceled);
    }

    [HttpPut("{id}/complete")]
#if !DEBUG
    [Authorize(Policy = "CanMoveAppointmentLifecycle")]
#endif
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AppointmentDto>> CompleteAppointment(Guid id, [FromBody] CompleteAppointmentRequest request)
    {
        var completed = await _mediator.Send(new CompleteAppointmentCommand(id, request.Notes));
        return UpdatedOrNotFound(completed);
    }

    [HttpPut("{id}/start")]
#if !DEBUG
    [Authorize(Policy = "CanMoveAppointmentLifecycle")]
#endif
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AppointmentDto>> StartAppointment(Guid id)
    {
        var started = await _mediator.Send(new StartAppointmentCommand(id));
        return UpdatedOrNotFound(started);
    }

    [HttpPut("{id}/no-show")]
#if !DEBUG
    [Authorize(Policy = "CanMoveAppointmentLifecycle")]
#endif
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AppointmentDto>> MarkNoShow(Guid id)
    {
        var noShow = await _mediator.Send(new MarkNoShowAppointmentCommand(id));
        return UpdatedOrNotFound(noShow);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAppointment(Guid id)
    {
        var deleted = await _mediator.Send(new DeleteAppointmentCommand(id));
        return NoContentOrNotFound(deleted);
    }
}

public class CompleteAppointmentRequest
{
    public string? Notes { get; set; }
}
