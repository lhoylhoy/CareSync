using CareSync.Domain.Entities;
using CareSync.Domain.Enums;
using CareSync.Domain.Interfaces;
using CareSync.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CareSync.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly CareSyncDbContext _context;

    public PaymentRepository(CareSyncDbContext context)
    {
        _context = context;
    }

    public async Task<Payment?> GetByIdAsync(Guid id)
    {
        return await _context.Payments.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Payment>> GetByBillIdAsync(Guid billId)
    {
        return await _context.Payments
            .Where(p => p.BillId == billId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();
    }

    public async Task<List<Payment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Payments
            .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();
    }

    public async Task<List<Payment>> GetByPaymentMethodAsync(PaymentMethod method, DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var query = _context.Payments.Where(p => p.Method == method);

        if (startDate.HasValue)
            query = query.Where(p => p.PaymentDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(p => p.PaymentDate <= endDate.Value);

        return await query.OrderByDescending(p => p.PaymentDate).ToListAsync();
    }

    public async Task AddAsync(Payment payment)
    {
        await _context.Payments.AddAsync(payment);
    }

    public Task UpdateAsync(Payment payment)
    {
        _context.Payments.Update(payment);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var payment = await _context.Payments.FindAsync(id);
        if (payment != null) _context.Payments.Remove(payment);
    }
}
