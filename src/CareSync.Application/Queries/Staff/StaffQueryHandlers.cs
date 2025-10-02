using CareSync.Application.DTOs.Staff;
using CareSync.Application.Queries.Staff;
using CareSync.Application.Common.Mapping;
using CareSync.Application.Common.Results;
using CareSync.Domain.Enums;
using CareSync.Domain.Interfaces;
using MediatR;
// Reuse centralized StaffMapper rather than duplicating mapping logic here.

namespace CareSync.Application.QueryHandlers.Staff;

// Query handlers for Staff domain
public class GetAllStaffQueryHandler(IStaffRepository staffRepository, StaffMapper mapper)
    : IRequestHandler<GetAllStaffQuery, Result<IEnumerable<StaffDto>>>
{
    public async Task<Result<IEnumerable<StaffDto>>> Handle(GetAllStaffQuery request, CancellationToken cancellationToken)
    {
        var staff = await staffRepository.GetAllAsync();
        var list = new List<StaffDto>();
        foreach (var s in staff)
        {
            var dto = mapper.Map(s);
            bool hasRelated = await staffRepository.HasRelatedDataAsync(s.Id);
            dto = dto with { HasRelatedData = hasRelated };
            list.Add(dto);
        }
        return Result<IEnumerable<StaffDto>>.Success(list);
    }
}

public class GetStaffByIdQueryHandler(IStaffRepository staffRepository, StaffMapper mapper)
    : IRequestHandler<GetStaffByIdQuery, Result<StaffDto>>
{
    public async Task<Result<StaffDto>> Handle(GetStaffByIdQuery request, CancellationToken cancellationToken)
    {
        var staff = await staffRepository.GetByIdAsync(request.Id);
        if (staff == null)
            return Result<StaffDto>.Failure("Staff not found");
        var dto = mapper.Map(staff);
        bool hasRelated = await staffRepository.HasRelatedDataAsync(staff.Id);
        dto = dto with { HasRelatedData = hasRelated };
        return Result<StaffDto>.Success(dto);
    }
}

public class GetStaffByRoleQueryHandler(IStaffRepository staffRepository, StaffMapper mapper)
    : IRequestHandler<GetStaffByRoleQuery, Result<IEnumerable<StaffDto>>>
{
    public async Task<Result<IEnumerable<StaffDto>>> Handle(GetStaffByRoleQuery request, CancellationToken cancellationToken)
    {
        var staff = await staffRepository.GetByRoleAsync(request.Role);
        var list = new List<StaffDto>();
        foreach (var s in staff)
        {
            var dto = mapper.Map(s);
            bool hasRelated = await staffRepository.HasRelatedDataAsync(s.Id);
            dto = dto with { HasRelatedData = hasRelated };
            list.Add(dto);
        }
        return Result<IEnumerable<StaffDto>>.Success(list);
    }
}

public class GetStaffByDepartmentQueryHandler(IStaffRepository staffRepository, StaffMapper mapper)
    : IRequestHandler<GetStaffByDepartmentQuery, Result<IEnumerable<StaffDto>>>
{
    public async Task<Result<IEnumerable<StaffDto>>> Handle(GetStaffByDepartmentQuery request, CancellationToken cancellationToken)
    {
        var staff = await staffRepository.GetByDepartmentAsync(request.Department);
        var list = new List<StaffDto>();
        foreach (var s in staff)
        {
            var dto = mapper.Map(s);
            bool hasRelated = await staffRepository.HasRelatedDataAsync(s.Id);
            dto = dto with { HasRelatedData = hasRelated };
            list.Add(dto);
        }
        return Result<IEnumerable<StaffDto>>.Success(list);
    }
}
