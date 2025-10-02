using CareSync.Domain.Entities;
using CareSync.Domain.Interfaces;
using CareSync.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CareSync.Infrastructure.Repositories;

public class BillRepository : IBillRepository
{
    private readonly CareSyncDbContext _context;

    public BillRepository(CareSyncDbContext context)
    {
        _context = context;
    }

    public async Task<Bill?> GetByIdAsync(Guid id)
    {
        var bill = await _context.Bills
            .FirstOrDefaultAsync(b => b.Id == id);

        if (bill != null)
        {
            // Manually load related data since navigation properties are ignored
            var billItems = await _context.BillItems.Where(bi => bi.BillId == id).ToListAsync();
            var payments = await _context.Payments.Where(p => p.BillId == id).ToListAsync();
            var claims = await _context.InsuranceClaims.Where(ic => ic.BillId == id).ToListAsync();
        }

        return bill;
    }

    public async Task<List<Bill>> GetAllAsync()
    {
        return await _context.Bills
            .AsNoTracking()
            .OrderByDescending(b => b.BillDate)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _context.Bills.CountAsync();
    }

    public async Task<List<Bill>> GetByPatientIdAsync(Guid patientId)
    {
        return await _context.Bills
            .Where(b => b.PatientId == patientId)
            .OrderByDescending(b => b.BillDate)
            .ToListAsync();
    }

    public async Task<List<Bill>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Bills
            .Where(b => b.BillDate >= startDate && b.BillDate <= endDate)
            .OrderByDescending(b => b.BillDate)
            .ToListAsync();
    }

    public async Task<List<Bill>> GetOutstandingBillsAsync()
    {
        return await _context.Bills
            .Where(b => b.BalanceAmount > 0)
            .OrderByDescending(b => b.BillDate)
            .ToListAsync();
    }

    public async Task<List<Bill>> GetOverdueBillsAsync()
    {
        var today = DateTime.UtcNow.Date;
        return await _context.Bills
            .Where(b => b.DueDate < today && b.BalanceAmount > 0)
            .OrderBy(b => b.DueDate)
            .ToListAsync();
    }

    public async Task<Bill?> GetByBillNumberAsync(string billNumber)
    {
        var bill = await _context.Bills
            .FirstOrDefaultAsync(b => b.BillNumber == billNumber);

        if (bill != null)
        {
            // Manually load related data since navigation properties are ignored
            var billItems = await _context.BillItems.Where(bi => bi.BillId == bill.Id).ToListAsync();
            var payments = await _context.Payments.Where(p => p.BillId == bill.Id).ToListAsync();
            var claims = await _context.InsuranceClaims.Where(ic => ic.BillId == bill.Id).ToListAsync();
        }

        return bill;
    }

    public async Task AddAsync(Bill bill)
    {
        await _context.Bills.AddAsync(bill);
    }

    public async Task<bool> HasRelatedDataAsync(Guid id)
    {
        var hasPayments = await _context.Payments.AnyAsync(p => p.BillId == id);
        var hasClaims = await _context.InsuranceClaims.AnyAsync(c => c.BillId == id);
        return hasPayments || hasClaims;
    }

    public Task UpdateAsync(Bill bill)
    {
        _context.Bills.Update(bill);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var bill = await _context.Bills.FindAsync(id);
        if (bill != null) _context.Bills.Remove(bill);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
