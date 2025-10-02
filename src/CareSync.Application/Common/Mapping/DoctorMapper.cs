using CareSync.Application.DTOs.Doctors;
using CareSync.Domain.Entities;

namespace CareSync.Application.Common.Mapping;

public sealed class DoctorMapper : IEntityMapper<Doctor, DoctorDto>
{
    public DoctorDto Map(Doctor doctor) => new(
        doctor.Id,
        doctor.Name.FirstName,
        doctor.Name.LastName,
        doctor.Name.MiddleName,
        doctor.DisplayName,
        doctor.Specialty,
        doctor.PhoneNumber.Number,
        doctor.Email.Value,
        doctor.IsActive,
        DateTime.UtcNow,
        DateTime.UtcNow
    );
}
