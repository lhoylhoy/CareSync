namespace CareSync.Domain.Enums;

public enum DiagnosisStatus
{
    Active = 1,
    Resolved = 2,
    Chronic = 3,
    Suspected = 4,
    RuledOut = 5
}

public enum TreatmentStatus
{
    Planned = 1,
    InProgress = 2,
    Completed = 3,
    Cancelled = 4,
    Rescheduled = 5
}

public enum TreatmentCategory
{
    Consultation = 1,
    Diagnostic = 2,
    Therapeutic = 3,
    Surgical = 4,
    Emergency = 5,
    Preventive = 6,
    Rehabilitation = 7
}

public enum ClaimStatus
{
    Submitted = 1,
    Processing = 2,
    Approved = 3,
    Denied = 4,
    PartiallyApproved = 5,
    PendingInformation = 6
}

public enum LabOrderStatus
{
    Pending = 1,
    Scheduled = 2,
    Collected = 3,
    SentToLab = 4,
    InProgress = 5,
    Completed = 6,
    Cancelled = 7
}

public enum LabResultStatus
{
    Pending = 1,
    InProgress = 2,
    Preliminary = 3,
    Final = 4,
    Corrected = 5,
    Cancelled = 6
}
