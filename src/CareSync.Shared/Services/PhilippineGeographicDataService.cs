using System.Text.Json;
using CareSync.Shared.DTOs;
using CareSync.Shared.Interfaces;
using Microsoft.Extensions.Logging;

namespace CareSync.Shared.Services
{
    public class PhilippineGeographicDataService : IPhilippineGeographicDataService
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
                using var request = new HttpRequestMessage(HttpMethod.Get, "https://psgc.cloud/api/v2/provinces");
                // Ensure no caching on the request
                request.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
                {
                    NoCache = true,
                    NoStore = true,
                    MustRevalidate = true
                };
                request.Headers.Add("Pragma", "no-cache");

                var response = await _httpClient.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                var root = JsonSerializer.Deserialize<JsonElement>(json);

                // Some versions of the PSGC API return { "data": [...] }, others may return an array directly.
                var provinceArray = root.ValueKind switch
                {
                    JsonValueKind.Object when root.TryGetProperty("data", out var dataArray) && dataArray.ValueKind == JsonValueKind.Array => dataArray,
                    JsonValueKind.Array => root,
                    _ => default
                };

                var provinces = new List<ProvinceDto>();
                if (provinceArray.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in provinceArray.EnumerateArray())
                    {
                        string Safe(JsonElement parent, string name) => parent.TryGetProperty(name, out var v) ? v.GetString() ?? "" : "";
                        var code = Safe(item, "code");
                        var name = Safe(item, "name");
                        var region = Safe(item, "region");
                        provinces.Add(new ProvinceDto(code, name, region));
                    }
                }
                return provinces.OrderBy(p => p.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading provinces");
                return new List<ProvinceDto>();
            }
        }

        public async Task<IEnumerable<CityDto>> GetCitiesByProvinceCodeAsync(string provinceCode, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("SERVICE: Loading cities for province {ProvinceCode}", provinceCode);
                var url = $"https://psgc.cloud/api/v2/provinces/{provinceCode}/cities-municipalities";
                _logger.LogInformation("SERVICE: Calling external API: {Url}", url);

                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                // Ensure no caching on the request
                request.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
                {
                    NoCache = true,
                    NoStore = true,
                    MustRevalidate = true
                };
                request.Headers.Add("Pragma", "no-cache");

                var response = await _httpClient.SendAsync(request, cancellationToken);
                _logger.LogInformation("SERVICE: External API response status: {StatusCode}", response.StatusCode);

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogInformation("SERVICE: External API response length: {Length} characters", json.Length);

                var root = JsonSerializer.Deserialize<JsonElement>(json);

                var cityArray = root.ValueKind switch
                {
                    JsonValueKind.Object when root.TryGetProperty("data", out var dataArray) && dataArray.ValueKind == JsonValueKind.Array => dataArray,
                    JsonValueKind.Array => root,
                    _ => default
                };

                var cities = new List<CityDto>();
                if (cityArray.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in cityArray.EnumerateArray())
                    {
                        string Safe(JsonElement parent, string name) => parent.TryGetProperty(name, out var v) ? v.GetString() ?? "" : "";
                        var code = Safe(item, "code");
                        var name = Safe(item, "name");
                        var provinceName = Safe(item, "province");
                        var zipCode = item.TryGetProperty("zip_code", out var zip) ? zip.GetString() : null;
                        cities.Add(new CityDto(code, name, provinceCode, provinceName, zipCode));
                    }
                }
                _logger.LogInformation("SERVICE: Processed {Count} cities from external API", cities.Count);
                return cities.OrderBy(c => c.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading cities for province {ProvinceCode}", provinceCode);
                return new List<CityDto>();
            }
        }

        public async Task<IEnumerable<BarangayDto>> GetBarangaysByCityCodeAsync(string cityCode, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = $"https://psgc.cloud/api/v2/cities-municipalities/{cityCode}/barangays";
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                // Ensure no caching on the request
                request.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
                {
                    NoCache = true,
                    NoStore = true,
                    MustRevalidate = true
                };
                request.Headers.Add("Pragma", "no-cache");

                var response = await _httpClient.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                var root = JsonSerializer.Deserialize<JsonElement>(json);
                var barangayArray = root.ValueKind switch
                {
                    JsonValueKind.Object when root.TryGetProperty("data", out var dataArray) && dataArray.ValueKind == JsonValueKind.Array => dataArray,
                    JsonValueKind.Array => root,
                    _ => default
                };

                var barangays = new List<BarangayDto>();
                if (barangayArray.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in barangayArray.EnumerateArray())
                    {
                        string Safe(JsonElement parent, string name) => parent.TryGetProperty(name, out var v) ? v.GetString() ?? "" : "";
                        var code = Safe(item, "code");
                        var name = Safe(item, "name");
                        var cityName = Safe(item, "city_municipality");
                        var provinceName = Safe(item, "province");
                        var zipCode = item.TryGetProperty("zip_code", out var zip) ? zip.GetString() : null;
                        barangays.Add(new BarangayDto(code, name, cityCode, cityName, "", provinceName, zipCode));
                    }
                }
                return barangays.OrderBy(b => b.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading barangays for city {CityCode}", cityCode);
                return new List<BarangayDto>();
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
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                // Ensure no caching on the request
                request.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
                {
                    NoCache = true,
                    NoStore = true,
                    MustRevalidate = true
                };
                request.Headers.Add("Pragma", "no-cache");

                var response = await _httpClient.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                var root = JsonSerializer.Deserialize<JsonElement>(json);
                // Single city may be returned directly or wrapped in { data: { ... } }
                JsonElement item = default;
                if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("data", out var wrapped) && wrapped.ValueKind == JsonValueKind.Object)
                    item = wrapped;
                else if (root.ValueKind == JsonValueKind.Object)
                    item = root; // assume direct object

                if (item.ValueKind == JsonValueKind.Object)
                {
                    string Safe(string name) => item.TryGetProperty(name, out var v) ? v.GetString() ?? "" : "";
                    var code = Safe("code");
                    var name = Safe("name");
                    var provinceCode = Safe("province_code");
                    var provinceName = Safe("province");
                    var zipCode = item.TryGetProperty("zip_code", out var zip) ? zip.GetString() : null;
                    return new CityDto(code, name, provinceCode, provinceName, zipCode);
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
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                // Ensure no caching on the request
                request.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
                {
                    NoCache = true,
                    NoStore = true,
                    MustRevalidate = true
                };
                request.Headers.Add("Pragma", "no-cache");

                var response = await _httpClient.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                var root = JsonSerializer.Deserialize<JsonElement>(json);
                JsonElement item = default;
                if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("data", out var wrapped) && wrapped.ValueKind == JsonValueKind.Object)
                    item = wrapped;
                else if (root.ValueKind == JsonValueKind.Object)
                    item = root;

                if (item.ValueKind == JsonValueKind.Object)
                {
                    string Safe(string name) => item.TryGetProperty(name, out var v) ? v.GetString() ?? "" : "";
                    var code = Safe("code");
                    var name = Safe("name");
                    var cityCode = Safe("city_municipality_code");
                    var cityName = Safe("city_municipality");
                    var provinceName = Safe("province");
                    var zipCode = item.TryGetProperty("zip_code", out var zip) ? zip.GetString() : null;
                    return new BarangayDto(code, name, cityCode, cityName, "", provinceName, zipCode);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading barangay {BarangayCode}", barangayCode);
                return null;
            }
        }
    }
}
