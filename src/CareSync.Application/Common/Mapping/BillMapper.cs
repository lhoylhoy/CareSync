using CareSync.Application.DTOs.Billing;
using CareSync.Domain.Entities;

namespace CareSync.Application.Common.Mapping;

public sealed class BillMapper : IEntityMapper<Bill, BillDto>
{
    public BillDto Map(Bill bill)
    {
        // Flatten items/payments/claims
        var items = bill.Items.Select(i => new BillItemDto
        {
            Id = i.Id,
            Description = i.Description,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice,
            TotalPrice = i.TotalPrice,
            ServiceCode = i.ServiceCode,
            ServiceDate = i.ServiceDate
        }).ToList();

        var payments = bill.Payments.Select(p => new PaymentDto
        {
            Id = p.Id,
            Amount = p.Amount,
            Method = p.Method,
            Status = p.Status,
            PaymentDate = p.PaymentDate,
            ReferenceNumber = p.ReferenceNumber,
            Notes = p.Notes
        }).ToList();

        var claims = bill.Claims.Select(c => new InsuranceClaimDto
        {
            Id = c.Id,
            ClaimNumber = c.ClaimNumber,
            SubmissionDate = c.SubmissionDate,
            ClaimAmount = c.ClaimAmount,
            ApprovedAmount = c.ApprovedAmount,
            PaidAmount = c.PaidAmount,
            Status = c.Status,
            DenialReason = c.DenialReason,
            ProcessedDate = c.ProcessedDate,
            Notes = c.Notes
        }).ToList();

        return new BillDto
        {
            Id = bill.Id,
            PatientId = bill.PatientId,
            BillNumber = bill.BillNumber,
            BillDate = bill.BillDate,
            DueDate = bill.DueDate,
            Subtotal = bill.Subtotal,
            TaxAmount = bill.TaxAmount,
            TaxRate = bill.TaxRate,
            DiscountAmount = bill.DiscountAmount,
            TotalAmount = bill.TotalAmount,
            PaidAmount = bill.PaidAmount,
            BalanceAmount = bill.BalanceAmount,
            Status = bill.Status,
            Notes = bill.Notes,
            Items = items,
            Payments = payments,
            Claims = claims
            ,
            HasRelatedData = payments.Count > 0 || claims.Count > 0
        };
    }
}

public static class BillMapperExtensions
{
    public static BillDto ToDto(this Bill bill, BillMapper mapper) => mapper.Map(bill);
}
