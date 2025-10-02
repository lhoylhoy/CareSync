namespace CareSync.Domain.Entities;

public class InsuranceProvider
{
    private InsuranceProvider() { }

    public InsuranceProvider(Guid id, string name, string policyPrefix, string contactPhone,
        string contactEmail, string address)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        PolicyPrefix = policyPrefix ?? throw new ArgumentNullException(nameof(policyPrefix));
        ContactPhone = contactPhone ?? throw new ArgumentNullException(nameof(contactPhone));
        ContactEmail = contactEmail ?? throw new ArgumentNullException(nameof(contactEmail));
        Address = address ?? throw new ArgumentNullException(nameof(address));
        IsActive = true;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string PolicyPrefix { get; private set; } = string.Empty;
    public string ContactPhone { get; private set; } = string.Empty;
    public string ContactEmail { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public decimal DefaultCopayAmount { get; private set; }
    public decimal DefaultDeductible { get; private set; }
    public string? Notes { get; private set; }

    public void UpdateContactInformation(string contactPhone, string contactEmail, string address)
    {
        ContactPhone = contactPhone ?? throw new ArgumentNullException(nameof(contactPhone));
        ContactEmail = contactEmail ?? throw new ArgumentNullException(nameof(contactEmail));
        Address = address ?? throw new ArgumentNullException(nameof(address));
    }

    public void SetDefaultAmounts(decimal copayAmount, decimal deductible)
    {
        DefaultCopayAmount =
            copayAmount >= 0 ? copayAmount : throw new ArgumentException("Copay amount cannot be negative");
        DefaultDeductible = deductible >= 0 ? deductible : throw new ArgumentException("Deductible cannot be negative");
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}

public class PatientInsurance
{
    private PatientInsurance() { }

    public PatientInsurance(Guid id, Guid patientId, Guid insuranceProviderId, string policyNumber,
        string groupNumber, string subscriberName, string relationship, DateTime effectiveDate)
    {
        Id = id;
        PatientId = patientId;
        InsuranceProviderId = insuranceProviderId;
        PolicyNumber = policyNumber ?? throw new ArgumentNullException(nameof(policyNumber));
        GroupNumber = groupNumber ?? throw new ArgumentNullException(nameof(groupNumber));
        SubscriberName = subscriberName ?? throw new ArgumentNullException(nameof(subscriberName));
        Relationship = relationship ?? throw new ArgumentNullException(nameof(relationship));
        EffectiveDate = effectiveDate;
        IsActive = true;
    }

    public Guid Id { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid InsuranceProviderId { get; private set; }
    public string PolicyNumber { get; private set; } = string.Empty;
    public string GroupNumber { get; private set; } = string.Empty;
    public string SubscriberName { get; private set; } = string.Empty;
    public string Relationship { get; private set; } = string.Empty;
    public DateTime EffectiveDate { get; }
    public DateTime? ExpirationDate { get; private set; }
    public decimal CopayAmount { get; private set; }
    public decimal DeductibleAmount { get; private set; }
    public bool IsPrimary { get; private set; }
    public bool IsActive { get; private set; }

    public bool IsCurrentlyActive => IsActive && DateTime.UtcNow >= EffectiveDate &&
                                     (!ExpirationDate.HasValue || DateTime.UtcNow <= ExpirationDate.Value);

    public void SetAsPrimary()
    {
        IsPrimary = true;
    }

    public void SetAsSecondary()
    {
        IsPrimary = false;
    }

    public void UpdateCoverageDetails(decimal copayAmount, decimal deductibleAmount, DateTime? expirationDate = null)
    {
        CopayAmount = copayAmount >= 0 ? copayAmount : throw new ArgumentException("Copay amount cannot be negative");
        DeductibleAmount = deductibleAmount >= 0
            ? deductibleAmount
            : throw new ArgumentException("Deductible amount cannot be negative");
        ExpirationDate = expirationDate;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
