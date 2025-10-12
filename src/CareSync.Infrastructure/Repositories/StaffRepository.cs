using CareSync.Domain.Entities;
using CareSync.Domain.Enums;
using CareSync.Domain.Interfaces;
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

    public async Task<(IReadOnlyList<Staff> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? searchTerm,
        IReadOnlyDictionary<string, string?> filters,
        CancellationToken cancellationToken = default)
    {
        if (page <= 0) page = CareSync.Application.Common.PagingDefaults.DefaultPage;
        if (pageSize <= 0) pageSize = CareSync.Application.Common.PagingDefaults.DefaultPageSize;
        pageSize = Math.Min(pageSize, CareSync.Application.Common.PagingDefaults.MaxPageSize);

        var query = _context.Staff
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            var likeTerm = $"%{term}%";
            query = query.Where(s =>
                EF.Functions.Like(s.Name.FirstName, likeTerm) ||
                EF.Functions.Like(s.Name.LastName, likeTerm) ||
                (s.Name.MiddleName != null && EF.Functions.Like(s.Name.MiddleName, likeTerm)) ||
                EF.Functions.Like(s.EmployeeId, likeTerm) ||
                EF.Functions.Like(s.Email.Value, likeTerm));
        }

        if (filters != null)
        {
            if (filters.TryGetValue("Role", out var roleValue) && !string.IsNullOrWhiteSpace(roleValue) &&
                Enum.TryParse<StaffRole>(roleValue, true, out var role))
            {
                query = query.Where(s => s.Role == role);
            }

            if (filters.TryGetValue("Department", out var departmentValue) && !string.IsNullOrWhiteSpace(departmentValue) &&
                Enum.TryParse<Department>(departmentValue, true, out var department))
            {
                query = query.Where(s => s.Department == department);
            }

            if (filters.TryGetValue("IsActive", out var isActiveValue) && !string.IsNullOrWhiteSpace(isActiveValue) &&
                bool.TryParse(isActiveValue, out var isActive))
            {
                query = query.Where(s => s.IsActive == isActive);
            }
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(s => s.Name.LastName)
            .ThenBy(s => s.Name.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
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
