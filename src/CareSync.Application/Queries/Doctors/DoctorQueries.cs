using CareSync.Application.Common.Results;
using CareSync.Application.DTOs.Doctors;
using MediatR;

namespace CareSync.Application.Queries.Doctors;

public record GetDoctorByIdQuery(Guid DoctorId) : IRequest<Result<DoctorDto>>;

public record GetAllDoctorsQuery : IRequest<Result<IEnumerable<DoctorDto>>>;
