using CareSync.Domain.Events;
using CareSync.Domain.Interfaces;

namespace CareSync.Domain.Entities;

public class MedicalRecord : IHasDomainEvents
{
    private readonly List<Diagnosis> _diagnoses = new();
    private readonly List<Prescription> _prescriptions = new();
    private readonly List<TreatmentRecord> _treatmentRecords = new();

    // Aggregated entities (encapsulated collections)
    private readonly List<VitalSigns> _vitalSigns = new();

    // Required for EF Core
    private MedicalRecord() { }

    /// <summary>
    ///     Create a new medical record - Primary constructor
    /// </summary>
    public MedicalRecord(
        Guid patientId,
        Guid doctorId,
        string chiefComplaint,
        Guid? appointmentId = null,
        string? historyOfPresentIllness = null)
    {
        ValidateRequiredFields(patientId, doctorId, chiefComplaint);

        Id = Guid.NewGuid();
        PatientId = patientId;
        DoctorId = doctorId;
        AppointmentId = appointmentId;
        ChiefComplaint = chiefComplaint;
        HistoryOfPresentIllness = historyOfPresentIllness ?? string.Empty;
        RecordDate = DateTime.UtcNow;

        // Initialize empty sections
        PhysicalExamination = string.Empty;
        Assessment = string.Empty;
        TreatmentPlan = string.Empty;

        // Record lifecycle
        IsFinalized = false;
    }

    // Core identifiers
    public Guid PatientId { get; private set; }
    public Guid Id { get; }
    public Guid DoctorId { get; private set; }
    public Guid? AppointmentId { get; private set; }
    public DateTime RecordDate { get; private set; }

    // Medical content
    public string ChiefComplaint { get; private set; } = string.Empty;
    public string HistoryOfPresentIllness { get; private set; } = string.Empty;
    public string PhysicalExamination { get; private set; } = string.Empty;
    public string Assessment { get; private set; } = string.Empty;
    public string TreatmentPlan { get; private set; } = string.Empty;
    public string? Notes { get; private set; }

    // Record lifecycle
    public bool IsFinalized { get; private set; }
    public DateTime? FinalizedDate { get; private set; }
    public string? FinalizedBy { get; private set; }

    // Read-only collections
    public IReadOnlyCollection<VitalSigns> VitalSigns => _vitalSigns.AsReadOnly();
    public IReadOnlyCollection<Diagnosis> Diagnoses => _diagnoses.AsReadOnly();
    public IReadOnlyCollection<Prescription> Prescriptions => _prescriptions.AsReadOnly();
    public IReadOnlyCollection<TreatmentRecord> TreatmentRecords => _treatmentRecords.AsReadOnly();

    // Navigation properties (for ORM only)
    public Patient? Patient { get; private set; }
    public Doctor? Doctor { get; private set; }
    public Appointment? Appointment { get; private set; }

    // Domain events raised by this aggregate
    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    private void Raise(DomainEvent evt) => _domainEvents.Add(evt);
    public void ClearDomainEvents() => _domainEvents.Clear();

    private static void ValidateRequiredFields(Guid patientId, Guid doctorId, string chiefComplaint)
    {
        if (patientId == Guid.Empty)
            throw new ArgumentException("Patient ID cannot be empty", nameof(patientId));

        if (doctorId == Guid.Empty)
            throw new ArgumentException("Doctor ID cannot be empty", nameof(doctorId));

        ValidateNotEmpty(chiefComplaint, nameof(chiefComplaint));
    }

    private static void ValidateNotEmpty(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{paramName} cannot be null or empty", paramName);
    }

    private void ThrowIfFinalized()
    {
        if (IsFinalized)
            throw new InvalidOperationException("Cannot modify finalized medical record");
    }

    private void ValidateCompleteness()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(ChiefComplaint))
            errors.Add("Chief complaint is required");

        if (string.IsNullOrWhiteSpace(Assessment))
            errors.Add("Clinical assessment is required");

        if (string.IsNullOrWhiteSpace(TreatmentPlan))
            errors.Add("Treatment plan is required");

        if (!_diagnoses.Any())
            errors.Add("At least one diagnosis is required");

        if (_diagnoses.Any() && !_diagnoses.Any(d => d.IsPrimary))
            errors.Add("A primary diagnosis must be specified");

        if (!_vitalSigns.Any())
            errors.Add("At least one set of vital signs is required");

        if (errors.Any())
            throw new InvalidOperationException($"Medical record is incomplete: {string.Join(", ", errors)}");
    }

    private void AddFinalNotes(string finalNotes)
    {
        var finalNotesText = $"FINALIZED - {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC\n{finalNotes}";
        Notes = string.IsNullOrEmpty(Notes) ? finalNotesText : $"{Notes}\n\n{finalNotesText}";
    }

    #region Business Logic - Record Content

    /// <summary>
    ///     Update the chief complaint
    /// </summary>
    public void UpdateChiefComplaint(string chiefComplaint)
    {
        ThrowIfFinalized();
        ValidateNotEmpty(chiefComplaint, nameof(chiefComplaint));

        ChiefComplaint = chiefComplaint;
    }

    /// <summary>
    ///     Update history of present illness
    /// </summary>
    public void UpdateHistoryOfPresentIllness(string historyOfPresentIllness)
    {
        ThrowIfFinalized();
        HistoryOfPresentIllness = historyOfPresentIllness ?? string.Empty;
    }

    /// <summary>
    ///     Update physical examination findings
    /// </summary>
    public void UpdatePhysicalExamination(string physicalExamination)
    {
        ThrowIfFinalized();
        PhysicalExamination = physicalExamination ?? string.Empty;
    }

    /// <summary>
    ///     Update clinical assessment
    /// </summary>
    public void UpdateAssessment(string assessment)
    {
        ThrowIfFinalized();
        Assessment = assessment ?? string.Empty;
    }

    /// <summary>
    ///     Update treatment plan
    /// </summary>
    public void UpdateTreatmentPlan(string treatmentPlan)
    {
        ThrowIfFinalized();
        TreatmentPlan = treatmentPlan ?? string.Empty;
    }

    /// <summary>
    ///     Add or update notes
    /// </summary>
    public void AddNotes(string notes)
    {
        ThrowIfFinalized();
        if (string.IsNullOrWhiteSpace(notes)) return;

        Notes = string.IsNullOrEmpty(Notes) ? notes : $"{Notes}\n{notes}";
    }

    /// <summary>
    ///     Update notes (replace existing)
    /// </summary>
    public void UpdateNotes(string? notes)
    {
        ThrowIfFinalized();
        Notes = notes;
    }

    #endregion

    #region Business Logic - Vital Signs Management

    /// <summary>
    ///     Add vital signs to this medical record
    /// </summary>
    public VitalSigns AddVitalSigns(
        decimal? temperature = null,
        int? systolicBp = null,
        int? diastolicBp = null,
        int? heartRate = null,
        int? respiratoryRate = null,
        decimal? weight = null,
        decimal? height = null,
        decimal? oxygenSaturation = null,
        string? notes = null,
        DateTime? measuredAt = null)
    {
        ThrowIfFinalized();

        var vitalSigns = new VitalSigns(
            Guid.NewGuid(), // Generate new Id
            Id, // MedicalRecordId
            measuredAt ?? DateTime.UtcNow,
            temperature,
            systolicBp,
            diastolicBp,
            heartRate,
            respiratoryRate,
            weight,
            height,
            oxygenSaturation,
            notes);

        _vitalSigns.Add(vitalSigns);
        return vitalSigns;
    }

    /// <summary>
    ///     Remove vital signs
    /// </summary>
    public void RemoveVitalSigns(VitalSigns vitalSigns)
    {
        ThrowIfFinalized();
        _vitalSigns.Remove(vitalSigns);
    }

    #endregion

    #region Business Logic - Diagnosis Management

    /// <summary>
    ///     Add a diagnosis to this medical record
    /// </summary>
    public Diagnosis AddDiagnosis(
        string code,
        string name,
        string? description = null,
        bool isPrimary = false,
        string? notes = null)
    {
        ThrowIfFinalized();
        ValidateNotEmpty(code, nameof(code));
        ValidateNotEmpty(name, nameof(name));

        // Ensure only one primary diagnosis
        if (isPrimary && _diagnoses.Any(d => d.IsPrimary))
            throw new InvalidOperationException("Only one primary diagnosis is allowed per medical record");

        var diagnosis = new Diagnosis(Guid.NewGuid(), Id, code, name, description, isPrimary, notes);
        _diagnoses.Add(diagnosis);
        return diagnosis;
    }

    /// <summary>
    ///     Remove a diagnosis
    /// </summary>
    public void RemoveDiagnosis(Diagnosis diagnosis)
    {
        ThrowIfFinalized();
        _diagnoses.Remove(diagnosis);
    }

    /// <summary>
    ///     Set primary diagnosis (ensuring business rules)
    /// </summary>
    public void SetPrimaryDiagnosis(Diagnosis diagnosis)
    {
        ThrowIfFinalized();

        if (!_diagnoses.Contains(diagnosis))
            throw new InvalidOperationException("Diagnosis not found in this medical record");

        // Clear existing primary
        foreach (var existing in _diagnoses.Where(d => d.IsPrimary)) existing.SetAsPrimary(false);

        diagnosis.SetAsPrimary(true);
    }

    #endregion

    #region Business Logic - Prescription Management

    /// <summary>
    ///     Add a prescription to this medical record
    /// </summary>
    public Prescription AddPrescription(
        string medicationName,
        string dosage,
        string frequency,
        string instructions,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? durationDays = null,
        string? notes = null)
    {
        ThrowIfFinalized();
        ValidateNotEmpty(medicationName, nameof(medicationName));
        ValidateNotEmpty(dosage, nameof(dosage));
        ValidateNotEmpty(frequency, nameof(frequency));
        ValidateNotEmpty(instructions, nameof(instructions));

        var prescription = new Prescription(
            Guid.NewGuid(), // Generate new Id
            Id, // MedicalRecordId
            medicationName,
            dosage,
            frequency,
            instructions,
            startDate ?? DateTime.UtcNow,
            endDate,
            durationDays,
            notes);

        _prescriptions.Add(prescription);
        return prescription;
    }

    /// <summary>
    ///     Remove a prescription
    /// </summary>
    public void RemovePrescription(Prescription prescription)
    {
        ThrowIfFinalized();
        _prescriptions.Remove(prescription);
    }

    #endregion

    #region Business Logic - Record Lifecycle

    /// <summary>
    ///     Finalize this medical record, preventing further modifications
    ///     Business Rule: Record must be complete before finalizing
    /// </summary>
    public void Finalize(string? finalNotes = null, string? finalizedBy = null)
    {
        if (IsFinalized)
            throw new InvalidOperationException("Medical record is already finalized");

        ValidateCompleteness();

        IsFinalized = true;
        FinalizedDate = DateTime.UtcNow;
        FinalizedBy = finalizedBy;

        if (!string.IsNullOrWhiteSpace(finalNotes)) AddFinalNotes(finalNotes);
        Raise(new MedicalRecordFinalizedEvent(Id, PatientId, DoctorId, FinalizedDate!.Value));
    }

    /// <summary>
    ///     Reopen a finalized medical record for editing (administrative action)
    /// </summary>
    public void Reopen()
    {
        if (!IsFinalized)
            throw new InvalidOperationException("Medical record is not finalized");

        IsFinalized = false;
        FinalizedDate = null;
        FinalizedBy = null;
        Raise(new MedicalRecordReopenedEvent(Id, PatientId, DoctorId));
    }

    #endregion
}
