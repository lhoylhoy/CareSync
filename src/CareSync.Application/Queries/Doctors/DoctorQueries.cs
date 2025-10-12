using CareSync.Application.Common.Results;
using CareSync.Application.DTOs.Doctors;
using MediatR;

namespace CareSync.Application.Queries.Doctors;

public record GetDoctorByIdQuery(Guid DoctorId) : IRequest<Result<DoctorDto>>;

public record GetAllDoctorsQuery : IRequest<Result<IEnumerable<DoctorDto>>>;

public record GetDoctorsPagedQuery(
    int Page,
    int PageSize,
    string? SearchTerm,
    IReadOnlyDictionary<string, string?> Filters) : IRequest<Result<CareSync.Application.Common.PagedResult<DoctorDto>>>;
