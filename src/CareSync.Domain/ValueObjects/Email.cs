using System.Text.RegularExpressions;

namespace CareSync.Domain.ValueObjects;

public record Email
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase,
        TimeSpan.FromMilliseconds(100));

    // Parameterless constructor for EF Core
    private Email() : this("noreply@caresync.health") { }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email address is required.", nameof(value));

        var trimmedValue = value.Trim();

        if (trimmedValue.Length > 254)
            throw new ArgumentException("Email address cannot exceed 254 characters.", nameof(value));

        if (!EmailRegex.IsMatch(trimmedValue))
            throw new ArgumentException("Email address format is invalid.", nameof(value));

        Value = trimmedValue.ToLowerInvariant();
    }

    public string Value { get; }

    public static implicit operator string(Email email)
    {
        return email.Value;
    }

    public override string ToString()
    {
        return Value;
    }
}
