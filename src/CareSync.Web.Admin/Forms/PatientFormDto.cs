using System.ComponentModel.DataAnnotations;
using CareSync.Application.DTOs.Patients;

namespace CareSync.Web.Admin.Forms;

public class PatientFormDto
{
    public Guid? Id { get; set; }
    [Required] public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    [Required] public string LastName { get; set; } = string.Empty;
    public string? Street { get; set; }
    [Required] public string ProvinceCode { get; set; } = string.Empty;
    [Required] public string CityCode { get; set; } = string.Empty;
    [Required] public string BarangayCode { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    [Required] public string Gender { get; set; } = string.Empty;
    [Phone] public string? PhoneNumber { get; set; }
    [EmailAddress] public string? Email { get; set; }
    public string? EmergencyContactName { get; set; }
    [Phone] public string? EmergencyContactNumber { get; set; }
    public string? PhilHealthNumber { get; set; }
    public string? SssNumber { get; set; }
    public string? Tin { get; set; }
    public string? ProvinceName { get; set; }
    public string? CityName { get; set; }
    public string? BarangayName { get; set; }
    [Required] public string? CityZipCode { get; set; }

    public CreatePatientDto ToCreateDto() => new(FirstName, MiddleName, LastName, Street, ProvinceCode, CityCode, BarangayCode,
        CityZipCode, DateOfBirth, Gender, PhoneNumber, Email, EmergencyContactName, EmergencyContactNumber,
        PhilHealthNumber, SssNumber, Tin);

    public UpdatePatientDto ToUpdateDto() => new(Id ?? Guid.Empty, FirstName, MiddleName, LastName, Street, ProvinceCode, CityCode,
        BarangayCode, CityZipCode, DateOfBirth, Gender, PhoneNumber, Email, EmergencyContactName,
        EmergencyContactNumber, PhilHealthNumber, SssNumber, Tin);

    public UpsertPatientDto ToUpsertDto() => new(null, FirstName, MiddleName, LastName, Street, ProvinceCode, CityCode, BarangayCode,
        CityZipCode, DateOfBirth, Gender, PhoneNumber, Email, EmergencyContactName, EmergencyContactNumber,
        PhilHealthNumber, SssNumber, Tin);

    public static PatientFormDto FromDto(PatientDto dto) => new()
    {
        Id = dto.Id,
        FirstName = dto.FirstName,
        MiddleName = dto.MiddleName,
        LastName = dto.LastName,
        Street = dto.Street,
        ProvinceCode = dto.ProvinceCode,
        ProvinceName = dto.ProvinceName,
        CityCode = dto.CityCode,
        CityName = dto.CityName,
        BarangayCode = dto.BarangayCode,
        BarangayName = dto.BarangayName,
        CityZipCode = dto.CityZipCode,
        DateOfBirth = dto.DateOfBirth,
        Gender = dto.Gender,
        PhoneNumber = dto.PhoneNumber,
        Email = dto.Email,
        EmergencyContactName = dto.EmergencyContactName,
        EmergencyContactNumber = dto.EmergencyContactNumber,
        PhilHealthNumber = dto.PhilHealthNumber,
        SssNumber = dto.SssNumber,
        Tin = dto.Tin
    };
}
