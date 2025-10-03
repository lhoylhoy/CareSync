using CareSync.Application.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CareSync.API.Controllers;

public abstract class BaseApiController(IMediator mediator) : ControllerBase
{
    protected IMediator Mediator { get; } = mediator;

    protected ActionResult<IEnumerable<T>> OkOrProblem<T>(Result<IEnumerable<T>> result)
    {
        if (!result.IsSuccess || result.Value is null)
            return ProblemFrom(result);
        return Ok(result.Value);
    }

    protected ActionResult<IEnumerable<T>> OkOrProblem<T>(Result<List<T>> result)
    {
        if (!result.IsSuccess || result.Value is null)
            return ProblemFrom(result);
        return Ok(result.Value);
    }

    protected ActionResult<T> OkOrProblem<T>(Result<T> result)
    {
        if (!result.IsSuccess || result.Value is null)
            return ProblemFrom(result);
        return Ok(result.Value);
    }

    protected ActionResult<T> OkOrNotFound<T>(Result<T> result)
    {
        if (!result.IsSuccess || result.Value is null)
            return NotFound(result.Error);
        return Ok(result.Value);
    }

    protected ActionResult CreatedOrBadRequest<T>(Result<T> result, string actionName, object routeValues)
    {
        if (!result.IsSuccess || result.Value is null)
            return ProblemFrom(result);
        return CreatedAtAction(actionName, routeValues, result.Value);
    }

    protected ActionResult UpdatedOrNotFound<T>(Result<T> result)
    {
        if (!result.IsSuccess || result.Value is null)
            return NotFound(result.Error);
        return Ok(result.Value);
    }

    protected ActionResult UpsertOkOrBadRequest<T>(Result<T> result)
    {
        if (!result.IsSuccess || result.Value is null)
            return ProblemFrom(result);
        return Ok(result.Value);
    }

    protected ActionResult NoContentOrNotFound(Result<Unit> result)
    {
        if (!result.IsSuccess)
            return NotFound(result.Error);
        return NoContent();
    }

    // Overload to support non-generic Result (for delete commands returning Result)
    protected ActionResult NoContentOrNotFound(Result result)
    {
        if (!result.IsSuccess)
            return NotFound(result.Error);
        return NoContent();
    }

    private ActionResult ProblemFrom<T>(Result<T> result)
    {
        var error = result.Error ?? "Unknown error";
        var status = MapStatusCode(error);
        return Problem(title: error, statusCode: status);
    }

    private static int MapStatusCode(string error)
    {
        if (string.IsNullOrWhiteSpace(error)) return StatusCodes.Status400BadRequest;
        var e = error.ToLowerInvariant();
        if (e.Contains("not found")) return StatusCodes.Status404NotFound;
        if (e.Contains("already exists") || e.Contains("conflict")) return StatusCodes.Status409Conflict;
        if (e.Contains("invalid") || e.Contains("required") || e.Contains("cannot")) return StatusCodes.Status400BadRequest;
        return StatusCodes.Status400BadRequest; // default client error
    }
}
