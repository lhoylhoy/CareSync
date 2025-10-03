using CareSync.Application.Common.Geographics;
using CareSync.Domain.Interfaces;
using CareSync.Infrastructure.Data;
using CareSync.Infrastructure.Repositories;
using CareSync.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CareSync.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            services.AddDbContextPool<CareSyncDbContext>(options =>
                options.UseInMemoryDatabase("CareSyncV2"));
        }
        else
        {
            services.AddDbContextPool<CareSyncDbContext>(options =>
                options.UseSqlServer(connectionString, b =>
                    b.MigrationsAssembly(typeof(CareSyncDbContext).Assembly.FullName)));
        }

        // Register repositories
        services.AddScoped<IDoctorRepository, DoctorRepository>();
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
        services.AddScoped<IBillRepository, BillRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IInsuranceClaimRepository, InsuranceClaimRepository>();
        services.AddScoped<IStaffRepository, StaffRepository>();

        // Register external data services with proper HTTP client configuration
        services.AddHttpClient<IPhilippineGeographicDataService, PhilippineGeographicDataService>(client =>
        {
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Accept-Charset", "utf-8");
            client.DefaultRequestHeaders.Add("User-Agent", "CareSync/1.0");
        }).SetHandlerLifetime(TimeSpan.FromMinutes(5)); // short pool for resiliency

        // Unit of work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDomainEventDispatcher, Services.DomainEventDispatcher>();

        // Domain events dispatcher (noop placeholder)
        services.AddScoped<IDomainEventDispatcher, Infrastructure.Services.DomainEventDispatcher>();

        return services;
    }
}
