namespace CareSync.Web.Admin.Forms;

using CareSync.Application.DTOs.Doctors;

public class DoctorFormDto
{
    public Guid? Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string Specialty { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public CreateDoctorDto ToCreateDto() => new(FirstName, LastName, MiddleName, Specialty, PhoneNumber, Email);
    public UpdateDoctorDto ToUpdateDto() => new(Id!.Value, FirstName, LastName, MiddleName, Specialty, PhoneNumber, Email);
    public UpsertDoctorDto ToUpsertDto() => new(Id, FirstName, LastName, MiddleName, Specialty, PhoneNumber, Email);

    public static DoctorFormDto FromDto(DoctorDto dto) => new()
    {
        Id = dto.Id,
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        MiddleName = dto.MiddleName,
        Specialty = dto.Specialty,
        PhoneNumber = dto.PhoneNumber,
        Email = dto.Email
    };
}
