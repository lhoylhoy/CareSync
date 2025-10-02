using CareSync.Domain.Enums;
using CareSync.Domain.Events;
using CareSync.Domain.Interfaces;

namespace CareSync.Domain.Entities;

public class Appointment : IHasDomainEvents
{
    private Appointment() { }

    public Appointment(Guid patientId, Guid doctorId, DateTime scheduledDate, int durationMinutes,
        string appointmentType)
    {
        if (scheduledDate <= DateTime.UtcNow)
            throw new ArgumentException("Scheduled date must be in the future.", nameof(scheduledDate));

        if (durationMinutes <= 0)
            throw new ArgumentException("Duration must be greater than zero.", nameof(durationMinutes));

        if (string.IsNullOrWhiteSpace(appointmentType))
            throw new ArgumentException("Appointment type cannot be null or empty.", nameof(appointmentType));

        Id = Guid.NewGuid();
        PatientId = patientId;
        DoctorId = doctorId;
        ScheduledDate = scheduledDate;
        Status = AppointmentStatus.Scheduled;
        Duration = TimeSpan.FromMinutes(durationMinutes);
        AppointmentType = appointmentType.Trim();
    }

    // Legacy constructor for backward compatibility
    public Appointment(Guid id, Guid patientId, Guid doctorId, DateTime scheduledDate, TimeSpan? duration = null)
    {
        if (scheduledDate <= DateTime.UtcNow)
            throw new ArgumentException("Scheduled date must be in the future.", nameof(scheduledDate));

        Id = id;
        PatientId = patientId;
        DoctorId = doctorId;
        ScheduledDate = scheduledDate;
        Status = AppointmentStatus.Scheduled;
        Duration = duration ?? TimeSpan.FromMinutes(30); // Default 30 minutes
        AppointmentType = "Consultation"; // Default type
    }

    public Guid Id { get; private set; }
    public Guid PatientId { get; private set; }
    public Patient? Patient { get; private set; }
    public Guid DoctorId { get; private set; }
    public Doctor? Doctor { get; private set; }
    public DateTime ScheduledDate { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public TimeSpan Duration { get; private set; }
    public string AppointmentType { get; private set; } = string.Empty;
    public string? Notes { get; private set; }
    public string? ReasonForVisit { get; private set; }
    public string? CancellationReason { get; private set; }

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    private void Raise(DomainEvent evt) => _domainEvents.Add(evt);
    public void ClearDomainEvents() => _domainEvents.Clear();

    public bool CanBeModified => Status == AppointmentStatus.Scheduled && ScheduledDate > DateTime.UtcNow.AddHours(2);

    public DateTime EndTime => ScheduledDate.Add(Duration);

    public void Reschedule(DateTime newScheduledDate)
    {
        if (newScheduledDate <= DateTime.UtcNow)
            throw new ArgumentException("New scheduled date must be in the future.", nameof(newScheduledDate));

        if (Status != AppointmentStatus.Scheduled)
            throw new InvalidOperationException("Only scheduled appointments can be rescheduled.");

        ScheduledDate = newScheduledDate;
    }

    public void UpdateDuration(int durationMinutes)
    {
        if (durationMinutes <= 0)
            throw new ArgumentException("Duration must be greater than zero.", nameof(durationMinutes));

        Duration = TimeSpan.FromMinutes(durationMinutes);
    }

    public void Cancel(string reason)
    {
        if (Status == AppointmentStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed appointment.");

        if (Status == AppointmentStatus.Cancelled)
            throw new InvalidOperationException("Appointment is already cancelled.");

        Status = AppointmentStatus.Cancelled;
        CancellationReason = reason?.Trim();
        Raise(new AppointmentCancelledEvent(Id, PatientId, DoctorId, CancellationReason, DateTime.UtcNow));
    }

    public void Start()
    {
        if (Status != AppointmentStatus.Scheduled)
            throw new InvalidOperationException("Only scheduled appointments can be started.");

        Status = AppointmentStatus.InProgress;
        Raise(new AppointmentStartedEvent(Id, PatientId, DoctorId, DateTime.UtcNow));
    }

    public void Complete(string? notes = null)
    {
        if (Status != AppointmentStatus.InProgress && Status != AppointmentStatus.Scheduled)
            throw new InvalidOperationException("Only scheduled or in-progress appointments can be completed.");

        Status = AppointmentStatus.Completed;
        Notes = notes?.Trim();
        Raise(new AppointmentCompletedEvent(Id, PatientId, DoctorId, DateTime.UtcNow));
    }

    public void UpdateNotes(string notes)
    {
        Notes = notes?.Trim();
    }

    public void UpdateReasonForVisit(string reasonForVisit)
    {
        ReasonForVisit = reasonForVisit?.Trim();
    }

    public void MarkAsNoShow()
    {
        if (Status != AppointmentStatus.Scheduled)
            throw new InvalidOperationException("Only scheduled appointments can be marked as no-show.");

        if (ScheduledDate.Date > DateTime.UtcNow.Date)
            throw new InvalidOperationException("Cannot mark future appointments as no-show.");

        Status = AppointmentStatus.NoShow;
        Raise(new AppointmentNoShowEvent(Id, PatientId, DoctorId, DateTime.UtcNow));
    }
}
