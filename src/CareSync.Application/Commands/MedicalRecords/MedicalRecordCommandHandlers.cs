using CareSync.Domain.Entities;
using CareSync.Domain.Interfaces;
using CareSync.Application.DTOs.MedicalRecords;
using MediatR;
using CareSync.Application.Common.Mapping;
using CareSync.Application.Common.Results;

namespace CareSync.Application.Commands.MedicalRecords;

public class CreateMedicalRecordCommandHandler(
    IMedicalRecordRepository medicalRecordRepository,
    IPatientRepository patientRepository,
    IDoctorRepository doctorRepository,
    MedicalRecordMapper mapper,
    IUnitOfWork uow)
    : IRequestHandler<CreateMedicalRecordCommand, Result<MedicalRecordDto>>
{
    public async Task<Result<MedicalRecordDto>> Handle(CreateMedicalRecordCommand request, CancellationToken cancellationToken)
    {
        // Validate that patient exists
        var patient = await patientRepository.GetByIdAsync(request.MedicalRecord.PatientId);
        if (patient == null)
            return Result<MedicalRecordDto>.Failure($"Patient with ID {request.MedicalRecord.PatientId} does not exist.");

        // Validate that doctor exists
        var doctor = await doctorRepository.GetByIdAsync(request.MedicalRecord.DoctorId);
        if (doctor == null)
            return Result<MedicalRecordDto>.Failure($"Doctor with ID {request.MedicalRecord.DoctorId} does not exist.");

        var medicalRecord = new MedicalRecord(
            request.MedicalRecord.PatientId,
            request.MedicalRecord.DoctorId,
            request.MedicalRecord.ChiefComplaint,
            null, // AppointmentId - not provided in DTO
            request.MedicalRecord.HistoryOfPresentIllness
        );

        // Update additional fields if provided
        if (!string.IsNullOrEmpty(request.MedicalRecord.PhysicalExamination))
            medicalRecord.UpdatePhysicalExamination(request.MedicalRecord.PhysicalExamination);

        if (!string.IsNullOrEmpty(request.MedicalRecord.Assessment))
            medicalRecord.UpdateAssessment(request.MedicalRecord.Assessment);

        if (!string.IsNullOrEmpty(request.MedicalRecord.TreatmentPlan))
            medicalRecord.UpdateTreatmentPlan(request.MedicalRecord.TreatmentPlan);

        if (!string.IsNullOrEmpty(request.MedicalRecord.Notes)) medicalRecord.UpdateNotes(request.MedicalRecord.Notes);

        await medicalRecordRepository.AddAsync(medicalRecord);
        await uow.SaveChangesAsync(cancellationToken);

        return Result<MedicalRecordDto>.Success(mapper.Map(medicalRecord));
    }
}

public class UpdateMedicalRecordCommandHandler(IMedicalRecordRepository medicalRecordRepository, MedicalRecordMapper mapper, IUnitOfWork uow)
    : IRequestHandler<UpdateMedicalRecordCommand, Result<MedicalRecordDto>>
{
    public async Task<Result<MedicalRecordDto>> Handle(UpdateMedicalRecordCommand request, CancellationToken cancellationToken)
    {
        var medicalRecord = await medicalRecordRepository.GetByIdAsync(request.MedicalRecord.Id)
                            ?? null;
        if (medicalRecord is null)
            return Result<MedicalRecordDto>.Failure("Medical record not found");

        // Update core fields
        medicalRecord.UpdateHistoryOfPresentIllness(request.MedicalRecord.HistoryOfPresentIllness ?? string.Empty);
        medicalRecord.UpdatePhysicalExamination(request.MedicalRecord.PhysicalExamination ?? string.Empty);
        medicalRecord.UpdateAssessment(request.MedicalRecord.Assessment ?? string.Empty);
        medicalRecord.UpdateTreatmentPlan(request.MedicalRecord.TreatmentPlan ?? string.Empty);
        medicalRecord.UpdateNotes(request.MedicalRecord.Notes);

        await uow.SaveChangesAsync(cancellationToken);

        return Result<MedicalRecordDto>.Success(mapper.Map(medicalRecord));
    }
}

public class DeleteMedicalRecordCommandHandler(IMedicalRecordRepository medicalRecordRepository, IUnitOfWork uow)
    : IRequestHandler<DeleteMedicalRecordCommand, Result>
{
    public async Task<Result> Handle(DeleteMedicalRecordCommand request, CancellationToken cancellationToken)
    {
        var medicalRecord = await medicalRecordRepository.GetByIdAsync(request.MedicalRecordId)
                            ?? null;
        if (medicalRecord is null)
            return Result.Failure("Medical record not found");

        // Guard: prevent deletion if record has clinical content or is finalized
        if (await medicalRecordRepository.HasRelatedDataAsync(request.MedicalRecordId))
        {
            return Result.Failure("Cannot delete a medical record that is finalized or contains clinical data (diagnoses, prescriptions, vital signs). Archive instead.");
        }

        await medicalRecordRepository.DeleteAsync(medicalRecord.Id);
        await uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

public class AddDiagnosisCommandHandler(IMedicalRecordRepository medicalRecordRepository)
    : IRequestHandler<AddDiagnosisCommand, Result<MedicalRecordDto>>
{
    public async Task<Result<MedicalRecordDto>> Handle(AddDiagnosisCommand request, CancellationToken cancellationToken)
    {
        var medicalRecord = await medicalRecordRepository.GetByIdAsync(request.MedicalRecordId)
                            ?? null;
        if (medicalRecord is null)
            return Result<MedicalRecordDto>.Failure("Medical record not found");

        // Note: The MedicalRecord entity doesn't have a direct AddDiagnosis method
        // This would need to be implemented in the entity or handled differently
        // For now, we'll throw a NotImplementedException
        return Result<MedicalRecordDto>.Failure("AddDiagnosis functionality not implemented");

        // await medicalRecordRepository.SaveChangesAsync();
        // return MapToDto(medicalRecord);
    }

}

public class AddTreatmentCommandHandler(IMedicalRecordRepository medicalRecordRepository)
    : IRequestHandler<AddTreatmentCommand, Result<MedicalRecordDto>>
{
    public async Task<Result<MedicalRecordDto>> Handle(AddTreatmentCommand request, CancellationToken cancellationToken)
    {
        var medicalRecord = await medicalRecordRepository.GetByIdAsync(request.MedicalRecordId)
                            ?? null;
        if (medicalRecord is null)
            return Result<MedicalRecordDto>.Failure("Medical record not found");

        // Note: The MedicalRecord entity doesn't have a direct AddTreatment method
        // This would need to be implemented in the entity or handled differently
        // For now, we'll throw a NotImplementedException
        return Result<MedicalRecordDto>.Failure("AddTreatment functionality not implemented");

        // await medicalRecordRepository.SaveChangesAsync();
        // return MapToDto(medicalRecord);
    }

}

public class AddMedicationCommandHandler(IMedicalRecordRepository medicalRecordRepository)
    : IRequestHandler<AddMedicationCommand, Result<MedicalRecordDto>>
{
    public async Task<Result<MedicalRecordDto>> Handle(AddMedicationCommand request, CancellationToken cancellationToken)
    {
        var medicalRecord = await medicalRecordRepository.GetByIdAsync(request.MedicalRecordId)
                            ?? null;
        if (medicalRecord is null)
            return Result<MedicalRecordDto>.Failure("Medical record not found");

        // Note: The MedicalRecord entity doesn't have a direct AddMedication method
        // This would need to be implemented in the entity or handled differently
        // For now, we'll throw a NotImplementedException
        return Result<MedicalRecordDto>.Failure("AddMedication functionality not implemented");

        // await medicalRecordRepository.SaveChangesAsync();
        // return MapToDto(medicalRecord);
    }

}

public class UpsertMedicalRecordCommandHandler(
    IMedicalRecordRepository medicalRecordRepository,
    IPatientRepository patientRepository,
    IDoctorRepository doctorRepository,
    Common.Mapping.MedicalRecordMapper mapper,
    IUnitOfWork uow)
    : IRequestHandler<UpsertMedicalRecordCommand, Result<MedicalRecordDto>>
{
    public async Task<Result<MedicalRecordDto>> Handle(UpsertMedicalRecordCommand request, CancellationToken cancellationToken)
    {
        // Validate that patient exists
        var patient = await patientRepository.GetByIdAsync(request.MedicalRecord.PatientId);
        if (patient == null)
            return Result<MedicalRecordDto>.Failure($"Patient with ID {request.MedicalRecord.PatientId} does not exist.");

        // Validate that doctor exists
        var doctor = await doctorRepository.GetByIdAsync(request.MedicalRecord.DoctorId);
        if (doctor == null)
            return Result<MedicalRecordDto>.Failure($"Doctor with ID {request.MedicalRecord.DoctorId} does not exist.");

        MedicalRecord? medicalRecord;

        if (request.MedicalRecord.Id.HasValue)
        {
            // Update existing record
            medicalRecord = await medicalRecordRepository.GetByIdAsync(request.MedicalRecord.Id.Value);
            if (medicalRecord == null)
                return Result<MedicalRecordDto>.Failure($"Medical record with ID {request.MedicalRecord.Id.Value} does not exist.");

            // Update fields (medicalRecord is guaranteed to be non-null here)
            medicalRecord.UpdateChiefComplaint(request.MedicalRecord.ChiefComplaint);
            if (!string.IsNullOrEmpty(request.MedicalRecord.HistoryOfPresentIllness))
                medicalRecord.UpdateHistoryOfPresentIllness(request.MedicalRecord.HistoryOfPresentIllness);

            if (!string.IsNullOrEmpty(request.MedicalRecord.PhysicalExamination))
                medicalRecord.UpdatePhysicalExamination(request.MedicalRecord.PhysicalExamination);

            if (!string.IsNullOrEmpty(request.MedicalRecord.Assessment))
                medicalRecord.UpdateAssessment(request.MedicalRecord.Assessment);

            if (!string.IsNullOrEmpty(request.MedicalRecord.TreatmentPlan))
                medicalRecord.UpdateTreatmentPlan(request.MedicalRecord.TreatmentPlan);

            if (!string.IsNullOrEmpty(request.MedicalRecord.Notes))
                medicalRecord.UpdateNotes(request.MedicalRecord.Notes);
        }
        else
        {
            // Create new record
            medicalRecord = new MedicalRecord(
                request.MedicalRecord.PatientId,
                request.MedicalRecord.DoctorId,
                request.MedicalRecord.ChiefComplaint,
                null, // AppointmentId - not provided in DTO
                request.MedicalRecord.HistoryOfPresentIllness
            );

            // Update additional fields if provided
            if (!string.IsNullOrEmpty(request.MedicalRecord.PhysicalExamination))
                medicalRecord.UpdatePhysicalExamination(request.MedicalRecord.PhysicalExamination);

            if (!string.IsNullOrEmpty(request.MedicalRecord.Assessment))
                medicalRecord.UpdateAssessment(request.MedicalRecord.Assessment);

            if (!string.IsNullOrEmpty(request.MedicalRecord.TreatmentPlan))
                medicalRecord.UpdateTreatmentPlan(request.MedicalRecord.TreatmentPlan);

            if (!string.IsNullOrEmpty(request.MedicalRecord.Notes))
                medicalRecord.UpdateNotes(request.MedicalRecord.Notes);

            await medicalRecordRepository.AddAsync(medicalRecord);
        }

        await uow.SaveChangesAsync(cancellationToken);

        return Result<MedicalRecordDto>.Success(mapper.Map(medicalRecord));
    }
}

public class FinalizeMedicalRecordCommandHandler(
    IMedicalRecordRepository medicalRecordRepository,
    MedicalRecordMapper mapper,
    IUnitOfWork uow)
    : IRequestHandler<FinalizeMedicalRecordCommand, Result<MedicalRecordDto>>
{
    public async Task<Result<MedicalRecordDto>> Handle(FinalizeMedicalRecordCommand request, CancellationToken cancellationToken)
    {
        var record = await medicalRecordRepository.GetByIdAsync(request.MedicalRecordId);
        if (record is null)
            return Result<MedicalRecordDto>.Failure("Medical record not found");

        try
        {
            record.Finalize(request.FinalNotes, request.FinalizedBy);
            await uow.SaveChangesAsync(cancellationToken);
            return Result<MedicalRecordDto>.Success(mapper.Map(record));
        }
        catch (Exception ex)
        {
            return Result<MedicalRecordDto>.Failure(ex.Message);
        }
    }
}

public class ReopenMedicalRecordCommandHandler(
    IMedicalRecordRepository medicalRecordRepository,
    MedicalRecordMapper mapper,
    IUnitOfWork uow)
    : IRequestHandler<ReopenMedicalRecordCommand, Result<MedicalRecordDto>>
{
    public async Task<Result<MedicalRecordDto>> Handle(ReopenMedicalRecordCommand request, CancellationToken cancellationToken)
    {
        var record = await medicalRecordRepository.GetByIdAsync(request.MedicalRecordId);
        if (record is null)
            return Result<MedicalRecordDto>.Failure("Medical record not found");

        try
        {
            record.Reopen();
            await uow.SaveChangesAsync(cancellationToken);
            return Result<MedicalRecordDto>.Success(mapper.Map(record));
        }
        catch (Exception ex)
        {
            return Result<MedicalRecordDto>.Failure(ex.Message);
        }
    }
}
