using CareSync.Application.Commands.Appointments;
using CareSync.Application.DTOs.Appointments;
using CareSync.Application.Queries.Appointments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace CareSync.API.Controllers;

#if DEBUG
[AllowAnonymous]
#else
[Authorize(Policy = "CanManageAppointments")]
#endif
[ApiController]
[Route("api/[controller]")]
public class AppointmentsController(IMediator mediator, ILogger<AppointmentsController> logger, IOutputCacheStore cacheStore) : BaseApiController(mediator)
{
    private readonly ILogger<AppointmentsController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IOutputCacheStore _cacheStore = cacheStore ?? throw new ArgumentNullException(nameof(cacheStore));

    [HttpGet]
    [OutputCache(PolicyName = "Appointments-All")]
    public async Task<IActionResult> GetAllAppointments(
        [FromQuery] int page = CareSync.Application.Common.PagingDefaults.DefaultPage,
        [FromQuery] int pageSize = 0,
        [FromQuery] string? search = null,
        [FromQuery] Dictionary<string, string?>? filters = null)
    {
        var hasQueryOverrides = pageSize > 0 || !string.IsNullOrWhiteSpace(search) || (filters?.Count > 0);

        if (!hasQueryOverrides)
        {
            var result = await Mediator.Send(new GetAllAppointmentsQuery());
            var response = OkOrProblem(result);
            if (response.Result is IActionResult actionResult)
            {
                return actionResult;
            }
            return Ok(response.Value);
        }

        var effectivePageSize = pageSize > 0 ? Math.Min(pageSize, CareSync.Application.Common.PagingDefaults.MaxPageSize) : CareSync.Application.Common.PagingDefaults.DefaultPageSize;
        var pagedResult = await Mediator.Send(new GetAppointmentsPagedQuery(
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

    [HttpGet("{id}")]
    [OutputCache(PolicyName = "Appointments-ById")]
    public async Task<ActionResult<AppointmentDto>> GetAppointmentById(Guid id)
    {
        var byId = await Mediator.Send(new GetAppointmentByIdQuery(id));
        return OkOrNotFound(byId);
    }

    [HttpGet("patient/{patientId}")]
    [OutputCache(PolicyName = "Appointments-ByPatient")]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointmentsByPatient(Guid patientId)
    {
        var byPatient = await Mediator.Send(new GetAppointmentsByPatientQuery(patientId));
        return OkOrProblem(byPatient);
    }

    [HttpGet("doctor/{doctorId}")]
    [OutputCache(PolicyName = "Appointments-ByDoctor")]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointmentsByDoctor(Guid doctorId)
    {
        var byDoctor = await Mediator.Send(new GetAppointmentsByDoctorQuery(doctorId));
        return OkOrProblem(byDoctor);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AppointmentDto>> CreateAppointment([FromBody] CreateAppointmentDto createAppointmentDto)
    {
        var created = await Mediator.Send(new CreateAppointmentCommand(createAppointmentDto));
        if (created.IsSuccess && created.Value is not null)
        {
            await InvalidateAppointmentCachesAsync(created.Value.Id, created.Value.PatientId, created.Value.DoctorId, HttpContext.RequestAborted);
        }
        return CreatedOrBadRequest(created, nameof(GetAppointmentById), new { id = created.Value?.Id });
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AppointmentDto>> UpdateAppointment(Guid id, [FromBody] UpdateAppointmentDto updateAppointmentDto)
    {
        if (id != updateAppointmentDto.Id) return BadRequest("ID mismatch between route and body");
        var updated = await Mediator.Send(new UpdateAppointmentCommand(updateAppointmentDto));
        if (updated.IsSuccess && updated.Value is not null)
        {
            await InvalidateAppointmentCachesAsync(updated.Value.Id, updated.Value.PatientId, updated.Value.DoctorId, HttpContext.RequestAborted);
        }
        return UpdatedOrNotFound(updated);
    }

    [HttpPut("upsert")]
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AppointmentDto>> UpsertAppointment([FromBody] UpsertAppointmentDto upsertAppointmentDto)
    {
        var upserted = await Mediator.Send(new UpsertAppointmentCommand(upsertAppointmentDto));
        if (upserted.IsSuccess && upserted.Value is not null)
        {
            await InvalidateAppointmentCachesAsync(upserted.Value.Id, upserted.Value.PatientId, upserted.Value.DoctorId, HttpContext.RequestAborted);
        }
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
        var canceled = await Mediator.Send(new CancelAppointmentCommand(cancelAppointmentDto));
        if (canceled.IsSuccess && canceled.Value is not null)
        {
            await InvalidateAppointmentCachesAsync(canceled.Value.Id, canceled.Value.PatientId, canceled.Value.DoctorId, HttpContext.RequestAborted);
        }
        return UpdatedOrNotFound(canceled);
    }

    [HttpPut("{id}/complete")]
#if !DEBUG
    [Authorize(Policy = "CanMoveAppointmentLifecycle")]
#endif
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AppointmentDto>> CompleteAppointment(Guid id, [FromBody] CompleteAppointmentRequest request)
    {
        var completed = await Mediator.Send(new CompleteAppointmentCommand(id, request.Notes));
        if (completed.IsSuccess && completed.Value is not null)
        {
            await InvalidateAppointmentCachesAsync(completed.Value.Id, completed.Value.PatientId, completed.Value.DoctorId, HttpContext.RequestAborted);
        }
        return UpdatedOrNotFound(completed);
    }

    [HttpPut("{id}/start")]
#if !DEBUG
    [Authorize(Policy = "CanMoveAppointmentLifecycle")]
#endif
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AppointmentDto>> StartAppointment(Guid id)
    {
        var started = await Mediator.Send(new StartAppointmentCommand(id));
        if (started.IsSuccess && started.Value is not null)
        {
            await InvalidateAppointmentCachesAsync(started.Value.Id, started.Value.PatientId, started.Value.DoctorId, HttpContext.RequestAborted);
        }
        return UpdatedOrNotFound(started);
    }

    [HttpPut("{id}/no-show")]
#if !DEBUG
    [Authorize(Policy = "CanMoveAppointmentLifecycle")]
#endif
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AppointmentDto>> MarkNoShow(Guid id)
    {
        var noShow = await Mediator.Send(new MarkNoShowAppointmentCommand(id));
        if (noShow.IsSuccess && noShow.Value is not null)
        {
            await InvalidateAppointmentCachesAsync(noShow.Value.Id, noShow.Value.PatientId, noShow.Value.DoctorId, HttpContext.RequestAborted);
        }
        return UpdatedOrNotFound(noShow);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAppointment(Guid id)
    {
        var deleted = await Mediator.Send(new DeleteAppointmentCommand(id));
        if (deleted.IsSuccess)
        {
            await InvalidateAppointmentCachesAsync(id, null, null, HttpContext.RequestAborted);
        }
        return NoContentOrNotFound(deleted);
    }

    private async Task InvalidateAppointmentCachesAsync(Guid appointmentId, Guid? patientId, Guid? doctorId, CancellationToken token)
    {
        try
        {
            await _cacheStore.EvictByTagAsync("appointments-all", token);
            await _cacheStore.EvictByTagAsync("appointments-byid", token);
            await _cacheStore.EvictByTagAsync("appointments-bypatient", token);
            await _cacheStore.EvictByTagAsync("appointments-bydoctor", token);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to evict appointment caches for AppointmentId {AppointmentId}, PatientId {PatientId}, DoctorId {DoctorId}", appointmentId, patientId, doctorId);
        }
    }
}

public class CompleteAppointmentRequest
{
    public string? Notes { get; set; }
}
