using CareSync.Application.DTOs.Staff;
using CareSync.Application.Common.Results;
using MediatR;

namespace CareSync.Application.Commands.Staff;

public record CreateStaffCommand(CreateStaffDto Staff) : IRequest<Result<StaffDto>>;

public record UpdateStaffCommand(UpdateStaffDto Staff) : IRequest<Result<StaffDto>>;

public record UpsertStaffCommand(UpsertStaffDto Staff) : IRequest<Result<StaffDto>>;

public record DeleteStaffCommand(Guid StaffId) : IRequest<Result<Unit>>;
