// DoctorFormDto moved to Web.Admin project.
namespace CareSync.Application.DTOs.Doctors;

public record DoctorDto(
    Guid Id,
    string FirstName,
    string LastName,
    string? MiddleName,
    string DisplayName,
    string Specialty,
    string PhoneNumber,
    string Email,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    bool HasRelatedData = false
);

public record CreateDoctorDto(
    string FirstName,
    string LastName,
    string? MiddleName,
    string Specialty,
    string PhoneNumber,
    string Email
);

public record UpdateDoctorDto(
    Guid Id,
    string FirstName,
    string LastName,
    string? MiddleName,
    string Specialty,
    string PhoneNumber,
    string Email
);

public record UpsertDoctorDto(
    Guid? Id,
    string FirstName,
    string LastName,
    string? MiddleName,
    string Specialty,
    string PhoneNumber,
    string Email
);

// UI form model - candidate to relocate to Web project later
// DoctorFormDto relocated to Web.Admin (see Forms/DoctorFormDto.cs)
