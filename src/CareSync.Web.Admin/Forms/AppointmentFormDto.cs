namespace CareSync.Web.Admin.Forms;

using CareSync.Application.DTOs.Appointments;

public class AppointmentFormDto
{
    public Guid? Id { get; set; }
    public string PatientIdString { get; set; } = string.Empty;
    public string DoctorIdString { get; set; } = string.Empty;
    public Guid PatientId => Guid.TryParse(PatientIdString, out var p) ? p : Guid.Empty;
    public Guid DoctorId => Guid.TryParse(DoctorIdString, out var d) ? d : Guid.Empty;
    public DateTime StartDateTime { get; set; } = DateTime.Now.AddHours(1);
    public int DurationMinutes { get; set; } = 30;
    public string AppointmentType { get; set; } = "Consultation";
    public string? Notes { get; set; }
    public string? ReasonForVisit { get; set; }

    public UpsertAppointmentDto ToUpsertDto() => new(Id, PatientId, DoctorId, StartDateTime, DurationMinutes, AppointmentType, Notes, ReasonForVisit);

    public static AppointmentFormDto FromDto(AppointmentDto dto) => new()
    {
        Id = dto.Id,
        PatientIdString = dto.PatientId.ToString(),
        DoctorIdString = dto.DoctorId.ToString(),
        StartDateTime = dto.StartDateTime,
        DurationMinutes = (int)dto.Duration.TotalMinutes,
        AppointmentType = "Consultation",
        Notes = dto.Notes
    };
}
