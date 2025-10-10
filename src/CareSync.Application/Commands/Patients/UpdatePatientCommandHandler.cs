using CareSync.Application.Common.Geographics;
using CareSync.Application.Common.Mapping;
using CareSync.Application.Common.Results;
using CareSync.Application.DTOs.Patients;
using CareSync.Domain.Entities;
using CareSync.Domain.Interfaces;
using CareSync.Domain.ValueObjects;
using MediatR;

namespace CareSync.Application.Commands.Patients;

public class UpdatePatientCommandHandler(IPatientRepository patientRepository, IPhilippineGeographicDataService geographicService, PatientMapper mapper, IUnitOfWork uow)
    : IRequestHandler<UpdatePatientCommand, Result<PatientDto>>
{
    public async Task<Result<PatientDto>> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        if (!request.Patient.Id.HasValue || request.Patient.Id.Value == Guid.Empty)
            return Result<PatientDto>.Failure("Patient Id is required for update operations.");
            
        var existingPatient = await patientRepository.GetByIdAsync(request.Patient.Id.Value);
        if (existingPatient == null)
            return Result<PatientDto>.Failure($"Patient with ID {request.Patient.Id.Value} not found.");

        // Get geographic names from codes (align with create/upsert behavior)
        var province = await geographicService.GetProvinceByCodeAsync(request.Patient.ProvinceCode, cancellationToken);
        var city = await geographicService.GetCityByCodeAsync(request.Patient.CityCode, cancellationToken);
        var barangay = await geographicService.GetBarangayByCodeAsync(request.Patient.BarangayCode, cancellationToken);

        // Update existing patient using available methods
        existingPatient.UpdatePersonalInformation(
            new FullName(request.Patient.FirstName, request.Patient.LastName, request.Patient.MiddleName),
            request.Patient.DateOfBirth,
            request.Patient.Gender
        );

        existingPatient.UpdateContactInformation(
            request.Patient.Street,
            request.Patient.ProvinceCode,
            province?.Name ?? existingPatient.ProvinceName,
            request.Patient.CityCode,
            city?.Name ?? existingPatient.CityName,
            request.Patient.CityZipCode,
            request.Patient.BarangayCode,
            barangay?.Name ?? existingPatient.BarangayName,
            string.IsNullOrEmpty(request.Patient.PhoneNumber) ? null : new PhoneNumber(request.Patient.PhoneNumber),
            string.IsNullOrEmpty(request.Patient.Email) ? null : new Email(request.Patient.Email)
        );

        // Update optional fields
        if (!string.IsNullOrEmpty(request.Patient.PhilHealthNumber))
            existingPatient.SetPhilHealthNumber(request.Patient.PhilHealthNumber);

        if (!string.IsNullOrEmpty(request.Patient.SssNumber))
            existingPatient.SetSssNumber(request.Patient.SssNumber);

        if (!string.IsNullOrEmpty(request.Patient.Tin))
            existingPatient.SetTin(request.Patient.Tin);

        await patientRepository.UpdateAsync(existingPatient);
        await uow.SaveChangesAsync(cancellationToken);
        return Result<PatientDto>.Success(mapper.Map(existingPatient));
    }
}
