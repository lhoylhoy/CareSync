namespace CareSync.Domain.Entities;

public class Province
{
    private readonly List<City> _cities = new();

    private Province() { }

    public Province(string name, string code, string region)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Province name cannot be null or empty.", nameof(name));

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Province code cannot be null or empty.", nameof(code));

        if (string.IsNullOrWhiteSpace(region))
            throw new ArgumentException("Region cannot be null or empty.", nameof(region));

        Name = name.Trim();
        Code = code.Trim().ToUpper();
        Region = region.Trim();
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty; // Province code like "NCR", "CAV", etc.

    public string Region { get; private set; } =
        string.Empty; // Region like "National Capital Region", "Region I", etc.

    public IReadOnlyCollection<City> Cities => _cities.AsReadOnly();

    public void AddCity(City city)
    {
        if (city == null)
            throw new ArgumentNullException(nameof(city));

        if (!_cities.Contains(city)) _cities.Add(city);
    }
}
