using CareSync.Application.Commands.Patients;
using CareSync.Application.Queries.Patients;
using CareSync.Application.DTOs.Patients;
using MediatR;
using CareSync.Application.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.Mvc;

namespace CareSync.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController(IMediator mediator, ILogger<PatientsController> logger, IOutputCacheStore cacheStore) : BaseApiController(mediator)
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly ILogger<PatientsController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IOutputCacheStore _cacheStore = cacheStore ?? throw new ArgumentNullException(nameof(cacheStore));

    [HttpGet]
    [OutputCache(PolicyName = "Patients-All")]
    [ProducesResponseType(typeof(IEnumerable<PatientDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PatientDto>>> GetAllPatients()
    {
        var result = await _mediator.Send(new GetAllPatientsQuery());
        return OkOrProblem(result);
    }

    [HttpGet("{id:guid}")]
    [OutputCache(PolicyName = "Patients-ById")]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientDto>> GetPatientById(Guid id)
    {
        var result = await _mediator.Send(new GetPatientByIdQuery(id));
        return OkOrNotFound(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PatientDto>> CreatePatient([FromBody] CreatePatientDto createPatientDto)
    {
        var result = await _mediator.Send(new CreatePatientCommand(createPatientDto));
        if (result.IsSuccess)
        {
            await InvalidatePatientCachesAsync(result.Value!.Id, HttpContext.RequestAborted);
        }
        return CreatedOrBadRequest(result, nameof(GetPatientById), new { id = result.Value?.Id });
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PatientDto>> UpdatePatient(Guid id, [FromBody] UpdatePatientDto updatePatientDto)
    {
        if (id != updatePatientDto.Id) return BadRequest("ID mismatch between route and body");
        var result = await _mediator.Send(new UpdatePatientCommand(updatePatientDto));
        if (result.IsSuccess)
        {
            await InvalidatePatientCachesAsync(id, HttpContext.RequestAborted);
        }
        return UpdatedOrNotFound(result);
    }

    [HttpPut("upsert")]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PatientDto>> UpsertPatient([FromBody] UpsertPatientDto upsertPatientDto)
    {
        var result = await _mediator.Send(new UpsertPatientCommand(upsertPatientDto));
        if (result.IsSuccess && result.Value is not null)
        {
            await InvalidatePatientCachesAsync(result.Value.Id, HttpContext.RequestAborted);
        }
        return UpsertOkOrBadRequest(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeletePatient(Guid id)
    {
        var result = await _mediator.Send(new DeletePatientCommand(id));
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
