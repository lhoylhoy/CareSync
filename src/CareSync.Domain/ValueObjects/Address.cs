namespace CareSync.Domain.ValueObjects;

public record Address
{
    // Parameterless constructor for EF Core - bypasses validation for loading from database
    private Address()
    {
        Street = string.Empty;
        Barangay = string.Empty;
        City = string.Empty;
        Province = string.Empty;
        ZipCode = string.Empty;
        Country = string.Empty;
    }

    public Address(string street, string barangay, string city, string province, string zipCode,
        string country = "Philippines")
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be null or empty.", nameof(street));

        if (string.IsNullOrWhiteSpace(barangay))
            throw new ArgumentException("Barangay cannot be null or empty.", nameof(barangay));

        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be null or empty.", nameof(city));

        if (string.IsNullOrWhiteSpace(province))
            throw new ArgumentException("Province cannot be null or empty.", nameof(province));

        if (string.IsNullOrWhiteSpace(zipCode))
            throw new ArgumentException("Zip code cannot be null or empty.", nameof(zipCode));

        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country cannot be null or empty.", nameof(country));

        Street = street.Trim();
        Barangay = barangay.Trim();
        City = city.Trim();
        Province = province.Trim();
        ZipCode = zipCode.Trim();
        Country = country.Trim();
    }

    public string Street { get; }
    public string Barangay { get; }
    public string City { get; }
    public string Province { get; }
    public string ZipCode { get; }
    public string Country { get; }

    public string FormattedAddress => $"{Street}, {Barangay}, {City}, {Province} {ZipCode}, {Country}";

    public override string ToString()
    {
        return FormattedAddress;
    }
}
