using CareSync.Application.Commands.Patients;
using CareSync.Application.DTOs.Patients;
using CareSync.Application.Queries.Patients;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace CareSync.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController(IMediator mediator, ILogger<PatientsController> logger, IOutputCacheStore cacheStore) : BaseApiController(mediator)
{
    private readonly ILogger<PatientsController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IOutputCacheStore _cacheStore = cacheStore ?? throw new ArgumentNullException(nameof(cacheStore));

    [HttpGet]
    [OutputCache(PolicyName = "Patients-All")]
    [ProducesResponseType(typeof(IEnumerable<PatientDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PatientDto>>> GetAllPatients()
    {
        var result = await Mediator.Send(new GetAllPatientsQuery());
        return OkOrProblem(result);
    }

    [HttpGet("{id:guid}")]
    [OutputCache(PolicyName = "Patients-ById")]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientDto>> GetPatientById(Guid id)
    {
        var result = await Mediator.Send(new GetPatientByIdQuery(id));
        return OkOrNotFound(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PatientDto>> CreatePatient([FromBody] PatientDto patientDto)
    {
        var result = await Mediator.Send(new CreatePatientCommand(patientDto));
        if (result.IsSuccess && result.Value!.Id.HasValue)
        {
            await InvalidatePatientCachesAsync(result.Value.Id.Value, HttpContext.RequestAborted);
        }
        return CreatedOrBadRequest(result, nameof(GetPatientById), new { id = result.Value?.Id });
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PatientDto>> UpdatePatient(Guid id, [FromBody] PatientDto patientDto)
    {
        if (id != patientDto.Id) return BadRequest("ID mismatch between route and body");
        var result = await Mediator.Send(new UpdatePatientCommand(patientDto));
        if (result.IsSuccess)
        {
            await InvalidatePatientCachesAsync(id, HttpContext.RequestAborted);
        }
        return UpdatedOrNotFound(result);
    }

    [HttpPut("upsert")]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PatientDto>> UpsertPatient([FromBody] PatientDto patientDto)
    {
        var result = await Mediator.Send(new UpsertPatientCommand(patientDto));
        if (result.IsSuccess && result.Value is not null && result.Value.Id.HasValue)
        {
            await InvalidatePatientCachesAsync(result.Value.Id.Value, HttpContext.RequestAborted);
        }
        return UpsertOkOrBadRequest(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeletePatient(Guid id)
    {
        var result = await Mediator.Send(new DeletePatientCommand(id));
        if (result.IsSuccess)
        {
            await InvalidatePatientCachesAsync(id, HttpContext.RequestAborted);
        }
        return NoContentOrNotFound(result);
    }

    private async Task InvalidatePatientCachesAsync(Guid id, CancellationToken token)
    {
        try
        {
            await _cacheStore.EvictByTagAsync("patients-all", token);
            await _cacheStore.EvictByTagAsync("patients-byid", token);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to evict output cache for patient {PatientId}", id);
        }
    }
}
