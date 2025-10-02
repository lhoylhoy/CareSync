using CareSync.Domain.Entities;
using CareSync.Domain.Enums;
using CareSync.Domain.Interfaces;
using CareSync.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CareSync.Infrastructure.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly CareSyncDbContext _context;

    public AppointmentRepository(CareSyncDbContext context)
    {
        _context = context;
    }

    public async Task<Appointment?> GetByIdAsync(Guid id)
    {
        return await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Appointment>> GetAllAsync()
    {
        return await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .OrderBy(a => a.ScheduledDate)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _context.Appointments.CountAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(Guid patientId)
    {
        return await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Where(a => a.PatientId == patientId)
            .OrderBy(a => a.ScheduledDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(Guid doctorId)
    {
        return await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Where(a => a.DoctorId == doctorId)
            .OrderBy(a => a.ScheduledDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAndDateRangeAsync(Guid doctorId,
        DateTime startDateTime, DateTime endDateTime)
    {
        return await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Where(a => a.DoctorId == doctorId &&
                        a.ScheduledDate < endDateTime &&
                        a.ScheduledDate.Add(a.Duration) > startDateTime)
            .ToListAsync();
    }

    public async Task AddAsync(Appointment appointment)
    {
        await _context.Appointments.AddAsync(appointment);
    }

    public Task UpdateAsync(Appointment appointment)
    {
        _context.Appointments.Update(appointment);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var appointment = await GetByIdAsync(id);
        if (appointment != null) _context.Appointments.Remove(appointment);
    }

    public async Task<Appointment?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Appointment>> GetAllWithDetailsAsync()
    {
        return await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .OrderBy(a => a.ScheduledDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByPatientIdWithDetailsAsync(Guid patientId)
    {
        return await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Where(a => a.PatientId == patientId)
            .OrderBy(a => a.ScheduledDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByDoctorIdWithDetailsAsync(Guid doctorId)
    {
        return await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Where(a => a.DoctorId == doctorId)
            .OrderBy(a => a.ScheduledDate)
            .ToListAsync();
    }

    public async Task<bool> HasConflictingAppointmentAsync(Guid doctorId, DateTime startTime, DateTime endTime)
    {
        return await _context.Appointments
            .AnyAsync(a => a.DoctorId == doctorId &&
                           a.Status == AppointmentStatus.Scheduled &&
                           a.ScheduledDate < endTime &&
                           a.ScheduledDate.Add(a.Duration) > startTime);
    }

    public async Task<bool> HasRelatedDataAsync(Guid id)
    {
        // Related data definition for appointments: any medical record referencing this appointment.
        // Future: bills, prescriptions directly tied to appointment.
        var hasMedicalRecord = await _context.MedicalRecords.AnyAsync(m => m.AppointmentId == id);
        return hasMedicalRecord;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
