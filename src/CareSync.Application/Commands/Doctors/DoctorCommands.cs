using CareSync.Application.DTOs.Doctors;
using CareSync.Application.Common.Results;
using MediatR;

namespace CareSync.Application.Commands.Doctors;

public record CreateDoctorCommand(CreateDoctorDto Doctor) : IRequest<Result<DoctorDto>>;

public record UpdateDoctorCommand(UpdateDoctorDto Doctor) : IRequest<Result<DoctorDto>>;

public record UpsertDoctorCommand(UpsertDoctorDto Doctor) : IRequest<Result<DoctorDto>>;

public record DeleteDoctorCommand(Guid DoctorId) : IRequest<Result<Unit>>;
