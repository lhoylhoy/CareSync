using CareSync.Domain.Entities;
using CareSync.Domain.Interfaces;
using CareSync.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CareSync.Infrastructure.Repositories;

public class PatientRepository : GenericRepository<Patient>, IPatientRepository
{
    private readonly CareSyncDbContext _context;

    public PatientRepository(CareSyncDbContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<Patient?> GetByIdAsync(Guid id)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == id);

        return patient;
    }

    public async Task<Patient?> GetByEmailAsync(string email)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.Email != null && p.Email.Value == email);

        return patient;
    }

    public override async Task<IEnumerable<Patient>> GetAllAsync() => await _context.Patients.AsNoTracking().ToListAsync();

    public override Task UpdateAsync(Patient patient) { _context.Patients.Update(patient); return Task.CompletedTask; }

    public async Task<bool> HasRelatedDataAsync(Guid id)
    {
        var hasAppointments = await _context.Appointments.AnyAsync(a => a.PatientId == id);
        var hasMedicalRecords = await _context.MedicalRecords.AnyAsync(m => m.PatientId == id);
        var hasBills = await _context.Bills.AnyAsync(b => b.PatientId == id);
        return hasAppointments || hasMedicalRecords || hasBills;
    }
}
