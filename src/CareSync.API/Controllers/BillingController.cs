using CareSync.Application.Commands.Billing;
using CareSync.Application.Queries.Billing;
using CareSync.Application.DTOs.Billing;
using MediatR;
using CareSync.Application.Common.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Logging;

namespace CareSync.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BillingController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly IOutputCacheStore _cacheStore;
    private readonly ILogger<BillingController> _logger;

    public BillingController(IMediator mediator, IOutputCacheStore cacheStore, ILogger<BillingController> logger) : base(mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _cacheStore = cacheStore ?? throw new ArgumentNullException(nameof(cacheStore));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    ///     Get all bills
    /// </summary>
    /// <returns>List of bills</returns>
    [HttpGet]
    [OutputCache(PolicyName = "Billing-All")]
    public async Task<ActionResult<IEnumerable<BillDto>>> GetAllBills()
    {
        var result = await _mediator.Send(new GetAllBillsQuery());
        return OkOrProblem(result);
    }

    /// <summary>
    ///     Get a bill by ID
    /// </summary>
    /// <param name="id">Bill ID</param>
    /// <returns>Bill details</returns>
    [HttpGet("{id:guid}")]
    [OutputCache(PolicyName = "Billing-ById")]
    public async Task<ActionResult<BillDto>> GetBill(Guid id)
    {
        var bill = await _mediator.Send(new GetBillQuery(id));
        return OkOrNotFound(bill);
    }

    /// <summary>
    ///     Get bills for a specific patient
    /// </summary>
    /// <param name="patientId">Patient ID</param>
    /// <returns>List of bills</returns>
    [HttpGet("patient/{patientId:guid}")]
    public async Task<ActionResult<object>> GetPatientBills(Guid patientId)
    {
        var bills = await _mediator.Send(new GetPatientBillsQuery(patientId));
        return OkOrProblem(bills);
    }

    /// <summary>
    ///     Get outstanding bills
    /// </summary>
    /// <returns>List of outstanding bills</returns>
    [HttpGet("outstanding")]
    public async Task<ActionResult<object>> GetOutstandingBills()
    {
        var bills = await _mediator.Send(new GetOutstandingBillsQuery());
        return OkOrProblem(bills);
    }

    /// <summary>
    ///     Get overdue bills
    /// </summary>
    /// <returns>List of overdue bills</returns>
    [HttpGet("overdue")]
    public async Task<ActionResult<object>> GetOverdueBills()
    {
        var bills = await _mediator.Send(new GetOverdueBillsQuery());
        return OkOrProblem(bills);
    }

    /// <summary>
    ///     Get billing summary
    /// </summary>
    /// <param name="startDate">Optional start date</param>
    /// <param name="endDate">Optional end date</param>
    /// <returns>Billing summary</returns>
    [HttpGet("summary")]
    public async Task<ActionResult<BillSummaryDto>> GetBillingSummary(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var summary = await _mediator.Send(new GetBillingSummaryQuery(startDate, endDate));
        return OkOrProblem(summary);
    }

    /// <summary>
    ///     Get payment summary
    /// </summary>
    /// <param name="startDate">Optional start date</param>
    /// <param name="endDate">Optional end date</param>
    /// <returns>Payment summary</returns>
    [HttpGet("payments/summary")]
    public async Task<ActionResult<PaymentSummaryDto>> GetPaymentSummary(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var summary = await _mediator.Send(new GetPaymentSummaryQuery(startDate, endDate));
        return OkOrProblem(summary);
    }

    /// <summary>
    ///     Create a new bill
    /// </summary>
    /// <param name="createBill">Bill data</param>
    /// <returns>Created bill ID</returns>
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateBill([FromBody] CreateBillDto createBill)
    {
        var billId = await _mediator.Send(new CreateBillCommand(createBill));
        if (billId.IsSuccess && billId.Value != Guid.Empty)
        {
            await InvalidateBillingCachesAsync(billId.Value, HttpContext.RequestAborted);
        }
        // Route value only used on success (method short-circuits on failure)
        return CreatedOrBadRequest(billId, nameof(GetBill), new { id = billId.Value });
    }

    /// <summary>
    ///     Process a payment for a bill
    /// </summary>
    /// <param name="processPayment">Payment data</param>
    /// <returns>Created payment ID</returns>
    [HttpPost("payments")]
    public async Task<ActionResult<Guid>> ProcessPayment([FromBody] ProcessPaymentDto processPayment)
    {
        var paymentId = await _mediator.Send(new ProcessPaymentCommand(processPayment));
        if (paymentId.IsSuccess)
        {
            await InvalidateBillingCachesAsync(processPayment.BillId, HttpContext.RequestAborted);
        }
        return CreatedOrBadRequest(paymentId, nameof(GetBill), new { id = processPayment.BillId });
    }

    /// <summary>
    ///     Create an insurance claim for a bill
    /// </summary>
    /// <param name="createClaim">Insurance claim data</param>
    /// <returns>Created claim ID</returns>
    [HttpPost("claims")]
    public async Task<ActionResult<Guid>> CreateInsuranceClaim([FromBody] CreateInsuranceClaimDto createClaim)
    {
        var claimId = await _mediator.Send(new CreateInsuranceClaimCommand(createClaim));
        if (claimId.IsSuccess)
        {
            await InvalidateBillingCachesAsync(createClaim.BillId, HttpContext.RequestAborted);
        }
        return CreatedOrBadRequest(claimId, nameof(GetBill), new { id = createClaim.BillId });
    }

    /// <summary>
    ///     Approve an insurance claim
    /// </summary>
    /// <param name="claimId">Claim ID</param>
    /// <param name="approvedAmount">Approved amount</param>
    /// <param name="paidAmount">Paid amount</param>
    /// <returns>Success result</returns>
    [HttpPost("claims/{claimId:guid}/approve")]
    public async Task<ActionResult> ApproveClaim(Guid claimId, [FromBody] decimal approvedAmount, [FromQuery] decimal paidAmount = 0)
    {
        var approved = await _mediator.Send(new ApproveClaimCommand(claimId, approvedAmount, paidAmount));
        return NoContentOrNotFound(approved);
    }

    /// <summary>
    ///     Deny an insurance claim
    /// </summary>
    /// <param name="claimId">Claim ID</param>
    /// <param name="reason">Denial reason</param>
    /// <returns>Success result</returns>
    [HttpPost("claims/{claimId:guid}/deny")]
    public async Task<ActionResult> DenyClaim(Guid claimId, [FromBody] string reason)
    {
        if (string.IsNullOrWhiteSpace(reason)) return BadRequest("Denial reason is required");
        var denied = await _mediator.Send(new DenyClaimCommand(claimId, reason));
        return NoContentOrNotFound(denied);
    }

    /// <summary>
    ///     Update a bill
    /// </summary>
    /// <param name="id">Bill ID</param>
    /// <param name="updateBill">Bill update data</param>
    /// <returns>Updated bill</returns>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BillDto>> UpdateBill(Guid id, [FromBody] UpdateBillDto updateBill)
    {
        if (id != updateBill.Id) return BadRequest("ID mismatch between route and body");
        var bill = await _mediator.Send(new UpdateBillCommand(updateBill));
        if (bill.IsSuccess && bill.Value is not null)
        {
            await InvalidateBillingCachesAsync(bill.Value.Id, HttpContext.RequestAborted);
        }
        return UpdatedOrNotFound(bill);
    }

    /// <summary>
    ///     Upsert a bill (Create if not exists, Update if exists)
    /// </summary>
    /// <param name="upsertBill">Bill data</param>
    /// <returns>Created or updated bill</returns>
    [HttpPut("upsert")]
    public async Task<ActionResult<BillDto>> UpsertBill([FromBody] UpsertBillDto upsertBill)
    {
        var bill = await _mediator.Send(new UpsertBillCommand(upsertBill));
        if (bill.IsSuccess && bill.Value is not null)
        {
            await InvalidateBillingCachesAsync(bill.Value.Id, HttpContext.RequestAborted);
        }
        return UpsertOkOrBadRequest(bill);
    }

    /// <summary>
    ///     Delete a bill by ID
    /// </summary>
    /// <param name="id">Bill ID</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteBill(Guid id)
    {
        var deleted = await _mediator.Send(new DeleteBillCommand(id));
        if (deleted.IsSuccess)
        {
            await InvalidateBillingCachesAsync(id, HttpContext.RequestAborted);
        }
        return NoContentOrNotFound(deleted);
    }

    private async Task InvalidateBillingCachesAsync(Guid billId, CancellationToken token)
    {
        try
        {
            await _cacheStore.EvictByTagAsync("billing-all", token);
            await _cacheStore.EvictByTagAsync("billing-byid", token);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to evict billing cache for bill {BillId}", billId);
        }
    }
}
