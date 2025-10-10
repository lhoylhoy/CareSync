using CareSync.API.Middleware;
using Serilog;

namespace CareSync.API.Extensions;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Configures middleware pipeline (no endpoint mapping here) and returns the same app for chaining.
    /// </summary>
    public static WebApplication UseCareSyncPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CareSync API v1");
                c.RoutePrefix = string.Empty;
                c.OAuthClientId(app.Configuration["AzureAd:ClientId"]);
                c.OAuthUsePkce();
            });
        }
        else
        {
            app.UseHsts();
        }

        // Correlation must be very early so later middleware/logging enriched
        app.UseRequestCorrelation();

        // Log AFTER correlation but BEFORE exception handling so Serilog sees final status codes (exception middleware will handle and then Serilog logs 400 not 500)
        app.UseSerilogRequestLogging(opts =>
        {
            opts.GetLevel = (ctx, elapsed, ex) =>
            {
                if (ex != null) return Serilog.Events.LogEventLevel.Error;
                var status = ctx.Response.StatusCode;
                if (status >= 500) return Serilog.Events.LogEventLevel.Error;
                if (status >= 400) return Serilog.Events.LogEventLevel.Warning; // 4xx are client issues
                return Serilog.Events.LogEventLevel.Information;
            };
        });

        // Now handle exceptions for everything after this point
        app.UseGlobalExceptionHandling();

        app.UseHttpsRedirection();
        app.UseResponseCompression();
        app.UseRequestLocalization();
        app.UseSecurityHeaders();
        app.UseRateLimiter();
        app.UseCors();
        app.UseOutputCache();
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }

    /// <summary>
    /// Maps API endpoints (split for clarity and testability).
    /// </summary>
    public static WebApplication MapCareSyncEndpoints(this WebApplication app)
    {
        app.MapControllers();
        
        // QUICK WIN #7: Enhanced health check endpoint with detailed JSON response
        app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var result = System.Text.Json.JsonSerializer.Serialize(new
                {
                    status = report.Status.ToString(),
                    timestamp = DateTime.UtcNow,
                    checks = report.Entries.Select(e => new
                    {
                        name = e.Key,
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description,
                        duration = e.Value.Duration.ToString(),
                        error = e.Value.Exception?.Message
                    }),
                    totalDuration = report.TotalDuration.ToString()
                });
                await context.Response.WriteAsync(result);
            }
        });
        
        return app;
    }
}
