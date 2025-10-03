// StaffFormDto moved to Web.Admin project.
using CareSync.Domain.Enums;

namespace CareSync.Application.DTOs.Staff;

public record StaffDto(
    Guid Id,
    string FirstName,
    string LastName,
    string? MiddleName,
    string Email,
    string PhoneNumber,
    StaffRole Role,
    Department Department,
    string EmployeeId,
    DateTime HireDate,
    DateTime? TerminationDate,
    bool IsActive,
    decimal? Salary,
    string? Notes,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    bool HasRelatedData = false
);

public record CreateStaffDto(
    string FirstName,
    string LastName,
    string? MiddleName,
    string Email,
    string PhoneNumber,
    StaffRole Role,
    Department Department,
    string EmployeeId,
    DateTime HireDate,
    decimal? Salary,
    string? Notes
);

public record UpdateStaffDto(
    Guid Id,
    string FirstName,
    string LastName,
    string? MiddleName,
    string Email,
    string PhoneNumber,
    StaffRole Role,
    Department Department,
    decimal? Salary,
    string? Notes
);

public record UpsertStaffDto(
    Guid? Id,
    string FirstName,
    string LastName,
    string? MiddleName,
    string Email,
    string PhoneNumber,
    StaffRole Role,
    Department Department,
    string EmployeeId,
    DateTime HireDate,
    decimal? Salary,
    string? Notes
);

// StaffFormDto relocated to Web.Admin (see Forms/StaffFormDto.cs)
