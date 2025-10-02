using CareSync.Domain.Enums;
using CareSync.Domain.ValueObjects;

namespace CareSync.Domain.Entities;

public class Lab
{
    private Lab() { }

    public Lab(Guid id, string name, string code, Address address, PhoneNumber phoneNumber, Email email)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Address = address ?? throw new ArgumentNullException(nameof(address));
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        IsActive = true;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty; // Lab identifier code
    public Address Address { get; private set; } = null!;
    public PhoneNumber PhoneNumber { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public string? ContactPerson { get; private set; }
    public bool IsActive { get; private set; }

    public void UpdateContactInfo(PhoneNumber phoneNumber, Email email, string? contactPerson)
    {
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        ContactPerson = contactPerson;
    }

    public void UpdateAddress(Address address)
    {
        Address = address ?? throw new ArgumentNullException(nameof(address));
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

public class LabTest
{
    private LabTest() { }

    public LabTest(Guid id, string name, string code, string description, string category,
        decimal cost, int turnaroundTimeHours, string unit)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Category = category ?? throw new ArgumentNullException(nameof(category));
        Cost = cost >= 0 ? cost : throw new ArgumentException("Cost cannot be negative");
        TurnaroundTimeHours = turnaroundTimeHours > 0
            ? turnaroundTimeHours
            : throw new ArgumentException("Turnaround time must be positive");
        Unit = unit ?? throw new ArgumentNullException(nameof(unit));
        IsActive = true;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty; // CPT or LOINC code
    public string Description { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public decimal Cost { get; private set; }
    public int TurnaroundTimeHours { get; private set; }
    public string? PreparationInstructions { get; private set; }
    public string? ReferenceRange { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public bool RequiresFasting { get; private set; }
    public bool IsActive { get; private set; }

    public void UpdateCost(decimal newCost)
    {
        Cost = newCost >= 0 ? newCost : throw new ArgumentException("Cost cannot be negative");
    }

    public void UpdateTurnaroundTime(int hours)
    {
        TurnaroundTimeHours = hours > 0 ? hours : throw new ArgumentException("Hours must be positive");
    }

    public void UpdatePreparationInstructions(string? instructions)
    {
        PreparationInstructions = instructions;
    }

    public void SetFastingRequirement(bool requiresFasting)
    {
        RequiresFasting = requiresFasting;
    }

    public void UpdateReferenceRange(string? range)
    {
        ReferenceRange = range;
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

public class LabOrder
{
    private LabOrder() { }

    public LabOrder(Guid id, Guid medicalRecordId, Guid patientId, Guid doctorId,
        Guid labId, string orderNumber, DateTime? scheduledDate = null)
    {
        Id = id;
        MedicalRecordId = medicalRecordId;
        PatientId = patientId;
        DoctorId = doctorId;
        LabId = labId;
        OrderNumber = orderNumber ?? throw new ArgumentNullException(nameof(orderNumber));
        OrderDate = DateTime.UtcNow;
        ScheduledDate = scheduledDate;
        Status = LabOrderStatus.Pending;
    }

    public Guid Id { get; }
    public Guid MedicalRecordId { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid DoctorId { get; private set; }
    public Guid LabId { get; private set; }
    public string OrderNumber { get; private set; } = string.Empty;
    public DateTime OrderDate { get; private set; }
    public DateTime? ScheduledDate { get; private set; }
    public LabOrderStatus Status { get; private set; }
    public string? ClinicalNotes { get; private set; }
    public bool IsUrgent { get; private set; }
    public DateTime? CompletedDate { get; private set; }
    public string? CancellationReason { get; private set; }

    // Navigation properties
    public virtual MedicalRecord MedicalRecord { get; private set; } = null!;
    public virtual Patient Patient { get; private set; } = null!;
    public virtual Doctor Doctor { get; private set; } = null!;
    public virtual Lab Lab { get; private set; } = null!;
    public virtual ICollection<LabOrderTest> OrderTests { get; } = new List<LabOrderTest>();

    public void AddTest(Guid labTestId)
    {
        if (Status != LabOrderStatus.Pending)
            throw new InvalidOperationException("Cannot add tests to a processed order");

        var orderTest = new LabOrderTest(Guid.NewGuid(), Id, labTestId);
        OrderTests.Add(orderTest);
    }

    public void ScheduleCollection(DateTime scheduledDate)
    {
        if (Status != LabOrderStatus.Pending)
            throw new InvalidOperationException("Cannot schedule a processed order");

        ScheduledDate = scheduledDate;
        Status = LabOrderStatus.Scheduled;
    }

    public void CollectSamples()
    {
        Status = LabOrderStatus.Collected;
    }

    public void SendToLab()
    {
        Status = LabOrderStatus.SentToLab;
    }

    public void CompleteOrder()
    {
        Status = LabOrderStatus.Completed;
        CompletedDate = DateTime.UtcNow;
    }

    public void CancelOrder(string reason)
    {
        Status = LabOrderStatus.Cancelled;
        CancellationReason = reason ?? throw new ArgumentNullException(nameof(reason));
    }

    public void MarkAsUrgent(string? notes = null)
    {
        IsUrgent = true;
        if (!string.IsNullOrEmpty(notes))
            ClinicalNotes = notes;
    }

    public void AddClinicalNotes(string notes)
    {
        ClinicalNotes = notes;
    }
}

public class LabOrderTest
{
    private LabOrderTest() { }

    public LabOrderTest(Guid id, Guid labOrderId, Guid labTestId)
    {
        Id = id;
        LabOrderId = labOrderId;
        LabTestId = labTestId;
    }

    public Guid Id { get; private set; }
    public Guid LabOrderId { get; private set; }
    public Guid LabTestId { get; private set; }
    public string? SpecialInstructions { get; private set; }

    // Navigation properties
    public virtual LabOrder LabOrder { get; private set; } = null!;
    public virtual LabTest LabTest { get; private set; } = null!;

    public void AddSpecialInstructions(string instructions)
    {
        SpecialInstructions = instructions;
    }
}

public class LabResult
{
    private LabResult() { }

    public LabResult(Guid id, Guid labOrderTestId, string value, DateTime testDate)
    {
        Id = id;
        LabOrderTestId = labOrderTestId;
        Value = value ?? throw new ArgumentNullException(nameof(value));
        TestDate = testDate;
        ResultDate = DateTime.UtcNow;
        Status = LabResultStatus.Pending;
    }

    public Guid Id { get; private set; }
    public Guid LabOrderTestId { get; private set; }
    public string Value { get; private set; } = string.Empty;
    public string? Unit { get; private set; }
    public string? ReferenceRange { get; private set; }
    public LabResultStatus Status { get; private set; }
    public bool IsAbnormal { get; private set; }
    public string? AbnormalFlag { get; private set; } // H (High), L (Low), etc.
    public string? Comments { get; private set; }
    public DateTime TestDate { get; private set; }
    public DateTime ResultDate { get; private set; }
    public string? TechnicianNotes { get; private set; }
    public bool IsReviewed { get; private set; }
    public Guid? ReviewedById { get; private set; } // Doctor who reviewed
    public DateTime? ReviewedDate { get; private set; }

    // Navigation properties
    public virtual LabOrderTest LabOrderTest { get; private set; } = null!;

    public void SetReferenceData(string? unit, string? referenceRange)
    {
        Unit = unit;
        ReferenceRange = referenceRange;
    }

    public void MarkAbnormal(string abnormalFlag, string? comments = null)
    {
        IsAbnormal = true;
        AbnormalFlag = abnormalFlag;
        Comments = comments;
    }

    public void AddTechnicianNotes(string notes)
    {
        TechnicianNotes = notes;
    }

    public void FinalizeResult()
    {
        Status = LabResultStatus.Final;
    }

    public void CorrectResult(string newValue, string reason)
    {
        Value = newValue ?? throw new ArgumentNullException(nameof(newValue));
        Comments = $"Corrected: {reason}. Previous value preserved in audit trail.";
        Status = LabResultStatus.Corrected;
    }

    public void ReviewResult(Guid doctorId)
    {
        IsReviewed = true;
        ReviewedById = doctorId;
        ReviewedDate = DateTime.UtcNow;
    }
}
