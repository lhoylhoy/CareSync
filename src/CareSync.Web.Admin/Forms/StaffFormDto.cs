namespace CareSync.Web.Admin.Forms;

using CareSync.Application.DTOs.Staff;
using CareSync.Domain.Enums;

public class StaffFormDto
{
    public Guid? Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public StaffRole Role { get; set; } = StaffRole.Nurse;
    public Department Department { get; set; } = Department.Administration;
    public string EmployeeId { get; set; } = string.Empty;
    public DateTime HireDate { get; set; } = DateTime.Today;
    public decimal? Salary { get; set; }
    public string? Notes { get; set; }

    public CreateStaffDto ToCreateDto() => new(FirstName, LastName, MiddleName, Email, PhoneNumber, Role, Department, EmployeeId, HireDate, Salary, Notes);
    public UpdateStaffDto ToUpdateDto() => new(Id!.Value, FirstName, LastName, MiddleName, Email, PhoneNumber, Role, Department, Salary, Notes);
    public UpsertStaffDto ToUpsertDto() => new(Id, FirstName, LastName, MiddleName, Email, PhoneNumber, Role, Department, EmployeeId, HireDate, Salary, Notes);
    public static StaffFormDto FromDto(StaffDto dto) => new() { Id = dto.Id, FirstName = dto.FirstName, LastName = dto.LastName, MiddleName = dto.MiddleName, Email = dto.Email, PhoneNumber = dto.PhoneNumber, Role = dto.Role, Department = dto.Department, EmployeeId = dto.EmployeeId, HireDate = dto.HireDate, Salary = dto.Salary, Notes = dto.Notes };
}
