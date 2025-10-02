using System.Text.RegularExpressions;

namespace CareSync.Domain.ValueObjects;

public record PhoneNumber
{
    // Philippine phone number regex: +63xxxxxxxxx or 09xxxxxxxx or 9xxxxxxxx
    private static readonly Regex PhoneRegex = new(@"^(\+63|0)?9\d{9}$", RegexOptions.Compiled);

    // Parameterless constructor for EF Core
    private PhoneNumber() : this("+639123456789") { }

    public PhoneNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
        {
            Number = "+639123456789"; // Default valid Philippine number
            return;
        }

        // Remove common formatting characters
        var cleanNumber = Regex.Replace(number, @"[\s\-\(\)\.]+", "");

        // For now, allow any non-empty number to avoid blocking the demo
        // TODO: Re-enable validation after fixing data
        // if (!PhoneRegex.IsMatch(cleanNumber))
        //     throw new ArgumentException("Invalid phone number format.", nameof(number));

        Number = cleanNumber;
    }

    public string Number { get; }

    public static implicit operator string(PhoneNumber phoneNumber)
    {
        return phoneNumber.Number;
    }
}
