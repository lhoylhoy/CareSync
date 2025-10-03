using CareSync.Application.Common.Results;
using CareSync.Application.DTOs.Patients;
using MediatR;

namespace CareSync.Application.Queries.Patients;

public record GetPatientByIdQuery(Guid PatientId) : IRequest<Result<PatientDto>>;

public record GetAllPatientsQuery : IRequest<Result<IEnumerable<PatientDto>>>;
