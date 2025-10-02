namespace CareSync.Domain.Events;

public record AppointmentStartedEvent(Guid AppointmentId, Guid PatientId, Guid DoctorId, DateTime StartedAtUtc) : DomainEvent;
public record AppointmentNoShowEvent(Guid AppointmentId, Guid PatientId, Guid DoctorId, DateTime OccurredAtUtc) : DomainEvent;
public record AppointmentCompletedEvent(Guid AppointmentId, Guid PatientId, Guid DoctorId, DateTime CompletedAtUtc) : DomainEvent;
public record AppointmentCancelledEvent(Guid AppointmentId, Guid PatientId, Guid DoctorId, string? Reason, DateTime CancelledAtUtc) : DomainEvent;
