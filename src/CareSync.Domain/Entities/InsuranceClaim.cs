using CareSync.Domain.Enums;

namespace CareSync.Domain.Entities;

public class InsuranceClaim
{
    private InsuranceClaim() { }

    public InsuranceClaim(Guid id, Guid billId, Guid patientInsuranceId, string claimNumber, decimal claimAmount)
    {
        Id = id;
        BillId = billId;
        PatientInsuranceId = patientInsuranceId;
        ClaimNumber = claimNumber ?? throw new ArgumentNullException(nameof(claimNumber));
        ClaimAmount = claimAmount > 0 ? claimAmount : throw new ArgumentException("Claim amount must be positive");
        SubmissionDate = DateTime.UtcNow;
        Status = ClaimStatus.Submitted;
    }

    public Guid Id { get; private set; }
    public Guid BillId { get; private set; }
    public Guid PatientInsuranceId { get; private set; }
    public string ClaimNumber { get; private set; } = string.Empty;
    public DateTime SubmissionDate { get; private set; }
    public decimal ClaimAmount { get; }
    public decimal ApprovedAmount { get; private set; }
    public decimal PaidAmount { get; private set; }
    public ClaimStatus Status { get; private set; }
    public string? DenialReason { get; private set; }
    public DateTime? ProcessedDate { get; private set; }
    public string? Notes { get; private set; }

    public decimal PendingAmount => ApprovedAmount - PaidAmount;

    public void MarkAsProcessing()
    {
        Status = ClaimStatus.Processing;
    }

    public void Approve(decimal approvedAmount, decimal paidAmount)
    {
        if (approvedAmount <= 0)
            throw new ArgumentException("Approved amount must be positive");

        if (paidAmount < 0 || paidAmount > approvedAmount)
            throw new ArgumentException("Paid amount must be between 0 and approved amount");

        Status = ClaimStatus.Approved;
        ApprovedAmount = approvedAmount;
        PaidAmount = paidAmount;
        ProcessedDate = DateTime.UtcNow;
    }

    public void Deny(string reason)
    {
        Status = ClaimStatus.Denied;
        DenialReason = reason ?? throw new ArgumentNullException(nameof(reason));
        ProcessedDate = DateTime.UtcNow;
    }

    public void PartialApproval(decimal approvedAmount, decimal paidAmount, string reason)
    {
        if (approvedAmount <= 0 || approvedAmount >= ClaimAmount)
            throw new ArgumentException("Approved amount must be positive and less than claim amount");

        Status = ClaimStatus.PartiallyApproved;
        ApprovedAmount = approvedAmount;
        PaidAmount = paidAmount;
        Notes = reason;
        ProcessedDate = DateTime.UtcNow;
    }
}
