using CareSync.API.Extensions;
using CareSync.Application;
using CareSync.Infrastructure;
using Serilog;

// Bootstrap logger (will be replaced by full configuration later)
Log.Logger = new LoggerConfiguration()
    .WriteTo.Async(config => config.Console())
    .Enrich.FromLogContext()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting CareSync API");

    var builder = WebApplication.CreateBuilder(args);

    // Structured logging configuration
    builder.Host.ConfigureSerilog();

    // Add core application & infrastructure + presentation layer
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddPresentation(builder.Configuration, builder.Environment);

    var app = builder.Build();

    // Database reset for development (uncomment when needed)
    // if (app.Environment.IsDevelopment())
    // {
    //     using var scope = app.Services.CreateScope();
    //     var dbContext = scope.ServiceProvider.GetRequiredService<CareSyncDbContext>();
    //
    //     Log.Information("Resetting database for development...");
    //     await dbContext.Database.EnsureDeletedAsync();
    //     await dbContext.Database.EnsureCreatedAsync();
    //     Log.Information("Database reset completed");
    // }

    // Configure request pipeline and endpoints
    app
        .UseCareSyncPipeline()
        .MapCareSyncEndpoints();

    Log.Information("CareSync API started successfully");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "CareSync API terminated unexpectedly");
    throw;
}
finally
{
    await Log.CloseAndFlushAsync();
}
