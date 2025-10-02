using CareSync.Domain.Enums;

namespace CareSync.Domain.Entities;

public class Payment
{
    private Payment() { }

    public Payment(Guid id, Guid billId, decimal amount, PaymentMethod method, string? referenceNumber = null)
    {
        Id = id;
        BillId = billId;
        Amount = amount > 0 ? amount : throw new ArgumentException("Payment amount must be positive", nameof(amount));
        PaymentDate = DateTime.UtcNow;
        Method = method;
        ReferenceNumber = referenceNumber;
        Status = PaymentStatus.Pending;
    }

    public Payment(Guid id, Guid billId, decimal amount, PaymentMethod method,
        string? referenceNumber, string? notes)
    {
        Id = id;
        BillId = billId;
        Amount = amount > 0 ? amount : throw new ArgumentException("Payment amount must be positive", nameof(amount));
        PaymentDate = DateTime.UtcNow;
        Method = method;
        ReferenceNumber = referenceNumber;
        Notes = notes;
        Status = PaymentStatus.Pending;
    }

    public Guid Id { get; private set; }
    public Guid BillId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime PaymentDate { get; private set; }
    public PaymentMethod Method { get; private set; }
    public string? ReferenceNumber { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? Notes { get; private set; }

    public void ProcessPayment()
    {
        if (Status == PaymentStatus.Completed)
            throw new InvalidOperationException("Payment is already processed");

        Status = PaymentStatus.Completed;
    }

    public void AddNotes(string notes)
    {
        Notes = notes;
    }

    public void MarkAsRefunded(string reason)
    {
        Status = PaymentStatus.Refunded;
        Notes = $"Refunded: {reason}";
    }

    public void MarkAsFailed(string reason)
    {
        Status = PaymentStatus.Failed;
        Notes = $"Failed: {reason}";
    }
}
