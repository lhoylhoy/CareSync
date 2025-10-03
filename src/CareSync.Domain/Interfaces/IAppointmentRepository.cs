using CareSync.Domain.Entities;

namespace CareSync.Domain.Interfaces;

public interface IAppointmentRepository
{
    public Task<Appointment?> GetByIdAsync(Guid id);
    public Task<IEnumerable<Appointment>> GetAllAsync();
    public Task<int> GetTotalCountAsync();
    public Task<IEnumerable<Appointment>> GetByPatientIdAsync(Guid patientId);
    public Task<IEnumerable<Appointment>> GetByDoctorIdAsync(Guid doctorId);

    public Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAndDateRangeAsync(Guid doctorId, DateTime startDateTime,
        DateTime endDateTime);

    public Task AddAsync(Appointment appointment);
    public Task UpdateAsync(Appointment appointment);
    public Task DeleteAsync(Guid id);

    // Optimized methods with eager loading to avoid N+1 queries
    public Task<Appointment?> GetByIdWithDetailsAsync(Guid id);
    public Task<IEnumerable<Appointment>> GetAllWithDetailsAsync();
    public Task<IEnumerable<Appointment>> GetByPatientIdWithDetailsAsync(Guid patientId);
    public Task<IEnumerable<Appointment>> GetByDoctorIdWithDetailsAsync(Guid doctorId);
    public Task<bool> HasConflictingAppointmentAsync(Guid doctorId, DateTime startTime, DateTime endTime);
    // Returns true if appointment has downstream clinical/financial artifacts (medical record, bill, etc.)
    public Task<bool> HasRelatedDataAsync(Guid id);
}
