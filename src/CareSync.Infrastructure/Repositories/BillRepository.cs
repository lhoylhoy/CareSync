using CareSync.Domain.Entities;
using CareSync.Domain.Enums;
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

    public async Task<(IReadOnlyList<Bill> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? searchTerm,
        IReadOnlyDictionary<string, string?> filters,
        CancellationToken cancellationToken = default)
    {
        if (page <= 0) page = CareSync.Application.Common.PagingDefaults.DefaultPage;
        if (pageSize <= 0) pageSize = CareSync.Application.Common.PagingDefaults.DefaultPageSize;
        pageSize = Math.Min(pageSize, CareSync.Application.Common.PagingDefaults.MaxPageSize);

        var query = _context.Bills
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            var likeTerm = $"%{term}%";

            query = query.Where(b =>
                EF.Functions.Like(b.BillNumber, likeTerm) ||
                (b.Notes != null && EF.Functions.Like(b.Notes, likeTerm)) ||
                _context.Patients.Any(p =>
                    p.Id == b.PatientId &&
                    (EF.Functions.Like(p.FullName.FirstName, likeTerm) ||
                     EF.Functions.Like(p.FullName.LastName, likeTerm) ||
                     (p.FullName.MiddleName != null && EF.Functions.Like(p.FullName.MiddleName, likeTerm)))) ||
                _context.Payments.Any(p =>
                    p.BillId == b.Id &&
                    p.ReferenceNumber != null &&
                    EF.Functions.Like(p.ReferenceNumber, likeTerm)) ||
                _context.InsuranceClaims.Any(ic =>
                    ic.BillId == b.Id &&
                    EF.Functions.Like(ic.ClaimNumber, likeTerm)));
        }

        if (filters is { Count: > 0 })
        {
            if (filters.TryGetValue("Status", out var statusValue) &&
                !string.IsNullOrWhiteSpace(statusValue) &&
                Enum.TryParse<BillStatus>(statusValue, true, out var status))
            {
                query = query.Where(b => b.Status == status);
            }

            if (filters.TryGetValue("PatientId", out var patientValue) &&
                Guid.TryParse(patientValue, out var patientId))
            {
                query = query.Where(b => b.PatientId == patientId);
            }

            if (filters.TryGetValue("BillDateFrom", out var billDateFromValue) &&
                DateTime.TryParse(billDateFromValue, out var billDateFrom))
            {
                query = query.Where(b => b.BillDate >= billDateFrom);
            }

            if (filters.TryGetValue("BillDateTo", out var billDateToValue) &&
                DateTime.TryParse(billDateToValue, out var billDateTo))
            {
                query = query.Where(b => b.BillDate <= billDateTo);
            }

            if (filters.TryGetValue("DueDateFrom", out var dueDateFromValue) &&
                DateTime.TryParse(dueDateFromValue, out var dueDateFrom))
            {
                query = query.Where(b => b.DueDate >= dueDateFrom);
            }

            if (filters.TryGetValue("DueDateTo", out var dueDateToValue) &&
                DateTime.TryParse(dueDateToValue, out var dueDateTo))
            {
                query = query.Where(b => b.DueDate <= dueDateTo);
            }

            if (filters.TryGetValue("HasBalance", out var hasBalanceValue) &&
                bool.TryParse(hasBalanceValue, out var hasBalance))
            {
                query = hasBalance
                    ? query.Where(b => b.BalanceAmount > 0)
                    : query.Where(b => b.BalanceAmount <= 0);
            }

            if (filters.TryGetValue("IsOverdue", out var isOverdueValue) &&
                bool.TryParse(isOverdueValue, out var isOverdue))
            {
                var today = DateTime.UtcNow.Date;
                query = isOverdue
                    ? query.Where(b => b.DueDate.Date < today && b.BalanceAmount > 0)
                    : query.Where(b => b.DueDate.Date >= today || b.BalanceAmount <= 0);
            }
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(b => b.BillDate)
            .ThenBy(b => b.BillNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
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
