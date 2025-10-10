using System.ComponentModel.DataAnnotations;

namespace CareSync.Application.DTOs.Patients;

/// <summary>
/// Unified Patient DTO - Used for all operations (Create/Read/Update)
/// QUICK WIN #1: Consolidated from 4 separate DTOs into one flexible DTO
/// If Id is null → Create new patient
/// If Id has value → Update existing patient
/// </summary>
public record PatientDto(
    Guid? Id,
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
    DateTime? CreatedAt = null,
    DateTime? UpdatedAt = null,
    bool HasRelatedData = false
)
{
    /// <summary>Calculated age based on date of birth</summary>
    public int? Age => DateOfBirth.HasValue
        ? DateTime.Today.Year - DateOfBirth.Value.Year - (DateTime.Today.DayOfYear < DateOfBirth.Value.DayOfYear ? 1 : 0)
        : null;
    
    /// <summary>Full display name with proper formatting</summary>
    public string DisplayName => string.IsNullOrEmpty(MiddleName)
        ? $"{FirstName} {LastName}"
        : $"{FirstName} {MiddleName} {LastName}";
    
    /// <summary>Full address in Philippine format</summary>
    public string FullAddress
    {
        get
        {
            var parts = new List<string>();
            if (!string.IsNullOrEmpty(Street)) parts.Add(Street);
            parts.Add(BarangayName);
            parts.Add(CityName);
            parts.Add(ProvinceName);
            if (!string.IsNullOrEmpty(CityZipCode)) parts.Add(CityZipCode);
            return string.Join(", ", parts);
        }
    }
    
    /// <summary>Check if patient is a minor (under 18)</summary>
    public bool IsMinor => Age.HasValue && Age.Value < 18;
    
    /// <summary>Check if this is a new patient (not yet saved)</summary>
    public bool IsNew => Id == null || Id == Guid.Empty;
};

// LEGACY DTOs - Kept for backwards compatibility during transition
// TODO: Remove these after all references are updated to use PatientDto

[Obsolete("Use PatientDto instead. This will be removed in the next major version.")]
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

[Obsolete("Use PatientDto instead. This will be removed in the next major version.")]
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

[Obsolete("Use PatientDto instead. This will be removed in the next major version.")]
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
