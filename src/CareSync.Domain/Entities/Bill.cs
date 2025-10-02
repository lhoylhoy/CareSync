using CareSync.Domain.Enums;

namespace CareSync.Domain.Entities;

public class Bill
{
    private readonly List<BillItem> _billItems = new();
    private readonly List<InsuranceClaim> _insuranceClaims = new();
    private readonly List<Payment> _payments = new();

    private Bill() { }

    public Bill(Guid id, Guid patientId, string billNumber, Guid? appointmentId = null)
    {
        Id = id;
        PatientId = patientId;
        AppointmentId = appointmentId;
        BillNumber = billNumber ?? throw new ArgumentNullException(nameof(billNumber));
        BillDate = DateTime.UtcNow;
        DueDate = DateTime.UtcNow.AddDays(30); // Default 30 days
        Status = BillStatus.Draft;
    }

    public Bill(Guid id, Guid patientId, string billNumber, DateTime dueDate,
        decimal taxRate, decimal discountAmount, string? notes)
    {
        Id = id;
        PatientId = patientId;
        BillNumber = billNumber ?? throw new ArgumentNullException(nameof(billNumber));
        BillDate = DateTime.UtcNow;
        DueDate = dueDate;
        TaxRate = taxRate < 0 ? 0 : taxRate;
        DiscountAmount = discountAmount;
        Notes = notes;
        Status = BillStatus.Draft;
    }

    public Guid Id { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid? AppointmentId { get; private set; }
    public string BillNumber { get; private set; } = string.Empty;
    public DateTime BillDate { get; }
    public DateTime DueDate { get; private set; }
    public decimal SubTotal { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TaxRate { get; private set; } // Stored as a decimal fraction (e.g. 0.12 for 12%)
    public decimal DiscountAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal PaidAmount { get; private set; }
    public decimal BalanceAmount { get; private set; }
    public BillStatus Status { get; private set; }
    public string? Notes { get; private set; }

    public IReadOnlyCollection<BillItem> BillItems => _billItems.AsReadOnly();
    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();
    public IReadOnlyCollection<InsuranceClaim> InsuranceClaims => _insuranceClaims.AsReadOnly();

    // Aliases for compatibility
    public IReadOnlyCollection<BillItem> Items => _billItems.AsReadOnly();
    public IReadOnlyCollection<InsuranceClaim> Claims => _insuranceClaims.AsReadOnly();
    public decimal Subtotal => SubTotal;

    public bool IsOverdue => DateTime.UtcNow > DueDate && BalanceAmount > 0;
    public decimal CollectionRate => TotalAmount > 0 ? PaidAmount / TotalAmount * 100 : 0;

    public void AddBillItem(BillItem item)
    {
        if (Status != BillStatus.Draft)
            throw new InvalidOperationException("Cannot modify finalized bill");

        _billItems.Add(item);
        RecalculateAmounts();
    }

    // Alias for compatibility
    public void AddItem(BillItem item)
    {
        AddBillItem(item);
    }

    public void AddClaim(InsuranceClaim claim)
    {
        _insuranceClaims.Add(claim);
    }

    public void RemoveBillItem(Guid itemId)
    {
        if (Status != BillStatus.Draft)
            throw new InvalidOperationException("Cannot modify finalized bill");

        var item = _billItems.FirstOrDefault(i => i.Id == itemId);
        if (item is not null)
        {
            _billItems.Remove(item);
            RecalculateAmounts();
        }
    }

    public void SetDueDate(DateTime dueDate)
    {
        if (dueDate < BillDate)
            throw new ArgumentException("Due date cannot be before bill date", nameof(dueDate));

        DueDate = dueDate;
    }

    public void ApplyDiscount(decimal discountAmount, string? reason = null)
    {
        var result = TryApplyDiscount(discountAmount, reason);
        if (!result)
            throw new ArgumentException("Invalid discount amount", nameof(discountAmount));
    }

    // Non-throwing variant used by handlers to convert to Result instead of exception path
    public bool TryApplyDiscount(decimal discountAmount, string? reason = null)
    {
        if (discountAmount < 0) return false;
        if (SubTotal <= 0 && discountAmount > 0) return false; // cannot discount when no items
        if (discountAmount > SubTotal) return false;
        DiscountAmount = discountAmount;
        if (!string.IsNullOrWhiteSpace(reason)) Notes = reason;
        RecalculateAmounts();
        return true;
    }

    public bool SetTaxRate(decimal taxRate)
    {
        if (taxRate < 0) return false;
        TaxRate = taxRate;
        RecalculateAmounts();
        return true;
    }

    public void FinalizeBill()
    {
        if (_billItems.Count == 0)
            throw new InvalidOperationException("Cannot finalize bill without items");

        Status = BillStatus.Pending;
    }

    public void AddPayment(Payment payment)
    {
        _payments.Add(payment);
        PaidAmount += payment.Amount;
        BalanceAmount = TotalAmount - PaidAmount;

        if (BalanceAmount <= 0)
            Status = BillStatus.Paid;
        else if (PaidAmount > 0) Status = BillStatus.PartiallyPaid;
    }

    public void MarkAsOverdue()
    {
        if (DateTime.UtcNow > DueDate && BalanceAmount > 0) Status = BillStatus.Overdue;
    }

    public void Cancel(string reason)
    {
        Status = BillStatus.Cancelled;
        Notes = $"Cancelled: {reason}";
    }

    private void RecalculateAmounts()
    {
        SubTotal = _billItems.Sum(item => item.TotalPrice);
        TaxAmount = Math.Round(SubTotal * TaxRate, 2, MidpointRounding.AwayFromZero);
        TotalAmount = SubTotal + TaxAmount - DiscountAmount;
        if (TotalAmount < 0) TotalAmount = 0; // Safety clamp
        BalanceAmount = TotalAmount - PaidAmount;
    }
}
