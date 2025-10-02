// MedicalRecordFormDto moved to Web.Admin project.
using CareSync.Domain.Enums;

namespace CareSync.Application.DTOs.MedicalRecords;

public record MedicalRecordDto
{
    public Guid Id { get; init; }
    public Guid PatientId { get; init; }
    public Guid DoctorId { get; init; }
    public string PatientName { get; init; } = string.Empty;
    public string DoctorName { get; init; } = string.Empty;
    public DateTime RecordDate { get; init; }
    public string ChiefComplaint { get; init; } = string.Empty;
    public string? HistoryOfPresentIllness { get; init; }
    public string? PhysicalExamination { get; init; }
    public string? Assessment { get; init; }
    public string? TreatmentPlan { get; init; }
    public string? Notes { get; init; }
    public bool IsFinalized { get; init; }
    public DateTime? FinalizedAt { get; init; }
    public List<VitalSignsDto> VitalSigns { get; init; } = new();
    public List<DiagnosisDto> Diagnoses { get; init; } = new();
    public List<PrescriptionDto> Prescriptions { get; init; } = new();
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public bool HasRelatedData { get; init; } = false;
}

public record CreateMedicalRecordDto
{
    public Guid PatientId { get; init; }
    public Guid DoctorId { get; init; }
    public string ChiefComplaint { get; init; } = string.Empty;
    public string? HistoryOfPresentIllness { get; init; }
    public string? PhysicalExamination { get; init; }
    public string? Assessment { get; init; }
    public string? TreatmentPlan { get; init; }
    public string? Notes { get; init; }
    public List<CreateVitalSignsDto> VitalSigns { get; init; } = new();
}

public record UpdateMedicalRecordDto
{
    public Guid Id { get; init; }
    public string? HistoryOfPresentIllness { get; init; }
    public string? PhysicalExamination { get; init; }
    public string? Assessment { get; init; }
    public string? TreatmentPlan { get; init; }
    public string? Notes { get; init; }
}

public record UpsertMedicalRecordDto(
    Guid? Id,
    Guid PatientId,
    Guid DoctorId,
    string ChiefComplaint,
    string? HistoryOfPresentIllness,
    string? PhysicalExamination,
    string? Assessment,
    string? TreatmentPlan,
    string? Notes
);

// MedicalRecordFormDto relocated to Web.Admin (see Forms/MedicalRecordFormDto.cs)

public record VitalSignsDto
{
    public Guid Id { get; init; }
    public DateTime MeasurementDate { get; init; }
    public decimal? Temperature { get; init; }
    public int? SystolicBp { get; init; }
    public int? DiastolicBp { get; init; }
    public int? HeartRate { get; init; }
    public int? RespiratoryRate { get; init; }
    public decimal? Weight { get; init; }
    public decimal? Height { get; init; }
    public decimal? Bmi { get; init; }
    public int? OxygenSaturation { get; init; }
    public string? Notes { get; init; }
}

public record CreateVitalSignsDto
{
    public decimal? Temperature { get; init; }
    public int? SystolicBp { get; init; }
    public int? DiastolicBp { get; init; }
    public int? HeartRate { get; init; }
    public int? RespiratoryRate { get; init; }
    public decimal? Weight { get; init; }
    public decimal? Height { get; init; }
    public int? OxygenSaturation { get; init; }
    public string? Notes { get; init; }
}

public record DiagnosisDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsPrimary { get; init; }
    public DiagnosisStatus Status { get; init; }
    public DateTime DiagnosisDate { get; init; }
    public string? Notes { get; init; }
}

public record CreateDiagnosisDto
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsPrimary { get; init; }
    public string? Notes { get; init; }
}

public record PrescriptionDto
{
    public Guid Id { get; init; }
    public string MedicationName { get; init; } = string.Empty;
    public string Dosage { get; init; } = string.Empty;
    public string Frequency { get; init; } = string.Empty;
    public string Route { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public int? Refills { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public PrescriptionStatus Status { get; init; }
    public string? Instructions { get; init; }
    public string? Notes { get; init; }
}

public record CreatePrescriptionDto
{
    public string MedicationName { get; init; } = string.Empty;
    public string Dosage { get; init; } = string.Empty;
    public string Frequency { get; init; } = string.Empty;
    public string Route { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public int? Refills { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string? Instructions { get; init; }
    public string? Notes { get; init; }
}

public record TreatmentRecordDto
{
    public Guid Id { get; init; }
    public Guid TreatmentId { get; init; }
    public string TreatmentName { get; init; } = string.Empty;
    public Guid ProviderId { get; init; }
    public DateTime TreatmentDate { get; init; }
    public TreatmentStatus Status { get; init; }
    public int ActualDurationMinutes { get; init; }
    public decimal ActualCost { get; init; }
    public string? Notes { get; init; }
    public string? Complications { get; init; }
    public string? FollowUpInstructions { get; init; }
    public DateTime? FollowUpDate { get; init; }
}

public record CreateTreatmentRecordDto
{
    public Guid TreatmentId { get; init; }
    public Guid ProviderId { get; init; }
    public DateTime TreatmentDate { get; init; }
    public decimal ActualCost { get; init; }
    public int ActualDurationMinutes { get; init; }
}

public record FinalizeMedicalRecordDto
{
    public Guid Id { get; init; }
    public string? FinalNotes { get; init; }
    public string? FinalizedBy { get; init; }
}

public record ReopenMedicalRecordDto
{
    public Guid Id { get; init; }
}
