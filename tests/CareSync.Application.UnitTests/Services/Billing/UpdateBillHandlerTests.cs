using System.Threading;
using System.Threading.Tasks;
using CareSync.Application.Commands.Billing;
using CareSync.Application.Common.Mapping;
using CareSync.Application.DTOs.Billing;
using CareSync.Application.Common.Results;
using CareSync.Domain.Entities;
using CareSync.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace CareSync.Application.UnitTests.Services.Billing;

public class UpdateBillHandlerTests
{
    private readonly Mock<IBillRepository> _billRepo = new();
    private readonly BillMapper _mapper = new();

    [Fact]
    public async Task Handle_InvalidDiscountGreaterThanSubtotal_ReturnsFailure()
    {
        // Arrange
        var bill = new Bill(Guid.NewGuid(), Guid.NewGuid(), "BILL-TEST", DateTime.UtcNow.AddDays(10), 0m, 0m, null);
        // Add one item to establish subtotal
        var item = new BillItem(Guid.NewGuid(), bill.Id, "Service", 1, 50m, null, null);
        bill.AddItem(item);
        _billRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(bill);

        var dto = new UpdateBillDto(bill.Id, bill.PatientId, bill.DueDate, 0m, 100m, null); // discount 100 > subtotal 50
        var handler = new UpdateBillHandler(_billRepo.Object, _mapper);

        // Act
        var result = await handler.Handle(new UpdateBillCommand(dto), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Invalid discount amount");
        _billRepo.Verify(r => r.UpdateAsync(It.IsAny<Bill>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidDiscount_ReturnsSuccessAndUpdates()
    {
        // Arrange
        var bill = new Bill(Guid.NewGuid(), Guid.NewGuid(), "BILL-TEST", DateTime.UtcNow.AddDays(10), 0m, 0m, null);
        var item = new BillItem(Guid.NewGuid(), bill.Id, "Service", 2, 25m, null, null); // subtotal 50
        bill.AddItem(item);
        _billRepo.Setup(r => r.GetByIdAsync(bill.Id)).ReturnsAsync(bill);

        var dto = new UpdateBillDto(bill.Id, bill.PatientId, bill.DueDate, 0m, 10m, "promo");
        var handler = new UpdateBillHandler(_billRepo.Object, _mapper);

        // Act
        var result = await handler.Handle(new UpdateBillCommand(dto), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.DiscountAmount.Should().Be(10m);
        _billRepo.Verify(r => r.UpdateAsync(bill), Times.Once);
    }

    [Fact]
    public async Task Handle_TaxRateApplied_RecalculatesTaxAndTotals()
    {
        // Arrange: subtotal 100 (4 items * 25)
        var bill = new Bill(Guid.NewGuid(), Guid.NewGuid(), "BILL-TEST", DateTime.UtcNow.AddDays(5), 0m, 0m, null);
        var item = new BillItem(Guid.NewGuid(), bill.Id, "Service", 4, 25m, null, null);
        bill.AddItem(item);
        _billRepo.Setup(r => r.GetByIdAsync(bill.Id)).ReturnsAsync(bill);

        var dto = new UpdateBillDto(bill.Id, bill.PatientId, bill.DueDate, 0.12m, 0m, null); // 12% tax
        var handler = new UpdateBillHandler(_billRepo.Object, _mapper);

        // Act
        var result = await handler.Handle(new UpdateBillCommand(dto), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Subtotal.Should().Be(100m);
        result.Value!.TaxRate.Should().Be(0.12m);
        result.Value!.TaxAmount.Should().Be(12m);
        result.Value!.TotalAmount.Should().Be(112m);
    }
}
