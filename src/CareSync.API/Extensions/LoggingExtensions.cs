using Serilog;
using Serilog.Events;

namespace CareSync.API.Extensions;

public static class LoggingExtensions
{
    /// <summary>
    /// Configures Serilog with environment aware sinks.
    /// </summary>
    public static void ConfigureSerilog(this IHostBuilder host)
    {
        host.UseSerilog((ctx, services, lc) =>
        {
            var env = ctx.HostingEnvironment;
            lc.MinimumLevel.Information()
              .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
              .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
              .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
              .Enrich.FromLogContext()
              .Enrich.WithProperty("Application", "CareSync.API")
              .WriteTo.Console();

            if (!env.IsDevelopment())
            {
                lc.WriteTo.File("logs/caresync-.txt", rollingInterval: RollingInterval.Day);
            }
        });
    }
}
