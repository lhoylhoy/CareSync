using CareSync.Domain.ValueObjects;

namespace CareSync.Domain.Entities;

public class Doctor
{
    // EF Core requires a parameterless constructor
    private Doctor()
    {
        Name = new FullName("Unknown", "Unknown");
        PhoneNumber = new PhoneNumber("000-000-0000");
        Email = new Email("unknown@example.com");
        Specialty = "General";
        IsActive = true;
    }

    public Doctor(Guid id, FullName name, string specialty, PhoneNumber phoneNumber, Email email)
    {
        if (string.IsNullOrWhiteSpace(specialty))
            throw new ArgumentException("Specialty cannot be null or empty.", nameof(specialty));

        Id = id;
        Name = name;
        Specialty = specialty.Trim();
        PhoneNumber = phoneNumber;
        Email = email;
        IsActive = true;
    }

    public Guid Id { get; private set; }
    public FullName Name { get; private set; }
    public string Specialty { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public Email Email { get; private set; }
    public bool IsActive { get; private set; }

    // Display name with default "Dr" title
    public string DisplayName => $"Dr {Name.DisplayName}";

    public void UpdateContactInformation(PhoneNumber phoneNumber, Email email)
    {
        PhoneNumber = phoneNumber;
        Email = email;
    }

    public void UpdateProfessionalInformation(FullName name, string specialty)
    {
        if (string.IsNullOrWhiteSpace(specialty))
            throw new ArgumentException("Specialty cannot be null or empty.", nameof(specialty));

        Name = name;
        Specialty = specialty.Trim();
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
