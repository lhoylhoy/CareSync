using System;
using CareSync.Application.Common.Mapping;
using CareSync.Domain.Entities;
using Xunit;

namespace CareSync.Application.UnitTests.Mapping;

public class BillMapperTests
{
    private readonly BillMapper _mapper = new();

    [Fact]
    public void Map_Should_Flatten_Collections()
    {
        var bill = new Bill(Guid.NewGuid(), Guid.NewGuid(), "BILL-001");
        // Add item
        var item = new BillItem(Guid.NewGuid(), bill.Id, "Consultation", 1, 500m, "CONS", DateTime.UtcNow);
        bill.AddItem(item);
        // Add payment
        var payment = new Payment(Guid.NewGuid(), bill.Id, 500m, CareSync.Domain.Enums.PaymentMethod.Cash, referenceNumber: "REF1");
        bill.AddPayment(payment);
        var dto = _mapper.Map(bill);
        Assert.Single(dto.Items);
        Assert.Single(dto.Payments);
        Assert.Equal(500m, dto.Subtotal);
    }
}
