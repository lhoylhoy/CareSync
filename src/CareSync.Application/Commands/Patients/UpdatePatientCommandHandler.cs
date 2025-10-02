using CareSync.Domain.Entities;
using CareSync.Domain.Interfaces;
using CareSync.Domain.ValueObjects;
using CareSync.Application.DTOs.Patients;
using CareSync.Application.Common.Mapping;
using MediatR;
using CareSync.Application.Common.Results;

namespace CareSync.Application.Commands.Patients;

public class UpdatePatientCommandHandler(IPatientRepository patientRepository, PatientMapper mapper, IUnitOfWork uow)
    : IRequestHandler<UpdatePatientCommand, Result<PatientDto>>
{
    public async Task<Result<PatientDto>> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        var existingPatient = await patientRepository.GetByIdAsync(request.Patient.Id);
        if (existingPatient == null)
            return Result<PatientDto>.Failure($"Patient with ID {request.Patient.Id} not found.");

        // Update existing patient using available methods
        existingPatient.UpdatePersonalInformation(
            new FullName(request.Patient.FirstName, request.Patient.LastName, request.Patient.MiddleName),
            request.Patient.DateOfBirth,
            request.Patient.Gender
        );

        existingPatient.UpdateContactInformation(
            request.Patient.Street,
            request.Patient.ProvinceCode,
            "Unknown", // Province name - will be populated by service
            request.Patient.CityCode,
            "Unknown", // City name - will be populated by service
            request.Patient.CityZipCode,
            request.Patient.BarangayCode,
            "Unknown", // Barangay name - will be populated by service
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
