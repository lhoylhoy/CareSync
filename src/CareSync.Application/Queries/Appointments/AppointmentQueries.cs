using CareSync.Application.DTOs.Appointments;
using CareSync.Application.Common.Results;
using MediatR;

namespace CareSync.Application.Queries.Appointments;

public record GetAllAppointmentsQuery : IRequest<Result<IEnumerable<AppointmentDto>>>;

public record GetAppointmentByIdQuery(Guid Id) : IRequest<Result<AppointmentDto>>;

public record GetAppointmentsByPatientQuery(Guid PatientId) : IRequest<Result<IEnumerable<AppointmentDto>>>;

public record GetAppointmentsByDoctorQuery(Guid DoctorId) : IRequest<Result<IEnumerable<AppointmentDto>>>;

public record GetAppointmentsByDateRangeQuery(DateTime StartDate, DateTime EndDate)
    : IRequest<Result<IEnumerable<AppointmentDto>>>;
