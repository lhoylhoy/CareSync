using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using CareSync.Application.Common.Behaviors;

namespace CareSync.Application;

/// <summary>
///     Extension methods for registering Application layer services
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    ///     Adds Application layer services to the dependency injection container
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        // Pipeline behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ResultWrappingBehavior<,>));

        // Mappers
        services.AddTransient<Common.Mapping.PatientMapper>();
        services.AddTransient<Common.Mapping.DoctorMapper>();
        services.AddTransient<Common.Mapping.AppointmentMapper>();
        services.AddTransient<Common.Mapping.BillMapper>();
        services.AddTransient<Common.Mapping.MedicalRecordMapper>();
        services.AddTransient<Common.Mapping.StaffMapper>();

        return services;
    }
}
