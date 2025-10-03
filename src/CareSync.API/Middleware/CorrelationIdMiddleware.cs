using System.Diagnostics;

namespace CareSync.API.Middleware;

/// <summary>
/// Ensures every request has a stable Correlation ID available via HttpContext.TraceIdentifier,
/// adds/echoes the X-Correlation-ID header, and pushes it into the logging scope.
/// </summary>
public class CorrelationIdMiddleware
{
    private const string HeaderName = "X-Correlation-ID";
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        // Reuse incoming header if present, else use existing TraceIdentifier (or create one if empty)
        var incoming = context.Request.Headers.TryGetValue(HeaderName, out var values) ? values.ToString() : null;
        var correlationId = !string.IsNullOrWhiteSpace(incoming) ? incoming : context.TraceIdentifier;
        if (string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = Activity.Current?.Id ?? Guid.NewGuid().ToString("N");
            context.TraceIdentifier = correlationId;
        }

        // Set response header (always) so clients can log it.
        context.Response.Headers[HeaderName] = correlationId;

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId
        }))
        {
            await _next(context);
        }
    }
}

public static class CorrelationIdMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestCorrelation(this IApplicationBuilder app)
        => app.UseMiddleware<CorrelationIdMiddleware>();
}
