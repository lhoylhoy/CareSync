// BillFormDto moved to Web.Admin project.
using CareSync.Domain.Enums;

namespace CareSync.Application.DTOs.Billing;

public record BillDto
{
    public Guid Id { get; init; }
    public Guid PatientId { get; init; }
    public string BillNumber { get; init; } = string.Empty;
    public DateTime BillDate { get; init; }
    public DateTime DueDate { get; init; }
    public decimal Subtotal { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal TaxRate { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal PaidAmount { get; init; }
    public decimal BalanceAmount { get; init; }
    public BillStatus Status { get; init; }
    public string? Notes { get; init; }
    public List<BillItemDto> Items { get; init; } = new();
    public List<PaymentDto> Payments { get; init; } = new();
    public List<InsuranceClaimDto> Claims { get; init; } = new();
    public bool HasRelatedData { get; init; } = false; // Payments or Claims present
}

public record CreateBillDto
{
    public Guid PatientId { get; init; }
    public DateTime DueDate { get; init; }
    public decimal TaxRate { get; init; }
    public decimal DiscountAmount { get; init; }
    public string? Notes { get; init; }
    public List<CreateBillItemDto> Items { get; init; } = new();
}

public record UpdateBillDto(
    Guid Id,
    Guid PatientId,
    DateTime DueDate,
    decimal TaxRate,
    decimal DiscountAmount,
    string? Notes
);

public record UpsertBillDto(
    Guid? Id,
    Guid PatientId,
    DateTime DueDate,
    decimal TaxRate,
    decimal DiscountAmount,
    string? Notes
);

// BillFormDto relocated to Web.Admin (see Forms/BillFormDto.cs)

public record BillItemDto
{
    public Guid Id { get; init; }
    public string Description { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TotalPrice { get; init; }
    public string? ServiceCode { get; init; }
    public DateTime? ServiceDate { get; init; }
}

public record CreateBillItemDto
{
    public string Description { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public string? ServiceCode { get; init; }
    public DateTime? ServiceDate { get; init; }
}

public record PaymentDto
{
    public Guid Id { get; init; }
    public decimal Amount { get; init; }
    public PaymentMethod Method { get; init; }
    public PaymentStatus Status { get; init; }
    public DateTime PaymentDate { get; init; }
    public string? ReferenceNumber { get; init; }
    public string? Notes { get; init; }
}

public record ProcessPaymentDto
{
    public Guid BillId { get; init; }
    public decimal Amount { get; init; }
    public PaymentMethod Method { get; init; }
    public string? ReferenceNumber { get; init; }
    public string? Notes { get; init; }
}

public record CreateInsuranceClaimDto
{
    public Guid BillId { get; init; }
    public Guid PatientInsuranceId { get; init; }
    public string ClaimNumber { get; init; } = string.Empty;
    public decimal ClaimAmount { get; init; }
}

public record InsuranceClaimDto
{
    public Guid Id { get; init; }
    public string ClaimNumber { get; init; } = string.Empty;
    public DateTime SubmissionDate { get; init; }
    public decimal ClaimAmount { get; init; }
    public decimal ApprovedAmount { get; init; }
    public decimal PaidAmount { get; init; }
    public ClaimStatus Status { get; init; }
    public string? DenialReason { get; init; }
    public DateTime? ProcessedDate { get; init; }
    public string? Notes { get; init; }
}

public record BillSummaryDto
{
    public int TotalBills { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal PaidAmount { get; init; }
    public decimal PendingAmount { get; init; }
    public decimal OverdueAmount { get; init; }
    public int OverdueBills { get; init; }
    public decimal CollectionRate { get; init; }
}

public record PaymentSummaryDto
{
    public int TotalPayments { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal CashPayments { get; init; }
    public decimal CardPayments { get; init; }
    public decimal InsurancePayments { get; init; }
    public decimal OnlinePayments { get; init; }
}
