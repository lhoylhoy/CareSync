using CareSync.Application.DTOs.MedicalRecords;
using MediatR;
using CareSync.Application.Common.Results;

namespace CareSync.Application.Commands.MedicalRecords;

public record CreateMedicalRecordCommand(CreateMedicalRecordDto MedicalRecord) : IRequest<Result<MedicalRecordDto>>;

public record UpdateMedicalRecordCommand(UpdateMedicalRecordDto MedicalRecord) : IRequest<Result<MedicalRecordDto>>;

public record DeleteMedicalRecordCommand(Guid MedicalRecordId) : IRequest<Result>;

public record UpsertMedicalRecordCommand(UpsertMedicalRecordDto MedicalRecord) : IRequest<Result<MedicalRecordDto>>;

public record AddDiagnosisCommand(Guid MedicalRecordId, string Diagnosis) : IRequest<Result<MedicalRecordDto>>;

public record AddTreatmentCommand(Guid MedicalRecordId, string Treatment) : IRequest<Result<MedicalRecordDto>>;

public record AddMedicationCommand(Guid MedicalRecordId, string Medication, string Dosage, string Instructions)
    : IRequest<Result<MedicalRecordDto>>;

// Lifecycle commands
public record FinalizeMedicalRecordCommand(Guid MedicalRecordId, string? FinalNotes, string? FinalizedBy)
    : IRequest<Result<MedicalRecordDto>>;

public record ReopenMedicalRecordCommand(Guid MedicalRecordId)
    : IRequest<Result<MedicalRecordDto>>;
