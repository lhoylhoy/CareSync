using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CareSync.API.Middleware;

/// <summary>
///     Centralized exception handling middleware producing RFC 7807 ProblemDetails responses.
/// </summary>
public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, logger, env);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex, ILogger logger, IHostEnvironment env)
    {
        // Fast-path: if it's a validation exception, make sure we lock the status code to 400 *before* doing any other work
        if (ex is ValidationException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }

        var problem = CreateProblemDetails(context, ex, env, logger);

        // If something else along the way mutated status back to 200 (rare), force it to the problem status now.
        if (context.Response.StatusCode == StatusCodes.Status200OK && problem.Status.HasValue)
        {
            context.Response.StatusCode = problem.Status.Value;
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = problem.Status ?? context.Response.StatusCode;

        // Diagnostic (debug) log to help verify we are not leaking a 500 for validation errors
        if (ex is ValidationException)
        {
            logger.LogInformation("Validation ProblemDetails emitted with status {Status} for {Path}", context.Response.StatusCode, context.Request.Path);
        }

        var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = env.IsDevelopment()
        });
        await context.Response.WriteAsync(json);
    }

    private static Microsoft.AspNetCore.Mvc.ProblemDetails CreateProblemDetails(HttpContext context, Exception ex, IHostEnvironment env, ILogger logger)
    {
        string title;
        string type;
        string detail = env.IsDevelopment() ? ex.ToString() : ex.Message;
        int status;

        switch (ex)
        {
            case ValidationException validationException:
                status = StatusCodes.Status400BadRequest;
                title = "Validation failed";
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                // Always provide concise detail (omit stack / full exception wall of text)
                detail = "One or more validation errors occurred.";
                logger.LogWarning(ex, "Validation error processing {Path}", context.Request.Path);

                static string SimplifyKey(string propertyName)
                {
                    // Strip object prefixes like "Patient.FirstName" -> "FirstName"
                    var key = propertyName.Contains('.') ? propertyName.Split('.').Last() : propertyName;
                    // Lower camelCase for client-side consistency
                    if (key.Length > 1)
                        return char.ToLowerInvariant(key[0]) + key[1..];
                    return key.ToLowerInvariant();
                }

                var errorDict = validationException.Errors
                    .GroupBy(e => SimplifyKey(e.PropertyName))
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage
                                // Remove redundant leading parent object words e.g. "Patient " if present
                                .Replace("Patient ", string.Empty, StringComparison.OrdinalIgnoreCase)
                            )
                            .Distinct()
                            .ToArray());

                var vpd = new Microsoft.AspNetCore.Mvc.ValidationProblemDetails(errorDict)
                {
                    Title = title,
                    Detail = detail,
                    Status = status,
                    Type = type,
                    Instance = context.TraceIdentifier
                };
                vpd.Extensions["correlationId"] = context.TraceIdentifier;
                return vpd;
            case ArgumentException:
                status = StatusCodes.Status400BadRequest;
                title = "Invalid argument";
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                logger.LogWarning(ex, "Argument error processing {Path}", context.Request.Path);
                break;
            case KeyNotFoundException:
                status = StatusCodes.Status404NotFound;
                title = "Resource not found";
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
                logger.LogInformation(ex, "Not found: {Path}", context.Request.Path);
                break;
            case DbUpdateException dbEx:
                status = StatusCodes.Status409Conflict;
                title = "Persistence error";
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.8";
                logger.LogError(dbEx, "Database error during request {Path}", context.Request.Path);
                // Provide a friendlier detail if common constraints detected
                if (!env.IsDevelopment())
                {
                    if (dbEx.InnerException?.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) == true ||
                        dbEx.InnerException?.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        detail = "A conflicting resource already exists.";
                    }
                    else if (dbEx.InnerException?.Message.Contains("NULL", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        detail = "A required field was missing.";
                    }
                    else
                    {
                        detail = "A data persistence error occurred.";
                    }
                }
                break;
            default:
                status = StatusCodes.Status500InternalServerError;
                title = "Unexpected server error";
                type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
                logger.LogError(ex, "Unhandled exception for {Path}", context.Request.Path);
                if (!env.IsDevelopment()) detail = "An unexpected error occurred.";
                break;
        }

        var problem = new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Title = title,
            Detail = detail,
            Status = status,
            Type = type,
            Instance = context.TraceIdentifier
        };
        // Attach correlation id (same as TraceIdentifier) for client diagnostics
        problem.Extensions["correlationId"] = context.TraceIdentifier;
        return problem;
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionHandlingMiddleware>();
}
