using CareSync.Domain.Entities;
using CareSync.Domain.Enums;
using CareSync.Domain.Interfaces;
using CareSync.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CareSync.Infrastructure.Repositories;

public class InsuranceClaimRepository : IInsuranceClaimRepository
{
    private readonly CareSyncDbContext _context;

    public InsuranceClaimRepository(CareSyncDbContext context)
    {
        _context = context;
    }

    public async Task<InsuranceClaim?> GetByIdAsync(Guid id)
    {
        return await _context.InsuranceClaims.FirstOrDefaultAsync(ic => ic.Id == id);
    }

    public async Task<List<InsuranceClaim>> GetByBillIdAsync(Guid billId)
    {
        return await _context.InsuranceClaims
            .Where(ic => ic.BillId == billId)
            .OrderByDescending(ic => ic.SubmissionDate)
            .ToListAsync();
    }

    public async Task<List<InsuranceClaim>> GetByPatientInsuranceIdAsync(Guid patientInsuranceId)
    {
        return await _context.InsuranceClaims
            .Where(ic => ic.PatientInsuranceId == patientInsuranceId)
            .OrderByDescending(ic => ic.SubmissionDate)
            .ToListAsync();
    }

    public async Task<List<InsuranceClaim>> GetByStatusAsync(ClaimStatus status)
    {
        return await _context.InsuranceClaims
            .Where(ic => ic.Status == status)
            .OrderByDescending(ic => ic.SubmissionDate)
            .ToListAsync();
    }

    public async Task<List<InsuranceClaim>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.InsuranceClaims
            .Where(ic => ic.SubmissionDate >= startDate && ic.SubmissionDate <= endDate)
            .OrderByDescending(ic => ic.SubmissionDate)
            .ToListAsync();
    }

    public async Task<InsuranceClaim?> GetByClaimNumberAsync(string claimNumber)
    {
        return await _context.InsuranceClaims
            .FirstOrDefaultAsync(ic => ic.ClaimNumber == claimNumber);
    }

    public async Task AddAsync(InsuranceClaim claim)
    {
        await _context.InsuranceClaims.AddAsync(claim);
    }

    public Task UpdateAsync(InsuranceClaim claim)
    {
        _context.InsuranceClaims.Update(claim);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var claim = await _context.InsuranceClaims.FindAsync(id);
        if (claim != null) _context.InsuranceClaims.Remove(claim);
    }
}
