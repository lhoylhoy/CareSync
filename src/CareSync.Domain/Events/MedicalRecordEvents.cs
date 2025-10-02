namespace CareSync.Domain.Events;

public record MedicalRecordFinalizedEvent(Guid MedicalRecordId, Guid PatientId, Guid DoctorId, DateTime FinalizedAtUtc) : DomainEvent;
public record MedicalRecordReopenedEvent(Guid MedicalRecordId, Guid PatientId, Guid DoctorId) : DomainEvent;
