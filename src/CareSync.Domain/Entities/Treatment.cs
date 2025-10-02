using CareSync.Domain.Enums;

namespace CareSync.Domain.Entities;

public class Treatment
{
    private Treatment() { }

    public Treatment(Guid id, string name, string code, string description,
        TreatmentCategory category, decimal baseCost, int estimatedDurationMinutes)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Category = category;
        BaseCost = baseCost >= 0 ? baseCost : throw new ArgumentException("Base cost cannot be negative");
        EstimatedDurationMinutes = estimatedDurationMinutes > 0
            ? estimatedDurationMinutes
            : throw new ArgumentException("Duration must be positive");
        IsActive = true;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty; // CPT or HCPCS code
    public string Description { get; private set; } = string.Empty;
    public TreatmentCategory Category { get; private set; }
    public decimal BaseCost { get; private set; }
    public int EstimatedDurationMinutes { get; private set; }
    public bool RequiresSpecialist { get; private set; }
    public string? SpecialInstructions { get; private set; }
    public bool IsActive { get; private set; }

    public void UpdateCost(decimal newCost)
    {
        BaseCost = newCost >= 0 ? newCost : throw new ArgumentException("Cost cannot be negative");
    }

    public void UpdateDuration(int durationMinutes)
    {
        EstimatedDurationMinutes = durationMinutes > 0
            ? durationMinutes
            : throw new ArgumentException("Duration must be positive");
    }

    public void SetSpecialistRequirement(bool requiresSpecialist)
    {
        RequiresSpecialist = requiresSpecialist;
    }

    public void UpdateInstructions(string? instructions)
    {
        SpecialInstructions = instructions;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Reactivate()
    {
        IsActive = true;
    }
}

public class TreatmentRecord
{
    private TreatmentRecord() { }

    public TreatmentRecord(Guid id, Guid medicalRecordId, Guid treatmentId, Guid providerId,
        DateTime treatmentDate, decimal actualCost, int actualDurationMinutes)
    {
        Id = id;
        MedicalRecordId = medicalRecordId;
        TreatmentId = treatmentId;
        ProviderId = providerId;
        TreatmentDate = treatmentDate;
        ActualCost = actualCost >= 0 ? actualCost : throw new ArgumentException("Cost cannot be negative");
        ActualDurationMinutes = actualDurationMinutes > 0
            ? actualDurationMinutes
            : throw new ArgumentException("Duration must be positive");
        Status = TreatmentStatus.Planned;
    }

    public Guid Id { get; private set; }
    public Guid MedicalRecordId { get; private set; }
    public Guid TreatmentId { get; private set; }
    public Guid ProviderId { get; private set; } // Doctor or Staff who performed treatment
    public DateTime TreatmentDate { get; private set; }
    public TreatmentStatus Status { get; private set; }
    public int ActualDurationMinutes { get; private set; }
    public decimal ActualCost { get; private set; }
    public string? Notes { get; private set; }
    public string? Complications { get; private set; }
    public string? FollowUpInstructions { get; private set; }
    public DateTime? FollowUpDate { get; private set; }

    // Navigation properties
    public virtual MedicalRecord MedicalRecord { get; private set; } = null!;
    public virtual Treatment Treatment { get; private set; } = null!;

    public void StartTreatment()
    {
        Status = TreatmentStatus.InProgress;
    }

    public void CompleteTreatment(string? notes, string? followUpInstructions, DateTime? followUpDate)
    {
        Status = TreatmentStatus.Completed;
        Notes = notes;
        FollowUpInstructions = followUpInstructions;
        FollowUpDate = followUpDate;
    }

    public void CancelTreatment(string reason)
    {
        Status = TreatmentStatus.Cancelled;
        Notes = reason;
    }

    public void ReportComplications(string complications)
    {
        Complications = complications ?? throw new ArgumentNullException(nameof(complications));
    }

    public void UpdateFollowUp(string instructions, DateTime? date)
    {
        FollowUpInstructions = instructions;
        FollowUpDate = date;
    }
}
