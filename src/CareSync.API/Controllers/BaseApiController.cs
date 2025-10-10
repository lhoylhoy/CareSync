using CareSync.Application.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CareSync.API.Controllers;

/// <summary>
/// QUICK WIN #6: Enhanced base controller with convenient helper methods
/// Provides consistent error responses and reduces boilerplate in controllers
/// </summary>
public abstract class BaseApiController(IMediator mediator) : ControllerBase
{
    protected IMediator Mediator { get; } = mediator;
    
    protected readonly IMediator _mediator = mediator; // Alias for consistency

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
    
    /// <summary>
    /// QUICK WIN #6: Additional helper methods
    /// </summary>
    
    /// <summary>
    /// Returns OK if result is success, BadRequest with error if failure
    /// </summary>
    protected ActionResult<T> OkOrBadRequest<T>(Result<T> result)
    {
        return result.IsSuccess && result.Value != null
            ? Ok(result.Value) 
            : BadRequest(new { error = result.Error });
    }
    
    /// <summary>
    /// Returns NoContent if result is success, BadRequest with error if failure
    /// </summary>
    protected ActionResult NoContentOrBadRequest(Result result)
    {
        return result.IsSuccess 
            ? NoContent() 
            : BadRequest(new { error = result.Error });
    }
    
    /// <summary>
    /// Returns a consistent error response with custom status code
    /// </summary>
    protected ActionResult Error(string message, int statusCode = 400)
    {
        return StatusCode(statusCode, new { error = message });
    }
    
    /// <summary>
    /// Returns a validation error response (400) with field errors
    /// </summary>
    protected ActionResult ValidationError(Dictionary<string, string[]> errors)
    {
        return BadRequest(new 
        { 
            error = "Validation failed",
            errors 
        });
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
