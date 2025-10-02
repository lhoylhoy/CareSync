namespace CareSync.Domain.ValueObjects;

public record FullName
{
    // Safe parameterless constructor for EF Core - directly initializes without validation
    protected FullName()
    {
        FirstName = "Unknown";
        LastName = "Unknown";
        MiddleName = null;
    }

    public FullName(string firstName, string lastName, string? middleName = null)
    {
        // Ensure we never get null or empty values
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be null or empty.", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be null or empty.", nameof(lastName));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        MiddleName = string.IsNullOrWhiteSpace(middleName) ? null : middleName.Trim();
    }

    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string? MiddleName { get; init; }

    public string DisplayName => string.IsNullOrEmpty(MiddleName)
        ? $"{FirstName} {LastName}"
        : $"{FirstName} {MiddleName} {LastName}";

    public override string ToString()
    {
        return DisplayName;
    }
}
