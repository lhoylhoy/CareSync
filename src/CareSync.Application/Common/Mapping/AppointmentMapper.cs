using CareSync.Application.DTOs.Appointments;
using CareSync.Domain.Entities;

namespace CareSync.Application.Common.Mapping;

public sealed class AppointmentMapper : IEntityMapper<Appointment, AppointmentDto>
{
    public AppointmentDto Map(Appointment a) => new(
        a.Id,
        a.PatientId,
        a.DoctorId,
        a.Patient?.FullName?.ToString() ?? string.Empty,
        a.Doctor?.DisplayName ?? string.Empty,
    a.ScheduledDate,
        a.Duration,
        a.Status,
        a.Notes,
        a.CancellationReason,
        DateTime.UtcNow, // TODO replace with auditing
        DateTime.UtcNow,
        false // HasRelatedData will be populated in query handlers
    );
}

public static class AppointmentMapperExtensions
{
    public static AppointmentDto ToDto(this Appointment a, AppointmentMapper mapper) => mapper.Map(a);
}// Placeholder removed: AppointmentMapper pending domain/entity audit.
