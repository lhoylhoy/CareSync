using System.ComponentModel.DataAnnotations;

namespace CareSync.Application.DTOs.Patients;

public record PatientDto(
    Guid Id,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? Street,
    string ProvinceCode,
    string ProvinceName,
    string CityCode,
    string CityName,
    string BarangayCode,
    string BarangayName,
    string? CityZipCode,
    DateTime? DateOfBirth,
    string Gender,
    string? PhoneNumber,
    string? Email,
    string? EmergencyContactName,
    string? EmergencyContactNumber,
    string? PhilHealthNumber,
    string? SssNumber,
    string? Tin,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    bool HasRelatedData = false
)
{
    public int? Age => DateOfBirth.HasValue
        ? DateTime.Today.Year - DateOfBirth.Value.Year - (DateTime.Today.DayOfYear < DateOfBirth.Value.DayOfYear ? 1 : 0)
        : null;
    public string DisplayName => string.IsNullOrEmpty(MiddleName)
        ? $"{FirstName} {LastName}"
        : $"{FirstName} {MiddleName} {LastName}";
};

public record CreatePatientDto(
    string FirstName,
    string? MiddleName,
    string LastName,
    string? Street,
    string ProvinceCode,
    string CityCode,
    string BarangayCode,
    string? CityZipCode,
    DateTime? DateOfBirth,
    string Gender,
    string? PhoneNumber,
    string? Email,
    string? EmergencyContactName,
    string? EmergencyContactNumber,
    string? PhilHealthNumber,
    string? SssNumber,
    string? Tin
);

public record UpdatePatientDto(
    Guid Id,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? Street,
    string ProvinceCode,
    string CityCode,
    string BarangayCode,
    string? CityZipCode,
    DateTime? DateOfBirth,
    string Gender,
    string? PhoneNumber,
    string? Email,
    string? EmergencyContactName,
    string? EmergencyContactNumber,
    string? PhilHealthNumber,
    string? SssNumber,
    string? Tin
);

public record UpsertPatientDto(
    Guid? Id,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? Street,
    string ProvinceCode,
    string CityCode,
    string BarangayCode,
    string? CityZipCode,
    DateTime? DateOfBirth,
    string Gender,
    string? PhoneNumber,
    string? Email,
    string? EmergencyContactName,
    string? EmergencyContactNumber,
    string? PhilHealthNumber,
    string? SssNumber,
    string? Tin
);

// Form DTO used primarily by UI layer (Blazor) - consider relocating to Web project if purely presentational.
// PatientFormDto moved to Web.Admin (presentation layer)
