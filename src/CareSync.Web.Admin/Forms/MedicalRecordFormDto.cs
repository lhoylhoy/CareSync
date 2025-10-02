namespace CareSync.Web.Admin.Forms;

using CareSync.Application.DTOs.MedicalRecords;

public class MedicalRecordFormDto
{
    public Guid? Id { get; set; }
    public string PatientIdString { get; set; } = string.Empty;
    public string DoctorIdString { get; set; } = string.Empty;
    public Guid PatientId => Guid.TryParse(PatientIdString, out var p) ? p : Guid.Empty;
    public Guid DoctorId => Guid.TryParse(DoctorIdString, out var d) ? d : Guid.Empty;
    public string ChiefComplaint { get; set; } = string.Empty;
    public string? HistoryOfPresentIllness { get; set; }
    public string? PhysicalExamination { get; set; }
    public string? Assessment { get; set; }
    public string? TreatmentPlan { get; set; }
    public string? Notes { get; set; }
    public CreateMedicalRecordDto ToCreateDto() => new() { PatientId = PatientId, DoctorId = DoctorId, ChiefComplaint = ChiefComplaint, HistoryOfPresentIllness = HistoryOfPresentIllness, PhysicalExamination = PhysicalExamination, Assessment = Assessment, TreatmentPlan = TreatmentPlan, Notes = Notes };
    public UpdateMedicalRecordDto ToUpdateDto() => new() { Id = Id!.Value, HistoryOfPresentIllness = HistoryOfPresentIllness, PhysicalExamination = PhysicalExamination, Assessment = Assessment, TreatmentPlan = TreatmentPlan, Notes = Notes };
    public UpsertMedicalRecordDto ToUpsertDto() => new(Id, PatientId, DoctorId, ChiefComplaint, HistoryOfPresentIllness, PhysicalExamination, Assessment, TreatmentPlan, Notes);
    public static MedicalRecordFormDto FromDto(MedicalRecordDto dto) => new() { Id = dto.Id, PatientIdString = dto.PatientId.ToString(), DoctorIdString = dto.DoctorId.ToString(), ChiefComplaint = dto.ChiefComplaint, HistoryOfPresentIllness = dto.HistoryOfPresentIllness, PhysicalExamination = dto.PhysicalExamination, Assessment = dto.Assessment, TreatmentPlan = dto.TreatmentPlan, Notes = dto.Notes };
}
