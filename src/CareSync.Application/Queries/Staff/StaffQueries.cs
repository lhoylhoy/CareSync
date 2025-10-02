using CareSync.Application.DTOs.Staff;
using CareSync.Application.Common.Results;
using MediatR;
using CareSync.Domain.Enums;

namespace CareSync.Application.Queries.Staff;

public record GetAllStaffQuery : IRequest<Result<IEnumerable<StaffDto>>>;

public record GetStaffByIdQuery(Guid Id) : IRequest<Result<StaffDto>>;

public record GetStaffByRoleQuery(StaffRole Role) : IRequest<Result<IEnumerable<StaffDto>>>; // Now strongly typed

public record GetStaffByDepartmentQuery(Department Department) : IRequest<Result<IEnumerable<StaffDto>>>;
