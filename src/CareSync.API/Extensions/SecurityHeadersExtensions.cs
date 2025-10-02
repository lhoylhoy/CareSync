namespace CareSync.API.Extensions;

/// <summary>
///     Security headers middleware extensions (modernized)
/// </summary>
public static class SecurityHeadersExtensions
{
    /// <summary>
    ///     Adds security headers to HTTP responses
    /// </summary>
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            var headers = context.Response.Headers;
            headers["X-Content-Type-Options"] = "nosniff";
            headers["X-Frame-Options"] = "DENY"; // legacy fallback for old browsers
            headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=(), accelerometer=(), gyroscope=(), magnetometer=()";
            // Basic CSP - adjust for Blazor / SignalR as needed
            if (!headers.ContainsKey("Content-Security-Policy"))
            {
                headers["Content-Security-Policy"] = "default-src 'self'; object-src 'none'; frame-ancestors 'none'; base-uri 'self'; form-action 'self';";
            }
            // HSTS added automatically by UseHsts in production; don't add duplicate here
            headers.Remove("Server");
            await next();
        });
    }
}
