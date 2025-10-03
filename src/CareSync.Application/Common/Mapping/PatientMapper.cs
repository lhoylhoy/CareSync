using CareSync.Application.DTOs.Patients;
using CareSync.Domain.Entities;

namespace CareSync.Application.Common.Mapping;

public sealed class PatientMapper : IEntityMapper<Patient, PatientDto>
{
    public PatientDto Map(Patient patient) => new(
        patient.Id,
        patient.FullName.FirstName,
        patient.FullName.MiddleName,
        patient.FullName.LastName,
        patient.Street,
        patient.ProvinceCode,
        patient.ProvinceName,
        patient.CityCode,
        patient.CityName,
        patient.BarangayCode,
        patient.BarangayName,
        patient.CityZipCode,
        patient.DateOfBirth,
        patient.Gender,
        patient.PhoneNumber?.Number,
        patient.Email?.Value,
        patient.EmergencyContactName,
        patient.EmergencyContactNumber,
        patient.PhilHealthNumber,
        patient.SssNumber,
        patient.Tin,
        DateTime.UtcNow, // TODO: Replace with entity auditing fields when available
        DateTime.UtcNow
    );
}

public static class PatientMapperExtensions
{
    public static PatientDto ToDto(this Patient patient, PatientMapper mapper) => mapper.Map(patient);
}
