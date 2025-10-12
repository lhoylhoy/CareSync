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

    public async Task<(IReadOnlyList<Appointment> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? searchTerm,
        IReadOnlyDictionary<string, string?> filters,
        CancellationToken cancellationToken = default)
    {
        if (page <= 0) page = CareSync.Application.Common.PagingDefaults.DefaultPage;
        if (pageSize <= 0) pageSize = CareSync.Application.Common.PagingDefaults.DefaultPageSize;
        pageSize = Math.Min(pageSize, CareSync.Application.Common.PagingDefaults.MaxPageSize);

        var query = _context.Appointments
            .AsNoTracking()
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .AsSplitQuery()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            var likeTerm = $"%{term}%";

            query = query.Where(a =>
                (a.Patient != null && (
                    EF.Functions.Like(a.Patient.FullName.FirstName, likeTerm) ||
                    EF.Functions.Like(a.Patient.FullName.LastName, likeTerm) ||
                    (a.Patient.FullName.MiddleName != null && EF.Functions.Like(a.Patient.FullName.MiddleName, likeTerm))
                )) ||
                (a.Doctor != null && (
                    EF.Functions.Like(a.Doctor.Name.FirstName, likeTerm) ||
                    EF.Functions.Like(a.Doctor.Name.LastName, likeTerm) ||
                    (a.Doctor.Name.MiddleName != null && EF.Functions.Like(a.Doctor.Name.MiddleName, likeTerm)) ||
                    EF.Functions.Like(a.Doctor.Specialty, likeTerm)
                )) ||
                EF.Functions.Like(a.AppointmentType, likeTerm) ||
                (a.Notes != null && EF.Functions.Like(a.Notes, likeTerm)) ||
                (a.CancellationReason != null && EF.Functions.Like(a.CancellationReason, likeTerm)));
        }

        if (filters != null && filters.Count > 0)
        {
            if (filters.TryGetValue("Status", out var statusValue) && !string.IsNullOrWhiteSpace(statusValue) &&
                Enum.TryParse<AppointmentStatus>(statusValue, true, out var status))
            {
                query = query.Where(a => a.Status == status);
            }

            if (filters.TryGetValue("DoctorId", out var doctorValue) && Guid.TryParse(doctorValue, out var doctorId))
            {
                query = query.Where(a => a.DoctorId == doctorId);
            }

            if (filters.TryGetValue("PatientId", out var patientValue) && Guid.TryParse(patientValue, out var patientId))
            {
                query = query.Where(a => a.PatientId == patientId);
            }

            if (filters.TryGetValue("ScheduledFrom", out var fromValue) &&
                DateTime.TryParse(fromValue, out var fromDate))
            {
                query = query.Where(a => a.ScheduledDate >= fromDate);
            }

            if (filters.TryGetValue("ScheduledTo", out var toValue) &&
                DateTime.TryParse(toValue, out var toDate))
            {
                query = query.Where(a => a.ScheduledDate <= toDate);
            }
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(a => a.ScheduledDate)
            .ThenBy(a => a.Patient != null ? a.Patient.FullName.LastName : string.Empty)
            .ThenBy(a => a.Patient != null ? a.Patient.FullName.FirstName : string.Empty)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
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
