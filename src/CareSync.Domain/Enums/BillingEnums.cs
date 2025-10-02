namespace CareSync.Domain.Enums;

public enum BillStatus
{
    Draft = 1,
    Pending = 2,
    Paid = 3,
    PartiallyPaid = 4,
    Overdue = 5,
    Cancelled = 6
}

public enum PaymentMethod
{
    Cash = 1,
    CreditCard = 2,
    DebitCard = 3,
    Check = 4,
    BankTransfer = 5,
    Insurance = 6,
    Online = 7,
    OnlinePayment = 8
}

public enum PaymentStatus
{
    Pending = 1,
    Completed = 2,
    Failed = 3,
    Refunded = 4,
    Cancelled = 5
}
