using CareSync.Application.Common.Results;
using CareSync.Application.DTOs.Patients;
using MediatR;

namespace CareSync.Application.Queries.Patients;

public record GetPatientByIdQuery(Guid PatientId) : IRequest<Result<PatientDto>>;

public record GetAllPatientsQuery : IRequest<Result<IEnumerable<PatientDto>>>;

public record GetPatientsPagedQuery(
    int Page,
    int PageSize,
    string? SearchTerm,
    IReadOnlyDictionary<string, string?> Filters) : IRequest<Result<CareSync.Application.Common.PagedResult<PatientDto>>>;
