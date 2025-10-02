using CareSync.Domain.Entities;
using CareSync.Domain.Interfaces;
using CareSync.Application.DTOs.Billing;
using CareSync.Application.Common.Mapping;
using MediatR;
using CareSync.Application.Common.Results;

namespace CareSync.Application.Commands.Billing;

public record CreateBillCommand(CreateBillDto Bill) : IRequest<Result<Guid>>;

public record UpdateBillCommand(UpdateBillDto Bill) : IRequest<Result<BillDto>>;
public record UpsertBillCommand(UpsertBillDto Bill) : IRequest<Result<BillDto>>;

public class CreateBillHandler : IRequestHandler<CreateBillCommand, Result<Guid>>
{
    private readonly IBillRepository _billRepository;
    private readonly IUnitOfWork _uow;

    public CreateBillHandler(IBillRepository billRepository, IUnitOfWork uow)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
    }

    public async Task<Result<Guid>> Handle(CreateBillCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Bill;
        var billNumber = GenerateBillNumber();

        var bill = new Bill(
            Guid.NewGuid(),
            dto.PatientId,
            billNumber,
            dto.DueDate,
            dto.TaxRate,
            dto.DiscountAmount,
            dto.Notes
        );

        foreach (var itemDto in dto.Items)
        {
            var billItem = new BillItem(
                Guid.NewGuid(),
                bill.Id,
                itemDto.Description,
                itemDto.Quantity,
                itemDto.UnitPrice,
                itemDto.ServiceCode,
                itemDto.ServiceDate
            );

            bill.AddItem(billItem);
        }

        await _billRepository.AddAsync(bill);
        await _uow.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(bill.Id);
    }

    private static string GenerateBillNumber()
    {
        return $"BILL-{DateTime.UtcNow:yyyyMM}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }
}

public record ProcessPaymentCommand(ProcessPaymentDto Payment) : IRequest<Result<Guid>>;

public class ProcessPaymentHandler : IRequestHandler<ProcessPaymentCommand, Result<Guid>>
{
    private readonly IBillRepository _billRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _uow;

    public ProcessPaymentHandler(IBillRepository billRepository, IPaymentRepository paymentRepository, IUnitOfWork uow)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
    }

    public async Task<Result<Guid>> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Payment;
        var bill = await _billRepository.GetByIdAsync(dto.BillId);
        if (bill is null)
            return Result<Guid>.Failure($"Bill with ID {dto.BillId} not found");

        var payment = new Payment(
            Guid.NewGuid(),
            dto.BillId,
            dto.Amount,
            dto.Method,
            dto.ReferenceNumber,
            dto.Notes
        );

        // Process the payment
        payment.ProcessPayment();
        bill.AddPayment(payment);

        await _paymentRepository.AddAsync(payment);
        await _billRepository.UpdateAsync(bill);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(payment.Id);
    }
}

public record CreateInsuranceClaimCommand(CreateInsuranceClaimDto Claim) : IRequest<Result<Guid>>;

public class CreateInsuranceClaimHandler : IRequestHandler<CreateInsuranceClaimCommand, Result<Guid>>
{
    private readonly IBillRepository _billRepository;
    private readonly IInsuranceClaimRepository _claimRepository;
    private readonly IUnitOfWork _uow;

    public CreateInsuranceClaimHandler(IInsuranceClaimRepository claimRepository, IBillRepository billRepository, IUnitOfWork uow)
    {
        _claimRepository = claimRepository ?? throw new ArgumentNullException(nameof(claimRepository));
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
    }

    public async Task<Result<Guid>> Handle(CreateInsuranceClaimCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Claim;
        var bill = await _billRepository.GetByIdAsync(dto.BillId);
        if (bill is null)
            return Result<Guid>.Failure($"Bill with ID {dto.BillId} not found");

        var claim = new InsuranceClaim(
            Guid.NewGuid(),
            dto.BillId,
            dto.PatientInsuranceId,
            dto.ClaimNumber,
            dto.ClaimAmount
        );

        bill.AddClaim(claim);

        await _claimRepository.AddAsync(claim);
        await _billRepository.UpdateAsync(bill);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(claim.Id);
    }
}

public record ApproveClaimCommand(Guid ClaimId, decimal ApprovedAmount, decimal PaidAmount) : IRequest<Result<Unit>>;

public class ApproveClaimHandler : IRequestHandler<ApproveClaimCommand, Result<Unit>>
{
    private readonly IInsuranceClaimRepository _claimRepository;
    private readonly IUnitOfWork _uow;

    public ApproveClaimHandler(IInsuranceClaimRepository claimRepository, IUnitOfWork uow)
    {
        _claimRepository = claimRepository ?? throw new ArgumentNullException(nameof(claimRepository));
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
    }

    public async Task<Result<Unit>> Handle(ApproveClaimCommand request, CancellationToken cancellationToken)
    {
        var claim = await _claimRepository.GetByIdAsync(request.ClaimId);
        if (claim is null)
            return Result<Unit>.Failure($"Claim with ID {request.ClaimId} not found");

        claim.Approve(request.ApprovedAmount, request.PaidAmount);
        await _claimRepository.UpdateAsync(claim);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}

public record DenyClaimCommand(Guid ClaimId, string Reason) : IRequest<Result<Unit>>;

public class DenyClaimHandler : IRequestHandler<DenyClaimCommand, Result<Unit>>
{
    private readonly IInsuranceClaimRepository _claimRepository;
    private readonly IUnitOfWork _uow;

    public DenyClaimHandler(IInsuranceClaimRepository claimRepository, IUnitOfWork uow)
    {
        _claimRepository = claimRepository ?? throw new ArgumentNullException(nameof(claimRepository));
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
    }

    public async Task<Result<Unit>> Handle(DenyClaimCommand request, CancellationToken cancellationToken)
    {
        var claim = await _claimRepository.GetByIdAsync(request.ClaimId);
        if (claim is null)
            return Result<Unit>.Failure($"Claim with ID {request.ClaimId} not found");

        claim.Deny(request.Reason);
        await _claimRepository.UpdateAsync(claim);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}

public record DeleteBillCommand(Guid BillId) : IRequest<Result<Unit>>;

public class DeleteBillHandler : IRequestHandler<DeleteBillCommand, Result<Unit>>
{
    private readonly IBillRepository _billRepository;
    private readonly IUnitOfWork _uow;

    public DeleteBillHandler(IBillRepository billRepository, IUnitOfWork uow)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
    }

    public async Task<Result<Unit>> Handle(DeleteBillCommand request, CancellationToken cancellationToken)
    {
        var bill = await _billRepository.GetByIdAsync(request.BillId);
        if (bill is null)
            return Result<Unit>.Failure($"Bill with ID {request.BillId} not found.");
        if (await _billRepository.HasRelatedDataAsync(request.BillId))
            return Result<Unit>.Failure("Cannot delete bill: payments or insurance claims exist.");
        await _billRepository.DeleteAsync(request.BillId);
        await _uow.SaveChangesAsync(cancellationToken);
        return Result<Unit>.Success(Unit.Value);
    }
}

public class UpdateBillHandler : IRequestHandler<UpdateBillCommand, Result<BillDto>>
{
    private readonly IBillRepository _billRepository;
    private readonly BillMapper _mapper;
    private readonly IUnitOfWork _uow;

    public UpdateBillHandler(IBillRepository billRepository, BillMapper mapper, IUnitOfWork uow)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
    }

    public async Task<Result<BillDto>> Handle(UpdateBillCommand request, CancellationToken cancellationToken)
    {
        var existingBill = await _billRepository.GetByIdAsync(request.Bill.Id);
        if (existingBill == null)
            return Result<BillDto>.Failure($"Bill with ID {request.Bill.Id} not found.");

        // Update bill properties using available methods
        existingBill.SetDueDate(request.Bill.DueDate);
        existingBill.SetTaxRate(request.Bill.TaxRate);

        var discount = request.Bill.DiscountAmount;
        if (!existingBill.TryApplyDiscount(discount, request.Bill.Notes))
            return Result<BillDto>.Failure("Invalid discount amount");

        await _billRepository.UpdateAsync(existingBill);
        await _uow.SaveChangesAsync(cancellationToken);

        // Return updated bill as DTO
        return Result<BillDto>.Success(_mapper.Map(existingBill));
    }
}

public class UpsertBillHandler : IRequestHandler<UpsertBillCommand, Result<BillDto>>
{
    private readonly IBillRepository _billRepository;
    private readonly BillMapper _mapper;
    private readonly IUnitOfWork _uow;

    public UpsertBillHandler(IBillRepository billRepository, BillMapper mapper, IUnitOfWork uow)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
    }

    public async Task<Result<BillDto>> Handle(UpsertBillCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Bill;

        if (dto.Id.HasValue)
        {
            // Update path
            var existing = await _billRepository.GetByIdAsync(dto.Id.Value);
            if (existing is null)
                return Result<BillDto>.Failure($"Bill with ID {dto.Id} not found.");
            existing.SetDueDate(dto.DueDate);
            existing.SetTaxRate(dto.TaxRate);
            if (!existing.TryApplyDiscount(dto.DiscountAmount, dto.Notes))
                return Result<BillDto>.Failure("Invalid discount amount");
            await _billRepository.UpdateAsync(existing);
            await _uow.SaveChangesAsync(cancellationToken);
            return Result<BillDto>.Success(_mapper.Map(existing));
        }
        else
        {
            // Create path
            var billNumber = $"BILL-{DateTime.UtcNow:yyyyMM}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
            var bill = new Bill(
                Guid.NewGuid(),
                dto.PatientId,
                billNumber,
                dto.DueDate,
                dto.TaxRate,
                dto.DiscountAmount,
                dto.Notes
            );
            await _billRepository.AddAsync(bill);
            await _uow.SaveChangesAsync(cancellationToken);
            return Result<BillDto>.Success(_mapper.Map(bill));
        }
    }

    // Legacy ToDto removed in favor of BillMapper usage
}
