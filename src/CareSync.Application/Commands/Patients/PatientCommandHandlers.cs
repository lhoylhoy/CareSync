using CareSync.Application.Common.Geographics;
using CareSync.Application.Common.Mapping;
using CareSync.Application.Common.Results;
using CareSync.Application.DTOs.Patients;
using CareSync.Domain.Entities;
using CareSync.Domain.Interfaces;
using CareSync.Domain.ValueObjects;
using MediatR;

namespace CareSync.Application.Commands.Patients;

public class CreatePatientCommandHandler(
    IPatientRepository patientRepository,
    IPhilippineGeographicDataService geographicService,
    PatientMapper mapper,
    IUnitOfWork uow)
    : IRequestHandler<CreatePatientCommand, Result<PatientDto>>
{
    public async Task<Result<PatientDto>> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        // Get geographic names from codes
        var province = await geographicService.GetProvinceByCodeAsync(request.Patient.ProvinceCode, cancellationToken);
        var city = await geographicService.GetCityByCodeAsync(request.Patient.CityCode, cancellationToken);
        var barangay = await geographicService.GetBarangayByCodeAsync(request.Patient.BarangayCode, cancellationToken);

        var patient = new Patient(
            new FullName(request.Patient.FirstName, request.Patient.LastName, request.Patient.MiddleName),
            string.IsNullOrEmpty(request.Patient.Email) ? null : new Email(request.Patient.Email),
            string.IsNullOrEmpty(request.Patient.PhoneNumber) ? null : new PhoneNumber(request.Patient.PhoneNumber),
            request.Patient.DateOfBirth,
            request.Patient.Gender,
            request.Patient.Street,
            request.Patient.ProvinceCode,
            province?.Name ?? "Unknown",
            request.Patient.CityCode,
            city?.Name ?? "Unknown",
            request.Patient.CityZipCode,
            request.Patient.BarangayCode,
            barangay?.Name ?? "Unknown"
        );

        // Set optional fields
        if (!string.IsNullOrEmpty(request.Patient.PhilHealthNumber))
            patient.SetPhilHealthNumber(request.Patient.PhilHealthNumber);

        if (!string.IsNullOrEmpty(request.Patient.SssNumber))
            patient.SetSssNumber(request.Patient.SssNumber);

        if (!string.IsNullOrEmpty(request.Patient.Tin))
            patient.SetTin(request.Patient.Tin);

        await patientRepository.AddAsync(patient);
        await uow.SaveChangesAsync(cancellationToken);

        return Result<PatientDto>.Success(mapper.Map(patient));
    }
}

public class UpsertPatientCommandHandler(
    IPatientRepository patientRepository,
    IPhilippineGeographicDataService geographicService,
    PatientMapper mapper,
    IUnitOfWork uow)
    : IRequestHandler<UpsertPatientCommand, Result<PatientDto>>
{
    public async Task<Result<PatientDto>> Handle(UpsertPatientCommand request, CancellationToken cancellationToken)
    {
        Patient patient;

        if (request.Patient.Id.HasValue && request.Patient.Id != Guid.Empty)
        {
            // ID provided - this is an UPDATE operation
            var existingPatient = await patientRepository.GetByIdAsync(request.Patient.Id.Value);

            if (existingPatient == null)
                return Result<PatientDto>.Failure($"Patient with ID {request.Patient.Id.Value} not found.");

            // Get geographic names from codes
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
                province?.Name ?? "Unknown",
                request.Patient.CityCode,
                city?.Name ?? "Unknown",
                request.Patient.CityZipCode,
                request.Patient.BarangayCode,
                barangay?.Name ?? "Unknown",
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
            patient = existingPatient;
        }
        else
        {
            // No ID provided - this is an INSERT operation
            // Get geographic names from codes
            var province = await geographicService.GetProvinceByCodeAsync(request.Patient.ProvinceCode, cancellationToken);
            var city = await geographicService.GetCityByCodeAsync(request.Patient.CityCode, cancellationToken);
            var barangay = await geographicService.GetBarangayByCodeAsync(request.Patient.BarangayCode, cancellationToken);

            patient = new Patient(
                new FullName(request.Patient.FirstName, request.Patient.LastName, request.Patient.MiddleName),
                string.IsNullOrEmpty(request.Patient.Email) ? null : new Email(request.Patient.Email),
                string.IsNullOrEmpty(request.Patient.PhoneNumber) ? null : new PhoneNumber(request.Patient.PhoneNumber),
                request.Patient.DateOfBirth,
                request.Patient.Gender,
                request.Patient.Street,
                request.Patient.ProvinceCode,
                province?.Name ?? "Unknown",
                request.Patient.CityCode,
                city?.Name ?? "Unknown",
                request.Patient.CityZipCode,
                request.Patient.BarangayCode,
                barangay?.Name ?? "Unknown"
            );

            // Set optional fields
            if (!string.IsNullOrEmpty(request.Patient.PhilHealthNumber))
                patient.SetPhilHealthNumber(request.Patient.PhilHealthNumber);

            if (!string.IsNullOrEmpty(request.Patient.SssNumber))
                patient.SetSssNumber(request.Patient.SssNumber);

            if (!string.IsNullOrEmpty(request.Patient.Tin))
                patient.SetTin(request.Patient.Tin);

            await patientRepository.AddAsync(patient);
        }

        await uow.SaveChangesAsync(cancellationToken);

        return Result<PatientDto>.Success(mapper.Map(patient));
    }
}

public class DeletePatientCommandHandler(IPatientRepository patientRepository, IUnitOfWork uow) : IRequestHandler<DeletePatientCommand, Result>
{
    public async Task<Result> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
    {
        var existing = await patientRepository.GetByIdAsync(request.PatientId);
        if (existing == null)
            return Result.Failure("Patient not found");
        if (await patientRepository.HasRelatedDataAsync(request.PatientId))
            return Result.Failure("Cannot delete patient because related appointments, medical records, or bills exist. Archive instead.");
        await patientRepository.DeleteAsync(request.PatientId);
        await uow.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
