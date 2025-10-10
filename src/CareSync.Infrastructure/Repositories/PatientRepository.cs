using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

    public async Task<(IReadOnlyList<Patient> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? searchTerm,
        IReadOnlyDictionary<string, string?> filters,
        CancellationToken cancellationToken = default)
    {
        if (page <= 0) page = CareSync.Application.Common.PagingDefaults.DefaultPage;
        if (pageSize <= 0) pageSize = CareSync.Application.Common.PagingDefaults.DefaultPageSize;
        pageSize = Math.Min(pageSize, CareSync.Application.Common.PagingDefaults.MaxPageSize);

        var query = _context.Patients
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            var likeTerm = $"%{term}%";

            query = query.Where(p =>
                EF.Functions.Like(p.FullName.FirstName, likeTerm) ||
                EF.Functions.Like(p.FullName.LastName, likeTerm) ||
                (p.FullName.MiddleName != null && EF.Functions.Like(p.FullName.MiddleName, likeTerm)) ||
                (p.Email != null && EF.Functions.Like(p.Email.Value, likeTerm)) ||
                (p.PhoneNumber != null && EF.Functions.Like(p.PhoneNumber.Number, likeTerm)) ||
                (p.Street != null && EF.Functions.Like(p.Street, likeTerm)) ||
                EF.Functions.Like(p.ProvinceName, likeTerm) ||
                EF.Functions.Like(p.CityName, likeTerm) ||
                EF.Functions.Like(p.BarangayName, likeTerm) ||
                (p.PhilHealthNumber != null && EF.Functions.Like(p.PhilHealthNumber, likeTerm)) ||
                (p.SssNumber != null && EF.Functions.Like(p.SssNumber, likeTerm)) ||
                (p.Tin != null && EF.Functions.Like(p.Tin, likeTerm)));
        }

        if (filters is { Count: > 0 })
        {
            if (filters.TryGetValue("Gender", out var genderValue) &&
                !string.IsNullOrWhiteSpace(genderValue))
            {
                query = query.Where(p => p.Gender == genderValue);
            }

            if (filters.TryGetValue("ProvinceCode", out var provinceCodeValue) &&
                !string.IsNullOrWhiteSpace(provinceCodeValue))
            {
                query = query.Where(p => p.ProvinceCode == provinceCodeValue);
            }

            if (filters.TryGetValue("CityCode", out var cityCodeValue) &&
                !string.IsNullOrWhiteSpace(cityCodeValue))
            {
                query = query.Where(p => p.CityCode == cityCodeValue);
            }

            if (filters.TryGetValue("BarangayCode", out var barangayCodeValue) &&
                !string.IsNullOrWhiteSpace(barangayCodeValue))
            {
                query = query.Where(p => p.BarangayCode == barangayCodeValue);
            }

            if (filters.TryGetValue("IsActive", out var isActiveValue) &&
                bool.TryParse(isActiveValue, out var isActive))
            {
                query = query.Where(p => p.IsActive == isActive);
            }

            if (filters.TryGetValue("BirthDateFrom", out var birthDateFromValue) &&
                DateTime.TryParse(birthDateFromValue, out var birthDateFrom))
            {
                query = query.Where(p => p.DateOfBirth >= birthDateFrom);
            }

            if (filters.TryGetValue("BirthDateTo", out var birthDateToValue) &&
                DateTime.TryParse(birthDateToValue, out var birthDateTo))
            {
                query = query.Where(p => p.DateOfBirth <= birthDateTo);
            }

            if (filters.TryGetValue("HasEmergencyContact", out var hasEmergencyContactValue) &&
                bool.TryParse(hasEmergencyContactValue, out var hasEmergencyContact))
            {
                query = hasEmergencyContact
                    ? query.Where(p => !string.IsNullOrWhiteSpace(p.EmergencyContactName))
                    : query.Where(p => string.IsNullOrWhiteSpace(p.EmergencyContactName));
            }
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.FullName.LastName)
            .ThenBy(p => p.FullName.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public override Task UpdateAsync(Patient patient) { _context.Patients.Update(patient); return Task.CompletedTask; }

    public async Task<bool> HasRelatedDataAsync(Guid id)
    {
        var hasAppointments = await _context.Appointments.AnyAsync(a => a.PatientId == id);
        var hasMedicalRecords = await _context.MedicalRecords.AnyAsync(m => m.PatientId == id);
        var hasBills = await _context.Bills.AnyAsync(b => b.PatientId == id);
        return hasAppointments || hasMedicalRecords || hasBills;
    }
}
