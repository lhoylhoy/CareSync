using CareSync.Application.DTOs.Staff;
using CareSync.Domain.Entities;
using CareSync.Domain.Enums;

namespace CareSync.Application.Common.Mapping;

public sealed class StaffMapper : IEntityMapper<Staff, StaffDto>
{
    public StaffDto Map(Staff staff) => new(
        staff.Id,
        staff.Name.FirstName,
        staff.Name.LastName,
        staff.Name.MiddleName,
        staff.Email.Value,
        staff.PhoneNumber.Number,
    staff.Role,
    staff.Department,
        staff.EmployeeId,
        staff.HireDate,
        staff.TerminationDate,
        staff.IsActive,
        staff.Salary,
        staff.Notes,
        DateTime.UtcNow,
        DateTime.UtcNow,
        false // HasRelatedData populated in query handlers after repository checks
    );
}

public static class StaffMapperExtensions
{
    public static StaffDto ToDto(this Staff staff, StaffMapper mapper) => mapper.Map(staff);
}// Placeholder removed: StaffMapper pending correct entity shape.
