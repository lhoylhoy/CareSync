using System.Globalization;
using CareSync.Shared.Interfaces;
using CareSync.Shared.Services;
using CareSync.Shared.Services.Contracts; // generic abstractions only
using CareSync.Web.Admin.Services.Contracts;
using CareSync.Web.Admin;
using CareSync.Web.Admin.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure Philippine culture for proper localization and encoding
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-PH");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-PH");

// Check if we're in development mode
var isDevelopment = builder.HostEnvironment.IsDevelopment();

if (isDevelopment)
{
    // For local development, use simple configuration without MSAL
    builder.Services.AddAuthorizationCore();
    builder.Services.AddScoped<AuthenticationStateProvider, LocalAuthenticationStateProvider>();

    // Configure HttpClient for local development
    builder.Services.AddHttpClient("ServerAPI", client =>
    {
        var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7262";
        client.BaseAddress = new Uri(apiBaseUrl);
        // For development, we might want to bypass SSL certificate validation
        // Note: Only use this in development!
    });
}
else
{
    // Configure MSAL for Entra ID authentication in production
    builder.Services.AddMsalAuthentication(options =>
    {
        builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);

        // Add the scope for accessing your API
        var apiScopes = builder.Configuration.GetSection("ApiSettings:Scopes").Get<string[]>();
        if (apiScopes != null)
            foreach (var scope in apiScopes)
                options.ProviderOptions.DefaultAccessTokenScopes.Add(scope);

        // Also add Microsoft Graph scope for user info
        options.ProviderOptions.DefaultAccessTokenScopes.Add("https://graph.microsoft.com/User.Read");
    });

    // Configure the authorization message handler for production
    builder.Services.AddScoped<AuthorizationMessageHandler>();

    // Configure HttpClient with authentication for production
    builder.Services.AddHttpClient("ServerAPI", client =>
        {
            var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7262";
            client.BaseAddress = new Uri(apiBaseUrl);

            // Disable all forms of caching
            client.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
            {
                NoCache = true,
                NoStore = true,
                MustRevalidate = true
            };
            client.DefaultRequestHeaders.Add("Pragma", "no-cache");
        })
        .AddHttpMessageHandler(sp =>
        {
            var handler = sp.GetRequiredService<AuthorizationMessageHandler>();
            var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7262";
            var apiScopes = builder.Configuration.GetSection("ApiSettings:Scopes").Get<string[]>() ??
                            Array.Empty<string>();

            handler.ConfigureHandler(
                new[] { apiBaseUrl },
                apiScopes);
            return handler;
        });
}

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient("ServerAPI"));

// Add your services
builder.Services.AddScoped<IBillingService, BillingService>();
builder.Services.AddScoped<IMedicalRecordService, MedicalRecordService>();
builder.Services.AddScoped<IToastService, ToastService>();
builder.Services.AddScoped<IValidationService, ValidationService>();

// Add geographic data service with HttpClient
builder.Services.AddHttpClient<IPhilippineGeographicDataService, CareSync.Web.Admin.Services.ApiGeographicDataService>(client =>
{
    // Reuse the configured API base URL (falls back to localhost if missing)
    var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7262";
    client.BaseAddress = new Uri(apiBaseUrl.EndsWith('/') ? apiBaseUrl : apiBaseUrl + "/");
    // Disable caching to always fetch fresh geographic data
    client.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
    {
        NoCache = true,
        NoStore = true,
        MustRevalidate = true
    };
    if (!client.DefaultRequestHeaders.Contains("Pragma"))
    {
        client.DefaultRequestHeaders.Add("Pragma", "no-cache");
    }
});

// Add CRUD services
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<ICrudService<CareSync.Application.DTOs.Staff.StaffDto, CareSync.Application.DTOs.Staff.CreateStaffDto, CareSync.Application.DTOs.Staff.UpdateStaffDto>, StaffService>();

// Add lazy loading support
builder.Services.AddScoped(typeof(Lazy<>), typeof(Lazy<>));

await builder.Build().RunAsync();
