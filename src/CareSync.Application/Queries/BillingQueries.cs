using CareSync.Application.Common.Mapping;
using CareSync.Application.Common.Results;
using CareSync.Application.DTOs.Billing;
using CareSync.Domain.Entities;
using CareSync.Domain.Enums;
using CareSync.Domain.Interfaces;
using MediatR;

namespace CareSync.Application.Queries.Billing;

public record GetBillQuery(Guid Id) : IRequest<Result<BillDto>>;

public record GetAllBillsQuery : IRequest<Result<IEnumerable<BillDto>>>;

public class GetBillHandler : IRequestHandler<GetBillQuery, Result<BillDto>>
{
    private readonly IBillRepository _billRepository;
    private readonly BillMapper _mapper;

    public GetBillHandler(IBillRepository billRepository, BillMapper mapper)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Result<BillDto>> Handle(GetBillQuery request, CancellationToken cancellationToken)
    {
        var bill = await _billRepository.GetByIdAsync(request.Id);
        if (bill is null)
            return Result<BillDto>.Failure($"Bill with ID {request.Id} not found");
        return Result<BillDto>.Success(_mapper.Map(bill));
    }
}

public class GetAllBillsHandler : IRequestHandler<GetAllBillsQuery, Result<IEnumerable<BillDto>>>
{
    private readonly IBillRepository _billRepository;
    private readonly BillMapper _mapper;

    public GetAllBillsHandler(IBillRepository billRepository, BillMapper mapper)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Result<IEnumerable<BillDto>>> Handle(GetAllBillsQuery request, CancellationToken cancellationToken)
    {
        var bills = await _billRepository.GetAllAsync();
        return Result<IEnumerable<BillDto>>.Success(bills.Select(b => _mapper.Map(b)).ToList());
    }
}

public record GetPatientBillsQuery(Guid PatientId) : IRequest<Result<List<BillDto>>>;

public class GetPatientBillsHandler : IRequestHandler<GetPatientBillsQuery, Result<List<BillDto>>>
{
    private readonly IBillRepository _billRepository;
    private readonly BillMapper _mapper;

    public GetPatientBillsHandler(IBillRepository billRepository, BillMapper mapper)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Result<List<BillDto>>> Handle(GetPatientBillsQuery request, CancellationToken cancellationToken)
    {
        var bills = await _billRepository.GetByPatientIdAsync(request.PatientId);
        return Result<List<BillDto>>.Success(bills.Select(b => _mapper.Map(b)).ToList());
    }
}

public record GetOutstandingBillsQuery : IRequest<Result<List<BillDto>>>;

public class GetOutstandingBillsHandler : IRequestHandler<GetOutstandingBillsQuery, Result<List<BillDto>>>
{
    private readonly IBillRepository _billRepository;
    private readonly BillMapper _mapper;

    public GetOutstandingBillsHandler(IBillRepository billRepository, BillMapper mapper)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Result<List<BillDto>>> Handle(GetOutstandingBillsQuery request, CancellationToken cancellationToken)
    {
        var bills = await _billRepository.GetOutstandingBillsAsync();
        return Result<List<BillDto>>.Success(bills.Select(b => _mapper.Map(b)).ToList());
    }
}

public record GetOverdueBillsQuery : IRequest<Result<List<BillDto>>>;

public class GetOverdueBillsHandler : IRequestHandler<GetOverdueBillsQuery, Result<List<BillDto>>>
{
    private readonly IBillRepository _billRepository;
    private readonly BillMapper _mapper;

    public GetOverdueBillsHandler(IBillRepository billRepository, BillMapper mapper)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Result<List<BillDto>>> Handle(GetOverdueBillsQuery request, CancellationToken cancellationToken)
    {
        var bills = await _billRepository.GetOverdueBillsAsync();
        return Result<List<BillDto>>.Success(bills.Select(b => _mapper.Map(b)).ToList());
    }
}

public record GetBillingSummaryQuery(DateTime? StartDate = null, DateTime? EndDate = null) : IRequest<Result<BillSummaryDto>>;

public class GetBillingSummaryHandler : IRequestHandler<GetBillingSummaryQuery, Result<BillSummaryDto>>
{
    private readonly IBillRepository _billRepository;

    public GetBillingSummaryHandler(IBillRepository billRepository)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
    }

    public async Task<Result<BillSummaryDto>> Handle(GetBillingSummaryQuery request, CancellationToken cancellationToken)
    {
        var startDate = request.StartDate ?? DateTime.UtcNow.AddMonths(-1);
        var endDate = request.EndDate ?? DateTime.UtcNow;

        var bills = await _billRepository.GetByDateRangeAsync(startDate, endDate);

        var totalAmount = bills.Sum(b => b.TotalAmount);
        var paidAmount = bills.Sum(b => b.PaidAmount);
        var pendingAmount = bills.Where(b => b.BalanceAmount > 0).Sum(b => b.BalanceAmount);
        var overdueAmount = bills.Where(b => b.IsOverdue).Sum(b => b.BalanceAmount);
        var overdueBills = bills.Count(b => b.IsOverdue);
        var collectionRate = totalAmount > 0 ? paidAmount / totalAmount * 100 : 0;

        return Result<BillSummaryDto>.Success(new BillSummaryDto
        {
            TotalBills = bills.Count,
            TotalAmount = totalAmount,
            PaidAmount = paidAmount,
            PendingAmount = pendingAmount,
            OverdueAmount = overdueAmount,
            OverdueBills = overdueBills,
            CollectionRate = collectionRate
        });
    }
}

public record GetPaymentSummaryQuery(DateTime? StartDate = null, DateTime? EndDate = null)
    : IRequest<Result<PaymentSummaryDto>>;

public class GetPaymentSummaryHandler : IRequestHandler<GetPaymentSummaryQuery, Result<PaymentSummaryDto>>
{
    private readonly IPaymentRepository _paymentRepository;

    public GetPaymentSummaryHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
    }

    public async Task<Result<PaymentSummaryDto>> Handle(GetPaymentSummaryQuery request, CancellationToken cancellationToken)
    {
        var startDate = request.StartDate ?? DateTime.UtcNow.AddMonths(-1);
        var endDate = request.EndDate ?? DateTime.UtcNow;

        var payments = await _paymentRepository.GetByDateRangeAsync(startDate, endDate);

        var totalAmount = payments.Sum(p => p.Amount);
        var cashPayments = payments.Where(p => p.Method == PaymentMethod.Cash).Sum(p => p.Amount);
        var cardPayments = payments
            .Where(p => p.Method == PaymentMethod.CreditCard || p.Method == PaymentMethod.DebitCard).Sum(p => p.Amount);
        var insurancePayments = payments.Where(p => p.Method == PaymentMethod.Insurance).Sum(p => p.Amount);
        var onlinePayments = payments.Where(p => p.Method == PaymentMethod.Online).Sum(p => p.Amount);

        return Result<PaymentSummaryDto>.Success(new PaymentSummaryDto
        {
            TotalPayments = payments.Count,
            TotalAmount = totalAmount,
            CashPayments = cashPayments,
            CardPayments = cardPayments,
            InsurancePayments = insurancePayments,
            OnlinePayments = onlinePayments
        });
    }
}

// Legacy BillMappingExtensions removed in favor of central BillMapper
