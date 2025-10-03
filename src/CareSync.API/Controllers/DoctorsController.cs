using System.Collections.Generic;
using CareSync.Application.Commands.Doctors;
using CareSync.Application.Common.Results;
using CareSync.Application.DTOs.Doctors;
using CareSync.Application.Queries.Doctors;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Logging;

namespace CareSync.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorsController(IMediator mediator, ILogger<DoctorsController> logger, IOutputCacheStore cacheStore) : BaseApiController(mediator)
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly ILogger<DoctorsController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IOutputCacheStore _cacheStore = cacheStore ?? throw new ArgumentNullException(nameof(cacheStore));

    [HttpGet]
    [OutputCache(PolicyName = "Doctors-All")]
    public async Task<IActionResult> GetAllDoctors(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 0,
        [FromQuery] string? search = null,
        [FromQuery] Dictionary<string, string?>? filters = null)
    {
        var hasQueryOverrides = pageSize > 0 || !string.IsNullOrWhiteSpace(search) || (filters?.Count > 0);

        if (!hasQueryOverrides)
        {
            var allResult = await _mediator.Send(new GetAllDoctorsQuery());
            var response = OkOrProblem(allResult);
            if (response.Result is IActionResult actionResult)
            {
                return actionResult;
            }

            return Ok(response.Value);
        }

        var effectivePageSize = pageSize > 0 ? pageSize : 25;
        var pagedResult = await _mediator.Send(new GetDoctorsPagedQuery(
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

    [HttpGet("{id:guid}")]
    [OutputCache(PolicyName = "Doctors-ById")]
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
        if (result.IsSuccess && result.Value is not null)
        {
            await InvalidateDoctorCachesAsync(result.Value.Id, HttpContext.RequestAborted);
        }
        return CreatedOrBadRequest(result, nameof(GetDoctorById), new { id = result.Value?.Id });
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(DoctorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DoctorDto>> UpdateDoctor(Guid id, [FromBody] UpdateDoctorDto updateDoctorDto)
    {
        if (id != updateDoctorDto.Id) return BadRequest("ID mismatch between route and body");
        var result = await _mediator.Send(new UpdateDoctorCommand(updateDoctorDto));
        if (result.IsSuccess)
        {
            await InvalidateDoctorCachesAsync(id, HttpContext.RequestAborted);
        }
        return UpdatedOrNotFound(result);
    }

    [HttpPut("upsert")]
    [ProducesResponseType(typeof(DoctorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DoctorDto>> UpsertDoctor([FromBody] UpsertDoctorDto upsertDoctorDto)
    {
        var result = await _mediator.Send(new UpsertDoctorCommand(upsertDoctorDto));
        if (result.IsSuccess && result.Value is not null)
        {
            await InvalidateDoctorCachesAsync(result.Value.Id, HttpContext.RequestAborted);
        }
        return UpdatedOrNotFound(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteDoctor(Guid id)
    {
        var result = await _mediator.Send(new DeleteDoctorCommand(id));
        if (result.IsSuccess)
        {
            await InvalidateDoctorCachesAsync(id, HttpContext.RequestAborted);
        }
        return NoContentOrNotFound(result);
    }

    private async Task InvalidateDoctorCachesAsync(Guid id, CancellationToken token)
    {
        try
        {
            await _cacheStore.EvictByTagAsync("doctors-all", token);
            await _cacheStore.EvictByTagAsync("doctors-byid", token);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to evict output cache for doctor {DoctorId}", id);
        }
    }
}
