namespace CareSync.Domain.Entities;

public class Barangay
{
    private Barangay() { }

    public Barangay(string name, string code, City city)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Barangay name cannot be null or empty.", nameof(name));

        if (city == null)
            throw new ArgumentNullException(nameof(city));

        Name = name.Trim();
        Code = string.IsNullOrWhiteSpace(code) ? string.Empty : code.Trim().ToUpper();
        CityId = city.Id;
        City = city;
    }

    public Barangay(Guid id, string name, string code, City city)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Barangay name cannot be null or empty.", nameof(name));

        if (city == null)
            throw new ArgumentNullException(nameof(city));

        Id = id;

        Name = name.Trim();
        Code = string.IsNullOrWhiteSpace(code) ? string.Empty : code.Trim().ToUpper();
        CityId = city.Id;
        City = city;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty; // Barangay code if available
    public Guid CityId { get; private set; }
    public City City { get; private set; } = null!;
}
