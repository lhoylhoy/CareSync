using CareSync.Domain.Entities;
using CareSync.Domain.Interfaces;
using CareSync.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CareSync.Infrastructure.Repositories;

public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository
{
    private readonly CareSyncDbContext _context;

    public DoctorRepository(CareSyncDbContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<Doctor?> GetByIdAsync(Guid id)
    {
        return await _context.Doctors.FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Doctor?> GetByEmailAsync(string email)
    {
        return await _context.Doctors
            .FirstOrDefaultAsync(d => d.Email.Value == email);
    }

    public override async Task<IEnumerable<Doctor>> GetAllAsync() => await _context.Doctors.Where(d => d.IsActive).ToListAsync();

    public override async Task DeleteAsync(Guid id)
    {
        var doctor = await GetByIdAsync(id);
        if (doctor != null)
        {
            doctor.Deactivate();
            _context.Doctors.Update(doctor);
        }
    }

    public async Task<bool> HasRelatedDataAsync(Guid id)
    {
        var hasAppointments = await _context.Appointments.AnyAsync(a => a.DoctorId == id);
        var hasMedicalRecords = await _context.MedicalRecords.AnyAsync(m => m.DoctorId == id);
        return hasAppointments || hasMedicalRecords;
    }
}
