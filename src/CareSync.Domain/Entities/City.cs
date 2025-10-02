namespace CareSync.Domain.Entities;

public class City
{
    // Note: Barangays collection removed - using API-first approach with PSGC Cloud API

    private City() { }

    public City(string name, string code, string? zipCode, Province province)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("City name cannot be null or empty.", nameof(name));

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("City code cannot be null or empty.", nameof(code));

        if (province == null)
            throw new ArgumentNullException(nameof(province));

        Name = name.Trim();
        Code = code.Trim().ToUpper();
        ZipCode = zipCode?.Trim();
        ProvinceId = province.Id;
        Province = province;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty; // City/Municipality code
    public string? ZipCode { get; private set; } // Philippine zip code
    public Guid ProvinceId { get; private set; }
    public Province Province { get; private set; } = null!;

    // Note: AddBarangay method removed - using API-first approach with PSGC Cloud API

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("City name cannot be null or empty.", nameof(name));

        Name = name.Trim();
    }

    public void UpdateZipCode(string? zipCode)
    {
        ZipCode = zipCode?.Trim();
    }
}
