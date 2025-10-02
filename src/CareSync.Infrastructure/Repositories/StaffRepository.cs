using CareSync.Domain.Entities;
using CareSync.Domain.Interfaces;
using CareSync.Domain.Enums;
using CareSync.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CareSync.Infrastructure.Repositories;

public class StaffRepository : IStaffRepository
{
    private readonly CareSyncDbContext _context;

    public StaffRepository(CareSyncDbContext context)
    {
        _context = context;
    }

    public async Task<Staff?> GetByIdAsync(Guid id)
    {
        return await _context.Staff.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Staff>> GetAllAsync()
    {
        return await _context.Staff.ToListAsync();
    }

    public async Task<List<Staff>> GetByRoleAsync(StaffRole role)
    {
        return await _context.Staff.Where(s => s.Role == role).ToListAsync();
    }

    public async Task<List<Staff>> GetByDepartmentAsync(Department department)
    {
        return await _context.Staff.Where(s => s.Department == department).ToListAsync();
    }

    public async Task<List<Staff>> GetActiveStaffAsync()
    {
        return await _context.Staff.Where(s => s.IsActive).ToListAsync();
    }

    public async Task<Staff?> GetByEmailAsync(string email)
    {
        return await _context.Staff.FirstOrDefaultAsync(s => s.Email.Value == email);
    }

    public async Task<Staff?> GetByEmployeeNumberAsync(string employeeNumber)
    {
        return await _context.Staff.FirstOrDefaultAsync(s => s.EmployeeId == employeeNumber);
    }

    public async Task<bool> HasRelatedDataAsync(Guid id)
    {
        // Current domain: Staff may overlap with Doctors (shared identity) or be unrelated.
        // We consider related data if this staff member ALSO exists as a doctor with appointments or medical records.
        var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == id);
        if (doctor == null)
            return false; // No doctor-linked data

        var hasAppointments = await _context.Appointments.AnyAsync(a => a.DoctorId == id);
        var hasMedicalRecords = await _context.MedicalRecords.AnyAsync(m => m.DoctorId == id);
        return hasAppointments || hasMedicalRecords;
    }

    public async Task AddAsync(Staff staff)
    {
        await _context.Staff.AddAsync(staff);
    }

    public Task UpdateAsync(Staff staff)
    {
        _context.Staff.Update(staff);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var staff = await GetByIdAsync(id);
        if (staff != null)
        {
            _context.Staff.Remove(staff);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
