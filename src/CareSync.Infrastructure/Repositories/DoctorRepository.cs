using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

    public async Task<(IReadOnlyList<Doctor> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? searchTerm,
        IReadOnlyDictionary<string, string?> filters,
        CancellationToken cancellationToken = default)
    {
        if (page <= 0) page = CareSync.Application.Common.PagingDefaults.DefaultPage;
        if (pageSize <= 0) pageSize = CareSync.Application.Common.PagingDefaults.DefaultPageSize;
        pageSize = Math.Min(pageSize, CareSync.Application.Common.PagingDefaults.MaxPageSize);

        var query = _context.Doctors
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            var likeTerm = $"%{term}%";
            query = query.Where(d =>
                EF.Functions.Like(d.Name.FirstName, likeTerm) ||
                EF.Functions.Like(d.Name.LastName, likeTerm) ||
                (d.Name.MiddleName != null && EF.Functions.Like(d.Name.MiddleName, likeTerm)) ||
                EF.Functions.Like(d.Specialty, likeTerm) ||
                EF.Functions.Like(d.PhoneNumber.Number, likeTerm) ||
                EF.Functions.Like(d.Email.Value, likeTerm));
        }

        if (filters != null && filters.Count > 0)
        {
            if (filters.TryGetValue("Specialty", out var specialtyValue) && !string.IsNullOrWhiteSpace(specialtyValue))
            {
                query = query.Where(d => d.Specialty == specialtyValue);
            }

            if (filters.TryGetValue("IsActive", out var isActiveValue) && !string.IsNullOrWhiteSpace(isActiveValue) &&
                bool.TryParse(isActiveValue, out var isActive))
            {
                query = query.Where(d => d.IsActive == isActive);
            }
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(d => d.Name.LastName)
            .ThenBy(d => d.Name.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

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
