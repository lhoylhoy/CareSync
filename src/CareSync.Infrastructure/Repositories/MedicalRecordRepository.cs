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
            .ToListAsync();
    }

    public async Task<List<MedicalRecord>> GetByPatientIdAsync(Guid patientId)
    {
        return await context.MedicalRecords
            .Where(mr => mr.PatientId == patientId)
            .OrderByDescending(mr => mr.RecordDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<MedicalRecord>> GetByDoctorIdAsync(Guid doctorId)
    {
        return await context.MedicalRecords
            .Where(mr => mr.DoctorId == doctorId)
            .OrderByDescending(mr => mr.RecordDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<MedicalRecord>> GetByAppointmentIdAsync(Guid appointmentId)
    {
        return await context.MedicalRecords
            .Where(mr => mr.AppointmentId == appointmentId)
            .OrderByDescending(mr => mr.RecordDate)
            .AsNoTracking()
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
