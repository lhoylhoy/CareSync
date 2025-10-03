using CareSync.Application.DTOs.MedicalRecords;
using CareSync.Domain.Entities;

namespace CareSync.Application.Common.Mapping;

public sealed class MedicalRecordMapper : IEntityMapper<MedicalRecord, MedicalRecordDto>
{
    public MedicalRecordDto Map(MedicalRecord record)
    {
        var vitals = record.VitalSigns.Select(v => new VitalSignsDto
        {
            Id = v.Id,
            MeasurementDate = v.MeasurementDate,
            Temperature = v.Temperature,
            SystolicBp = v.SystolicBp,
            DiastolicBp = v.DiastolicBp,
            HeartRate = v.HeartRate,
            RespiratoryRate = v.RespiratoryRate,
            Weight = v.Weight,
            Height = v.Height,
            Bmi = v.Bmi,
            // Domain uses decimal? for OxygenSaturation; DTO expects int? (percentage as whole number)
            OxygenSaturation = v.OxygenSaturation.HasValue ? (int?)Math.Round(v.OxygenSaturation.Value) : null,
            Notes = v.Notes
        }).ToList();

        var diagnoses = record.Diagnoses.Select(d => new DiagnosisDto
        {
            Id = d.Id,
            Code = d.Code,
            Name = d.Name,
            Description = d.Description,
            IsPrimary = d.IsPrimary,
            Status = d.Status,
            DiagnosisDate = d.DiagnosisDate,
            Notes = d.Notes
        }).ToList();

        var prescriptions = record.Prescriptions.Select(p => new PrescriptionDto
        {
            Id = p.Id,
            MedicationName = p.MedicationName,
            Dosage = p.Dosage,
            Frequency = p.Frequency,
            Route = p.Route,
            Quantity = p.Quantity,
            Refills = p.Refills,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            Status = p.Status,
            Instructions = p.Instructions,
            Notes = p.Notes
        }).ToList();

        return new MedicalRecordDto
        {
            Id = record.Id,
            PatientId = record.PatientId,
            DoctorId = record.DoctorId,
            PatientName = record.Patient?.FullName?.ToString() ?? string.Empty,
            DoctorName = record.Doctor?.DisplayName ?? string.Empty,
            RecordDate = record.RecordDate,
            ChiefComplaint = record.ChiefComplaint,
            HistoryOfPresentIllness = record.HistoryOfPresentIllness,
            PhysicalExamination = record.PhysicalExamination,
            Assessment = record.Assessment,
            TreatmentPlan = record.TreatmentPlan,
            Notes = record.Notes,
            IsFinalized = record.IsFinalized,
            FinalizedAt = record.FinalizedDate,
            VitalSigns = vitals,
            Diagnoses = diagnoses,
            Prescriptions = prescriptions,
            CreatedAt = record.RecordDate,
            UpdatedAt = record.FinalizedDate,
            HasRelatedData = false // populated post-mapping in query handlers
        };
    }
}

public static class MedicalRecordMapperExtensions
{
    public static MedicalRecordDto ToDto(this MedicalRecord record, MedicalRecordMapper mapper) => mapper.Map(record);
}// Placeholder removed: MedicalRecordMapper pending entity review.
