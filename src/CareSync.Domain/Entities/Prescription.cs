using CareSync.Domain.Enums;

namespace CareSync.Domain.Entities;

public class Prescription
{
    private Prescription() { }

    public Prescription(Guid id, Guid medicalRecordId, Guid patientId, Guid doctorId,
        string medicationName, string dosage, string frequency, string instructions,
        int quantity, int refillsAllowed)
    {
        Id = id;
        MedicalRecordId = medicalRecordId;
        PatientId = patientId;
        DoctorId = doctorId;
        MedicationName = medicationName ?? throw new ArgumentNullException(nameof(medicationName));
        Dosage = dosage ?? throw new ArgumentNullException(nameof(dosage));
        Frequency = frequency ?? throw new ArgumentNullException(nameof(frequency));
        Route = "PO"; // Default route
        Instructions = instructions ?? throw new ArgumentNullException(nameof(instructions));
        Quantity = quantity > 0 ? quantity : throw new ArgumentException("Quantity must be positive", nameof(quantity));
        RefillsAllowed = refillsAllowed >= 0
            ? refillsAllowed
            : throw new ArgumentException("Refills allowed cannot be negative", nameof(refillsAllowed));
        RefillsUsed = 0;
        PrescribedDate = DateTime.UtcNow;
        StartDate = DateTime.UtcNow;
        Status = PrescriptionStatus.Active;
    }

    public Prescription(Guid id, Guid medicalRecordId, string medicationName, string dosage,
        string frequency, string route, int quantity, int? refills, DateTime startDate,
        DateTime? endDate, string? instructions, string? notes)
    {
        Id = id;
        MedicalRecordId = medicalRecordId;
        MedicationName = medicationName ?? throw new ArgumentNullException(nameof(medicationName));
        Dosage = dosage ?? throw new ArgumentNullException(nameof(dosage));
        Frequency = frequency ?? throw new ArgumentNullException(nameof(frequency));
        Route = route ?? throw new ArgumentNullException(nameof(route));
        Quantity = quantity > 0 ? quantity : throw new ArgumentException("Quantity must be positive", nameof(quantity));
        RefillsAllowed = refills ?? 0;
        RefillsUsed = 0;
        StartDate = startDate;
        EndDate = endDate;
        PrescribedDate = DateTime.UtcNow;
        Instructions = instructions ?? string.Empty;
        Notes = notes;
        Status = PrescriptionStatus.Active;
    }

    // Constructor for AddPrescriptionCommand
    public Prescription(Guid id, Guid medicalRecordId, string medicationName, string dosage,
        string frequency, string instructions, DateTime startDate, DateTime? endDate,
        int? durationDays, string? notes)
    {
        Id = id;
        MedicalRecordId = medicalRecordId;
        MedicationName = medicationName ?? throw new ArgumentNullException(nameof(medicationName));
        Dosage = dosage ?? throw new ArgumentNullException(nameof(dosage));
        Frequency = frequency ?? throw new ArgumentNullException(nameof(frequency));
        Instructions = instructions ?? throw new ArgumentNullException(nameof(instructions));
        Route = "PO"; // Default route
        Quantity = 30; // Default quantity
        RefillsAllowed = 0; // Default no refills
        RefillsUsed = 0;
        StartDate = startDate;
        EndDate = endDate;
        PrescribedDate = DateTime.UtcNow;
        Notes = notes;
        Status = PrescriptionStatus.Active;

        // Calculate end date if duration is provided and end date is not set
        if (!EndDate.HasValue && durationDays.HasValue) EndDate = StartDate.AddDays(durationDays.Value);
    }

    public Guid Id { get; private set; }
    public Guid MedicalRecordId { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid DoctorId { get; private set; }
    public string MedicationName { get; private set; } = string.Empty;
    public string Dosage { get; private set; } = string.Empty;
    public string Frequency { get; private set; } = string.Empty;
    public string Route { get; private set; } = string.Empty;
    public string Instructions { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public int RefillsAllowed { get; private set; }
    public int? Refills => RefillsAllowed; // Alias for compatibility
    public int RefillsUsed { get; private set; }
    public DateTime PrescribedDate { get; private set; }
    public DateTime StartDate { get; }
    public DateTime? EndDate { get; }
    public DateTime? ExpiryDate { get; private set; }

    // Calculate duration in days
    public int? DurationDays => EndDate.HasValue ? (int)(EndDate.Value - StartDate).TotalDays : null;

    public PrescriptionStatus Status { get; private set; }

    // Check if prescription is currently active
    public bool IsActive => Status == PrescriptionStatus.Active &&
                            DateTime.UtcNow >= StartDate &&
                            (!EndDate.HasValue || DateTime.UtcNow <= EndDate.Value);

    public string? Notes { get; private set; }

    public bool CanRefill => Status == PrescriptionStatus.Active &&
                             RefillsUsed < RefillsAllowed &&
                             (!ExpiryDate.HasValue || DateTime.UtcNow <= ExpiryDate.Value);

    public int RemainingRefills => Math.Max(0, RefillsAllowed - RefillsUsed);

    public void UpdatePrescription(string medicationName, string dosage, string frequency,
        string instructions, int quantity, int refillsAllowed)
    {
        MedicationName = medicationName ?? throw new ArgumentNullException(nameof(medicationName));
        Dosage = dosage ?? throw new ArgumentNullException(nameof(dosage));
        Frequency = frequency ?? throw new ArgumentNullException(nameof(frequency));
        Instructions = instructions ?? throw new ArgumentNullException(nameof(instructions));
        Quantity = quantity > 0 ? quantity : throw new ArgumentException("Quantity must be positive", nameof(quantity));
        RefillsAllowed = refillsAllowed >= 0
            ? refillsAllowed
            : throw new ArgumentException("Refills allowed cannot be negative", nameof(refillsAllowed));
    }

    public void SetExpiryDate(DateTime expiryDate)
    {
        if (expiryDate <= DateTime.UtcNow)
            throw new ArgumentException("Expiry date must be in the future", nameof(expiryDate));

        ExpiryDate = expiryDate;
    }

    public void ProcessRefill()
    {
        if (Status != PrescriptionStatus.Active)
            throw new InvalidOperationException("Cannot refill inactive prescription");

        if (RefillsUsed >= RefillsAllowed)
            throw new InvalidOperationException("No refills remaining");

        if (ExpiryDate.HasValue && DateTime.UtcNow > ExpiryDate.Value)
            throw new InvalidOperationException("Prescription has expired");

        RefillsUsed++;

        if (RefillsUsed >= RefillsAllowed) Status = PrescriptionStatus.Completed;
    }

    public void Cancel(string reason)
    {
        Status = PrescriptionStatus.Cancelled;
        Notes = $"Cancelled: {reason}";
    }

    public void Complete()
    {
        Status = PrescriptionStatus.Completed;
    }
}
