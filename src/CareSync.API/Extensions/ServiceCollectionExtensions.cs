using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.RateLimiting;
using CareSync.API.Attributes;
using CareSync.API.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

namespace CareSync.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
    {
        services.AddLocalizationAndCulture();
        services.AddProblemDetails();
        services.AddApiControllers();
        services.AddSwaggerDocumentation(env);
        services.AddOptions(configuration);
        services.AddJwtOrEntraAuthentication(configuration, env);
        services.AddAuthorizationPolicies();
        services.AddCorsPolicies(env);
        services.AddRateLimiting();
        services.AddOutputCaching();
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.MimeTypes = new[]
            {
                "text/plain", "text/css", "application/javascript", "text/html", "application/xml", "text/xml",
                "application/json", "text/json"
            };
        });
        services.AddHealthChecks();
        return services;
    }

    private static IServiceCollection AddLocalizationAndCulture(this IServiceCollection services)
    {
        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[]
            {
                new CultureInfo("en-PH"),
                new CultureInfo("fil-PH")
            };
            options.DefaultRequestCulture = new RequestCulture("en-PH");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-PH");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-PH");
        });
        return services;
    }

    private static IServiceCollection AddApiControllers(this IServiceCollection services)
    {
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            options.JsonSerializerOptions.PropertyNamingPolicy = null; // preserve C# name (explicit DTO control)
            options.JsonSerializerOptions.WriteIndented = false;
            options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        });
        services.AddEndpointsApiExplorer();
        return services;
    }

    private static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services, IHostEnvironment env)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "CareSync API",
                Version = "v1",
                Description = env.IsDevelopment()
                    ? "Healthcare Management API (Development Mode - JWT Authentication)"
                    : "Healthcare Management API with Entra ID Authentication"
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    }, Array.Empty<string>()
                }
            });
        });
        return services;
    }

    private static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.Secret) && !string.IsNullOrWhiteSpace(o.ValidIssuer) && !string.IsNullOrWhiteSpace(o.ValidAudience),
                failureMessage: "JWT options are invalid or incomplete")
            .ValidateOnStart();

        services
            .AddOptions<AzureAdOptions>()
            .Bind(configuration.GetSection(AzureAdOptions.SectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.TenantId) && (!string.IsNullOrWhiteSpace(o.Audience) || !string.IsNullOrWhiteSpace(o.ClientId)),
                failureMessage: "AzureAd options are invalid or incomplete")
            .ValidateOnStart();

        return services;
    }

    private static IServiceCollection AddJwtOrEntraAuthentication(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                if (env.IsDevelopment())
                {
                    var jwt = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
                              ?? throw new InvalidOperationException("JWT section missing");
                    if (string.IsNullOrWhiteSpace(jwt.Secret))
                        throw new InvalidOperationException("JWT secret missing in configuration");
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwt.ValidIssuer,
                        ValidAudience = jwt.ValidAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret)),
                        ClockSkew = TimeSpan.FromMinutes(2)
                    };
                }
                else
                {
                    var azureAd = configuration.GetSection(AzureAdOptions.SectionName).Get<AzureAdOptions>()
                                 ?? throw new InvalidOperationException("AzureAd section missing");
                    var tenantId = azureAd.TenantId ?? throw new InvalidOperationException("AzureAd:TenantId missing");
                    var audience = azureAd.Audience ?? azureAd.ClientId ?? throw new InvalidOperationException("AzureAd Audience/ClientId missing");
                    options.Authority = $"https://login.microsoftonline.com/{tenantId}/v2.0";
                    options.Audience = audience;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = $"https://login.microsoftonline.com/{tenantId}/v2.0",
                        ValidAudiences = new[] { audience, $"api://{audience}" },
                        ClockSkew = TimeSpan.FromMinutes(2)
                    };
                }
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Log.Warning("JWT Authentication failed: {Exception}", context.Exception?.Message);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var principal = context.Principal;
                        var userId = env.IsDevelopment()
                            ? principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                            : principal?.FindFirst("oid")?.Value;
                        var userName = principal?.Identity?.Name ?? principal?.FindFirst("name")?.Value ?? "Unknown";
                        Log.Information("JWT Token validated for {UserName} ({UserId})", userName, userId);
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        Log.Warning("JWT Challenge: {Error} - {Description}", context.Error, context.ErrorDescription);
                        return Task.CompletedTask;
                    }
                };
            });
        return services;
    }

    private static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAuthenticatedUser", policy => policy.RequireAuthenticatedUser());
            options.AddPolicy("CustomOnly", policy => policy.Requirements.Add(new CustomAuthorizeRequirement()));
            options.AddPolicy("CanManageMedicalRecords", policy =>
                policy.RequireRole("Doctor", "Admin"));
            options.AddPolicy("CanManageAppointments", policy =>
                policy.RequireRole("Doctor", "Staff", "Admin"));
            options.AddPolicy("CanFinalizeMedicalRecord", policy =>
                policy.RequireRole("Doctor", "Admin"));
            options.AddPolicy("CanReopenMedicalRecord", policy =>
                policy.RequireRole("Admin"));
            options.AddPolicy("CanMoveAppointmentLifecycle", policy =>
                policy.RequireRole("Doctor", "Staff", "Admin"));
        });
        services.AddSingleton<IAuthorizationHandler, CustomAuthorizationHandler>();
        return services;
    }

    private static IServiceCollection AddCorsPolicies(this IServiceCollection services, IHostEnvironment env)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                if (env.IsDevelopment())
                {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                }
                else
                {
                    policy.WithOrigins("https://yourdomain.com", "https://caresync.yourdomain.com")
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                }
            });
        });
        return services;
    }

    private static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new System.Threading.RateLimiting.FixedWindowRateLimiterOptions
                {
                    PermitLimit = 100,
                    Window = TimeSpan.FromSeconds(10),
                    QueueLimit = 0,
                    QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst
                });
            });
        });
        return services;
    }

    private static IServiceCollection AddOutputCaching(this IServiceCollection services)
    {
        services.AddOutputCache(options =>
        {
            options.AddBasePolicy(builder => builder.Cache() // default: cache successful GET/HEAD
                .Expire(TimeSpan.FromSeconds(30))
                .SetVaryByQuery("page", "pageSize"));
            options.AddPolicy("Patients-All", b => b.Cache().Expire(TimeSpan.FromSeconds(15)).Tag("patients-all"));
            options.AddPolicy("Patients-ById", b => b.Cache().Expire(TimeSpan.FromMinutes(1)).Tag("patients-byid"));
            options.AddPolicy("Billing-All", b => b.Cache().Expire(TimeSpan.FromSeconds(15)).Tag("billing-all"));
            options.AddPolicy("Billing-ById", b => b.Cache().Expire(TimeSpan.FromMinutes(1)).Tag("billing-byid"));
            options.AddPolicy("Appointments-All", b => b.Cache().Expire(TimeSpan.FromSeconds(10)).Tag("appointments-all"));
            options.AddPolicy("Appointments-ById", b => b.Cache().Expire(TimeSpan.FromSeconds(30)).Tag("appointments-byid"));
            options.AddPolicy("Appointments-ByPatient", b => b.Cache()
                .Expire(TimeSpan.FromSeconds(10))
                .SetVaryByRouteValue("patientId")
                .Tag("appointments-bypatient"));
            options.AddPolicy("Appointments-ByDoctor", b => b.Cache()
                .Expire(TimeSpan.FromSeconds(10))
                .SetVaryByRouteValue("doctorId")
                .Tag("appointments-bydoctor"));
        });
        return services;
    }
}
