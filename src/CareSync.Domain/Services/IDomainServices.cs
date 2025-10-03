using CareSync.Domain.Entities;

namespace CareSync.Domain.Services;

public interface IAppointmentSchedulingService
{
    public Task<bool> IsDoctorAvailableAsync(Guid doctorId, DateTime startTime, TimeSpan duration);

    public Task<Appointment> ScheduleAppointmentAsync(Guid patientId, Guid doctorId, DateTime scheduledDate,
        TimeSpan? duration = null);

    public Task<bool> CanRescheduleAsync(Guid appointmentId, DateTime newDateTime);
    public Task RescheduleAppointmentAsync(Guid appointmentId, DateTime newDateTime);
}

public interface IMedicalRecordValidationService
{
    public Task<bool> ValidateRecordCompleteness(MedicalRecord record);
    public Task<List<string>> GetValidationErrors(MedicalRecord record);
    public Task<bool> CanFinalizeRecord(Guid recordId, Guid doctorId);
}

public interface IBillingCalculationService
{
    public Task<decimal> CalculateTotalAmount(Bill bill);
    public Task<decimal> CalculateInsuranceCoverage(Guid patientId, Guid appointmentId);
    public Task<Bill> GenerateBillFromAppointment(Guid appointmentId);
}
