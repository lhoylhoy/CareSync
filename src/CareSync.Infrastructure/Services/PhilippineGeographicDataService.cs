using System.Text.Json;
using CareSync.Application.Common.Geographics;
using Microsoft.Extensions.Logging;

namespace CareSync.Infrastructure.Services;

public sealed class PhilippineGeographicDataService : IPhilippineGeographicDataService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PhilippineGeographicDataService> _logger;

    public PhilippineGeographicDataService(HttpClient httpClient, ILogger<PhilippineGeographicDataService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IEnumerable<ProvinceDto>> GetProvincesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var request = CreateRequest("https://psgc.cloud/api/v2/provinces");
            var json = await SendAsync(request, cancellationToken);
            var provinceArray = ExtractDataArray(json);

            var provinces = new List<ProvinceDto>();
            if (provinceArray.ValueKind == JsonValueKind.Array)
            {
                foreach (var element in provinceArray.EnumerateArray())
                {
                    provinces.Add(new ProvinceDto(
                        Code: Safe(element, "code"),
                        Name: Safe(element, "name"),
                        Region: Safe(element, "region")));
                }
            }

            return provinces.OrderBy(p => p.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading provinces");
            return Enumerable.Empty<ProvinceDto>();
        }
    }

    public async Task<IEnumerable<CityDto>> GetCitiesByProvinceCodeAsync(string provinceCode, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Loading cities for province {ProvinceCode}", provinceCode);
            var url = $"https://psgc.cloud/api/v2/provinces/{provinceCode}/cities-municipalities";
            using var request = CreateRequest(url);

            var json = await SendAsync(request, cancellationToken);
            var cityArray = ExtractDataArray(json);

            var cities = new List<CityDto>();
            if (cityArray.ValueKind == JsonValueKind.Array)
            {
                foreach (var element in cityArray.EnumerateArray())
                {
                    cities.Add(new CityDto(
                        Code: Safe(element, "code"),
                        Name: Safe(element, "name"),
                        ProvinceCode: provinceCode,
                        ProvinceName: Safe(element, "province"),
                        ZipCode: element.TryGetProperty("zip_code", out var zip) ? zip.GetString() : null));
                }
            }

            return cities.OrderBy(c => c.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading cities for province {ProvinceCode}", provinceCode);
            return Enumerable.Empty<CityDto>();
        }
    }

    public async Task<IEnumerable<BarangayDto>> GetBarangaysByCityCodeAsync(string cityCode, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"https://psgc.cloud/api/v2/cities-municipalities/{cityCode}/barangays";
            using var request = CreateRequest(url);

            var json = await SendAsync(request, cancellationToken);
            var barangayArray = ExtractDataArray(json);

            var barangays = new List<BarangayDto>();
            if (barangayArray.ValueKind == JsonValueKind.Array)
            {
                foreach (var element in barangayArray.EnumerateArray())
                {
                    barangays.Add(new BarangayDto(
                        Code: Safe(element, "code"),
                        Name: Safe(element, "name"),
                        CityCode: cityCode,
                        CityName: Safe(element, "city_municipality"),
                        ProvinceCode: string.Empty,
                        ProvinceName: Safe(element, "province"),
                        ZipCode: element.TryGetProperty("zip_code", out var zip) ? zip.GetString() : null));
                }
            }

            return barangays.OrderBy(b => b.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading barangays for city {CityCode}", cityCode);
            return Enumerable.Empty<BarangayDto>();
        }
    }

    public async Task<ProvinceDto?> GetProvinceByCodeAsync(string provinceCode, CancellationToken cancellationToken = default)
    {
        try
        {
            var provinces = await GetProvincesAsync(cancellationToken);
            return provinces.FirstOrDefault(p => p.Code == provinceCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading province {ProvinceCode}", provinceCode);
            return null;
        }
    }

    public async Task<CityDto?> GetCityByCodeAsync(string cityCode, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"https://psgc.cloud/api/v2/cities-municipalities/{cityCode}";
            using var request = CreateRequest(url);

            var json = await SendAsync(request, cancellationToken);
            var element = ExtractDataObject(json);

            if (element.ValueKind == JsonValueKind.Object)
            {
                return new CityDto(
                    Code: Safe(element, "code"),
                    Name: Safe(element, "name"),
                    ProvinceCode: Safe(element, "province_code"),
                    ProvinceName: Safe(element, "province"),
                    ZipCode: element.TryGetProperty("zip_code", out var zip) ? zip.GetString() : null);
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading city {CityCode}", cityCode);
            return null;
        }
    }

    public async Task<BarangayDto?> GetBarangayByCodeAsync(string barangayCode, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"https://psgc.cloud/api/v2/barangays/{barangayCode}";
            using var request = CreateRequest(url);

            var json = await SendAsync(request, cancellationToken);
            var element = ExtractDataObject(json);

            if (element.ValueKind == JsonValueKind.Object)
            {
                return new BarangayDto(
                    Code: Safe(element, "code"),
                    Name: Safe(element, "name"),
                    CityCode: Safe(element, "city_municipality_code"),
                    CityName: Safe(element, "city_municipality"),
                    ProvinceCode: Safe(element, "province_code"),
                    ProvinceName: Safe(element, "province"),
                    ZipCode: element.TryGetProperty("zip_code", out var zip) ? zip.GetString() : null);
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading barangay {BarangayCode}", barangayCode);
            return null;
        }
    }

    private static HttpRequestMessage CreateRequest(string uri)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
        {
            NoCache = true,
            NoStore = true,
            MustRevalidate = true
        };
        request.Headers.Add("Pragma", "no-cache");
        return request;
    }

    private async Task<JsonElement> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<JsonElement>(payload);
    }

    private static JsonElement ExtractDataArray(JsonElement root)
    {
        return root.ValueKind switch
        {
            JsonValueKind.Object when root.TryGetProperty("data", out var dataArray) && dataArray.ValueKind == JsonValueKind.Array => dataArray,
            JsonValueKind.Array => root,
            _ => default
        };
    }

    private static JsonElement ExtractDataObject(JsonElement root)
    {
        return root.ValueKind switch
        {
            JsonValueKind.Object when root.TryGetProperty("data", out var wrapped) && wrapped.ValueKind == JsonValueKind.Object => wrapped,
            JsonValueKind.Object => root,
            _ => default
        };
    }

    private static string Safe(JsonElement element, string propertyName)
        => element.TryGetProperty(propertyName, out var value) ? value.GetString() ?? string.Empty : string.Empty;
}
