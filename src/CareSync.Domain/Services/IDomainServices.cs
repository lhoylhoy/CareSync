using CareSync.Domain.Entities;

namespace CareSync.Domain.Services;

public interface IAppointmentSchedulingService
{
    Task<bool> IsDoctorAvailableAsync(Guid doctorId, DateTime startTime, TimeSpan duration);

    Task<Appointment> ScheduleAppointmentAsync(Guid patientId, Guid doctorId, DateTime scheduledDate,
        TimeSpan? duration = null);

    Task<bool> CanRescheduleAsync(Guid appointmentId, DateTime newDateTime);
    Task RescheduleAppointmentAsync(Guid appointmentId, DateTime newDateTime);
}

public interface IMedicalRecordValidationService
{
    Task<bool> ValidateRecordCompleteness(MedicalRecord record);
    Task<List<string>> GetValidationErrors(MedicalRecord record);
    Task<bool> CanFinalizeRecord(Guid recordId, Guid doctorId);
}

public interface IBillingCalculationService
{
    Task<decimal> CalculateTotalAmount(Bill bill);
    Task<decimal> CalculateInsuranceCoverage(Guid patientId, Guid appointmentId);
    Task<Bill> GenerateBillFromAppointment(Guid appointmentId);
}
