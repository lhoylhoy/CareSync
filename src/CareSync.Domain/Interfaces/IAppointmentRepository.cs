using CareSync.Domain.Entities;

namespace CareSync.Domain.Interfaces;

public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(Guid id);
    Task<IEnumerable<Appointment>> GetAllAsync();
    Task<int> GetTotalCountAsync();
    Task<IEnumerable<Appointment>> GetByPatientIdAsync(Guid patientId);
    Task<IEnumerable<Appointment>> GetByDoctorIdAsync(Guid doctorId);

    Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAndDateRangeAsync(Guid doctorId, DateTime startDateTime,
        DateTime endDateTime);

    Task AddAsync(Appointment appointment);
    Task UpdateAsync(Appointment appointment);
    Task DeleteAsync(Guid id);

    // Optimized methods with eager loading to avoid N+1 queries
    Task<Appointment?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<Appointment>> GetAllWithDetailsAsync();
    Task<IEnumerable<Appointment>> GetByPatientIdWithDetailsAsync(Guid patientId);
    Task<IEnumerable<Appointment>> GetByDoctorIdWithDetailsAsync(Guid doctorId);
    Task<bool> HasConflictingAppointmentAsync(Guid doctorId, DateTime startTime, DateTime endTime);
    // Returns true if appointment has downstream clinical/financial artifacts (medical record, bill, etc.)
    Task<bool> HasRelatedDataAsync(Guid id);
}
