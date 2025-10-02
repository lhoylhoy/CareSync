using CareSync.Application.DTOs.Appointments;
using CareSync.Application.Common.Results;
using MediatR;

namespace CareSync.Application.Commands.Appointments;

public record CreateAppointmentCommand(CreateAppointmentDto Appointment) : IRequest<Result<AppointmentDto>>;

public record UpdateAppointmentCommand(UpdateAppointmentDto Appointment) : IRequest<Result<AppointmentDto>>;

public record UpsertAppointmentCommand(UpsertAppointmentDto Appointment) : IRequest<Result<AppointmentDto>>;

public record CancelAppointmentCommand(CancelAppointmentDto Appointment) : IRequest<Result<AppointmentDto>>;

public record CompleteAppointmentCommand(Guid AppointmentId, string? Notes) : IRequest<Result<AppointmentDto>>;

public record StartAppointmentCommand(Guid AppointmentId) : IRequest<Result<AppointmentDto>>;

public record MarkNoShowAppointmentCommand(Guid AppointmentId) : IRequest<Result<AppointmentDto>>;

public record DeleteAppointmentCommand(Guid AppointmentId) : IRequest<Result<Unit>>;
