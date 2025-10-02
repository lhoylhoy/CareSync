using CareSync.Domain.Enums;

namespace CareSync.Domain.Entities;

public class Diagnosis
{
    private Diagnosis() { }

    public Diagnosis(Guid id, string icdCode, string description, string severity, DateTime diagnosedDate,
        string? notes = null)
    {
        Id = id;
        IcdCode = icdCode ?? throw new ArgumentNullException(nameof(icdCode));
        Name = description ?? throw new ArgumentNullException(nameof(description));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Severity = severity ?? throw new ArgumentNullException(nameof(severity));
        DiagnosedDate = diagnosedDate;
        Notes = notes;
        IsActive = true;
        IsPrimary = false;
    }

    public Diagnosis(Guid id, Guid medicalRecordId, string code, string name, string? description = null,
        bool isPrimary = false, string? notes = null)
    {
        Id = id;
        MedicalRecordId = medicalRecordId;
        IcdCode = code ?? throw new ArgumentNullException(nameof(code));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? string.Empty;
        IsPrimary = isPrimary;
        DiagnosedDate = DateTime.UtcNow;
        Notes = notes;
        IsActive = true;
        Status = DiagnosisStatus.Active;
    }

    public Guid Id { get; private set; }
    public Guid MedicalRecordId { get; private set; }
    public string IcdCode { get; private set; } = string.Empty;
    public string Code => IcdCode; // Alias for compatibility
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsPrimary { get; private set; }
    public DiagnosisStatus Status { get; private set; } = DiagnosisStatus.Active;
    public string Severity { get; private set; } = string.Empty;
    public DateTime DiagnosedDate { get; private set; }
    public DateTime DiagnosisDate => DiagnosedDate; // Alias for compatibility
    public string? Notes { get; private set; }
    public bool IsActive { get; private set; }

    public void UpdateDiagnosis(string description, string severity, string? notes = null)
    {
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Severity = severity ?? throw new ArgumentNullException(nameof(severity));
        Notes = notes ?? string.Empty;
    }

    public void Update(string code, string name, string? description, bool isPrimary, string status,
        DateTime diagnosisDate, string? notes)
    {
        IcdCode = code;
        Name = name;
        Description = description ?? string.Empty;
        IsPrimary = isPrimary;
        DiagnosedDate = diagnosisDate;
        Notes = notes;

        // Parse status enum
        if (Enum.TryParse<DiagnosisStatus>(status, out var parsedStatus)) Status = parsedStatus;
    }

    public void MarkAsInactive()
    {
        IsActive = false;
    }

    public void MarkAsActive()
    {
        IsActive = true;
    }

    public void SetAsPrimary(bool isPrimary)
    {
        IsPrimary = isPrimary;
    }
}
