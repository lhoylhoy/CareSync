// AppointmentFormDto moved to Web.Admin project.
using CareSync.Domain.Enums;

namespace CareSync.Application.DTOs.Appointments;

public record AppointmentDto(
    Guid Id,
    Guid PatientId,
    Guid DoctorId,
    string PatientName,
    string DoctorName,
    DateTime StartDateTime,
    TimeSpan Duration,
    AppointmentStatus Status,
    string? Notes,
    string? CancellationReason,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    bool HasRelatedData = false
);

public record CreateAppointmentDto(
    Guid PatientId,
    Guid DoctorId,
    DateTime StartDateTime,
    TimeSpan? Duration
);

public record UpdateAppointmentDto(
    Guid Id,
    DateTime StartDateTime,
    TimeSpan Duration,
    string? Notes
);

public record CancelAppointmentDto(
    Guid Id,
    string Reason
);

public record UpsertAppointmentDto(
    Guid? Id,
    Guid PatientId,
    Guid DoctorId,
    DateTime StartDateTime,
    int DurationMinutes,
    string AppointmentType,
    string? Notes,
    string? ReasonForVisit
);

// UI form model - candidate to relocate to Web layer.
// AppointmentFormDto relocated to Web.Admin (see Forms/AppointmentFormDto.cs)
