namespace CareSync.Domain.Entities;

public class BillItem
{
    private BillItem() { }

    public BillItem(Guid id, string serviceCode, string description, int quantity, decimal unitPrice,
        string? notes = null)
    {
        Id = id;
        ServiceCode = serviceCode ?? throw new ArgumentNullException(nameof(serviceCode));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Quantity = quantity > 0 ? quantity : throw new ArgumentException("Quantity must be positive", nameof(quantity));
        UnitPrice = unitPrice >= 0
            ? unitPrice
            : throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));
        TotalPrice = quantity * unitPrice;
        Notes = notes;
    }

    public BillItem(Guid id, Guid billId, string description, int quantity, decimal unitPrice,
        string? serviceCode, DateTime? serviceDate)
    {
        Id = id;
        BillId = billId;
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Quantity = quantity > 0 ? quantity : throw new ArgumentException("Quantity must be positive", nameof(quantity));
        UnitPrice = unitPrice >= 0
            ? unitPrice
            : throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));
        TotalPrice = quantity * unitPrice;
        ServiceCode = serviceCode ?? string.Empty;
        ServiceDate = serviceDate;
    }

    public Guid Id { get; private set; }
    public Guid BillId { get; private set; }
    public string ServiceCode { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice { get; private set; }
    public DateTime? ServiceDate { get; private set; }
    public string? Notes { get; private set; }

    public void UpdateQuantity(int quantity)
    {
        Quantity = quantity > 0 ? quantity : throw new ArgumentException("Quantity must be positive", nameof(quantity));
        TotalPrice = Quantity * UnitPrice;
    }

    public void UpdateUnitPrice(decimal unitPrice)
    {
        UnitPrice = unitPrice >= 0
            ? unitPrice
            : throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));
        TotalPrice = Quantity * UnitPrice;
    }
}
