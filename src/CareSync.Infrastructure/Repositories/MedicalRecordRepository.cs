using CareSync.Domain.Entities;
using CareSync.Domain.Interfaces;
using CareSync.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CareSync.Infrastructure.Repositories;

/// <summary>
///     Medical Record repository implementation
/// </summary>
public class MedicalRecordRepository(CareSyncDbContext context) : IMedicalRecordRepository
{
    public async Task<MedicalRecord?> GetByIdAsync(Guid id)
    {
        return await context.MedicalRecords
            .Include(mr => mr.Patient)
            .Include(mr => mr.Doctor)
            .FirstOrDefaultAsync(mr => mr.Id == id);
    }

    public async Task<MedicalRecord?> GetByIdWithDetailsAsync(Guid id)
    {
        return await context.MedicalRecords
            .Include(mr => mr.Patient)
            .Include(mr => mr.Doctor)
            .FirstOrDefaultAsync(mr => mr.Id == id);
    }

    public async Task<IEnumerable<MedicalRecord>> GetAllAsync()
    {
        return await context.MedicalRecords
            .AsNoTracking()
            .Include(mr => mr.Patient)
            .Include(mr => mr.Doctor)
            .ToListAsync();
    }

    public async Task<(IReadOnlyList<MedicalRecord> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? searchTerm,
        IReadOnlyDictionary<string, string?> filters,
        CancellationToken cancellationToken = default)
    {
        if (page <= 0) page = CareSync.Application.Common.PagingDefaults.DefaultPage;
        if (pageSize <= 0) pageSize = CareSync.Application.Common.PagingDefaults.DefaultPageSize;
        pageSize = Math.Min(pageSize, CareSync.Application.Common.PagingDefaults.MaxPageSize);

        var query = context.MedicalRecords
            .AsNoTracking()
            .Include(mr => mr.Patient)
            .Include(mr => mr.Doctor)
            .AsSplitQuery()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            var likeTerm = $"%{term}%";

            query = query.Where(mr =>
                EF.Functions.Like(mr.ChiefComplaint, likeTerm) ||
                EF.Functions.Like(mr.Assessment, likeTerm) ||
                EF.Functions.Like(mr.TreatmentPlan, likeTerm) ||
                (mr.Notes != null && EF.Functions.Like(mr.Notes, likeTerm)) ||
                (mr.Patient != null && (
                    EF.Functions.Like(mr.Patient.FullName.FirstName, likeTerm) ||
                    EF.Functions.Like(mr.Patient.FullName.LastName, likeTerm) ||
                    (mr.Patient.FullName.MiddleName != null && EF.Functions.Like(mr.Patient.FullName.MiddleName, likeTerm)))) ||
                (mr.Doctor != null && (
                    EF.Functions.Like(mr.Doctor.Name.FirstName, likeTerm) ||
                    EF.Functions.Like(mr.Doctor.Name.LastName, likeTerm) ||
                    (mr.Doctor.Name.MiddleName != null && EF.Functions.Like(mr.Doctor.Name.MiddleName, likeTerm)) ||
                    EF.Functions.Like(mr.Doctor.Specialty, likeTerm))));
        }

        if (filters is { Count: > 0 })
        {
            if (filters.TryGetValue("PatientId", out var patientValue) &&
                Guid.TryParse(patientValue, out var patientId))
            {
                query = query.Where(mr => mr.PatientId == patientId);
            }

            if (filters.TryGetValue("DoctorId", out var doctorValue) &&
                Guid.TryParse(doctorValue, out var doctorId))
            {
                query = query.Where(mr => mr.DoctorId == doctorId);
            }

            if (filters.TryGetValue("AppointmentId", out var appointmentValue) &&
                Guid.TryParse(appointmentValue, out var appointmentId))
            {
                query = query.Where(mr => mr.AppointmentId == appointmentId);
            }

            if (filters.TryGetValue("IsFinalized", out var finalizedValue) &&
                bool.TryParse(finalizedValue, out var isFinalized))
            {
                query = query.Where(mr => mr.IsFinalized == isFinalized);
            }

            if (filters.TryGetValue("RecordDateFrom", out var fromValue) &&
                DateTime.TryParse(fromValue, out var fromDate))
            {
                query = query.Where(mr => mr.RecordDate >= fromDate);
            }

            if (filters.TryGetValue("RecordDateTo", out var toValue) &&
                DateTime.TryParse(toValue, out var toDate))
            {
                query = query.Where(mr => mr.RecordDate <= toDate);
            }

            if (filters.TryGetValue("HasAppointment", out var hasAppointmentValue) &&
                bool.TryParse(hasAppointmentValue, out var hasAppointment))
            {
                query = hasAppointment
                    ? query.Where(mr => mr.AppointmentId.HasValue)
                    : query.Where(mr => !mr.AppointmentId.HasValue);
            }

            if (filters.TryGetValue("DiagnosisCode", out var diagnosisCode) &&
                !string.IsNullOrWhiteSpace(diagnosisCode))
            {
                query = query.Where(mr => mr.Diagnoses.Any(d => d.Code == diagnosisCode));
            }
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(mr => mr.RecordDate)
            .ThenBy(mr => mr.Patient != null ? mr.Patient.FullName.LastName : string.Empty)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<List<MedicalRecord>> GetByPatientIdAsync(Guid patientId)
    {
        return await context.MedicalRecords
            .Where(mr => mr.PatientId == patientId)
            .OrderByDescending(mr => mr.RecordDate)
            .AsNoTracking()
            .Include(mr => mr.Patient)
            .Include(mr => mr.Doctor)
            .ToListAsync();
    }

    public async Task<List<MedicalRecord>> GetByDoctorIdAsync(Guid doctorId)
    {
        return await context.MedicalRecords
            .Where(mr => mr.DoctorId == doctorId)
            .OrderByDescending(mr => mr.RecordDate)
            .AsNoTracking()
            .Include(mr => mr.Patient)
            .Include(mr => mr.Doctor)
            .ToListAsync();
    }

    public async Task<List<MedicalRecord>> GetByAppointmentIdAsync(Guid appointmentId)
    {
        return await context.MedicalRecords
            .Where(mr => mr.AppointmentId == appointmentId)
            .OrderByDescending(mr => mr.RecordDate)
            .AsNoTracking()
            .Include(mr => mr.Patient)
            .Include(mr => mr.Doctor)
            .ToListAsync();
    }

    public async Task<IEnumerable<MedicalRecord>> SearchAsync(
        Guid? patientId = null,
        Guid? doctorId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        bool? isFinalized = null)
    {
        var query = context.MedicalRecords.AsQueryable();
        query = query.Include(mr => mr.Patient).Include(mr => mr.Doctor);

        if (patientId.HasValue)
            query = query.Where(mr => mr.PatientId == patientId.Value);

        if (doctorId.HasValue)
            query = query.Where(mr => mr.DoctorId == doctorId.Value);

        if (fromDate.HasValue)
            query = query.Where(mr => mr.RecordDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(mr => mr.RecordDate <= toDate.Value);

        if (isFinalized.HasValue)
            query = query.Where(mr => mr.IsFinalized == isFinalized.Value);

        return await query
            .OrderByDescending(mr => mr.RecordDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<MedicalRecord>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
    {
        return await context.MedicalRecords
            .Where(mr => mr.RecordDate >= fromDate && mr.RecordDate <= toDate)
            .OrderByDescending(mr => mr.RecordDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<MedicalRecord>> GetByDiagnosisCodeAsync(string diagnosisCode)
    {
        return await context.MedicalRecords
            .Where(mr => mr.Diagnoses.Any(d => d.Code == diagnosisCode))
            .OrderByDescending(mr => mr.RecordDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> ExistsForAppointmentAsync(Guid appointmentId)
    {
        return await context.MedicalRecords
            .AnyAsync(mr => mr.AppointmentId == appointmentId);
    }

    public async Task AddAsync(MedicalRecord medicalRecord)
    {
        context.MedicalRecords.Add(medicalRecord);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MedicalRecord medicalRecord)
    {
        context.MedicalRecords.Update(medicalRecord);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var medicalRecord = await context.MedicalRecords.FindAsync(id);
        if (medicalRecord != null)
        {
            context.MedicalRecords.Remove(medicalRecord);
            await context.SaveChangesAsync();
        }
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }

    public async Task<bool> HasRelatedDataAsync(Guid id)
    {
        // A medical record is considered to have related data if:
        // - It has any diagnoses, prescriptions, vital signs, or treatment records (treatment records list not exposed here but part of entity)
        // - It is finalized (IsFinalized == true)
        var record = await context.MedicalRecords
            .Include(mr => mr.Diagnoses)
            .Include(mr => mr.Prescriptions)
            .Include(mr => mr.VitalSigns)
            .AsSplitQuery()
            .FirstOrDefaultAsync(mr => mr.Id == id);

        if (record == null)
            return false;

        var hasCollections = record.Diagnoses.Any() || record.Prescriptions.Any() || record.VitalSigns.Any();
        return record.IsFinalized || hasCollections;
    }
}
