using System.Net.Http.Json;
using CareSync.Application.Common.Geographics;

namespace CareSync.Web.Admin.Services
{
    public class ApiGeographicDataService : IPhilippineGeographicDataService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiGeographicDataService> _logger;

        public ApiGeographicDataService(HttpClient httpClient, ILogger<ApiGeographicDataService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<ProvinceDto>> GetProvincesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("WEB ADMIN: Calling API for provinces");
                var provinces = await _httpClient.GetFromJsonAsync<IEnumerable<ProvinceDto>>("api/GeographicData/provinces", cancellationToken);
                _logger.LogInformation("WEB ADMIN: Received {Count} provinces from API", provinces?.Count() ?? 0);
                return provinces ?? new List<ProvinceDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WEB ADMIN: Error loading provinces from API");
                return new List<ProvinceDto>();
            }
        }

        public async Task<IEnumerable<CityDto>> GetCitiesByProvinceCodeAsync(string provinceCode, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("WEB ADMIN: Calling API for cities in province {ProvinceCode}", provinceCode);
                var cities = await _httpClient.GetFromJsonAsync<IEnumerable<CityDto>>($"api/GeographicData/provinces/{provinceCode}/cities", cancellationToken);
                _logger.LogInformation("WEB ADMIN: Received {Count} cities from API for province {ProvinceCode}", cities?.Count() ?? 0, provinceCode);
                return cities ?? new List<CityDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WEB ADMIN: Error loading cities for province {ProvinceCode} from API", provinceCode);
                return new List<CityDto>();
            }
        }

        public async Task<IEnumerable<BarangayDto>> GetBarangaysByCityCodeAsync(string cityCode, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("WEB ADMIN: Calling API for barangays in city {CityCode}", cityCode);
                var barangays = await _httpClient.GetFromJsonAsync<IEnumerable<BarangayDto>>($"api/GeographicData/cities/{cityCode}/barangays", cancellationToken);
                _logger.LogInformation("WEB ADMIN: Received {Count} barangays from API for city {CityCode}", barangays?.Count() ?? 0, cityCode);
                return barangays ?? new List<BarangayDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WEB ADMIN: Error loading barangays for city {CityCode} from API", cityCode);
                return new List<BarangayDto>();
            }
        }

        public async Task<ProvinceDto?> GetProvinceByCodeAsync(string provinceCode, CancellationToken cancellationToken = default)
        {
            // Not needed for the current form, but implementing for interface completeness
            var provinces = await GetProvincesAsync(cancellationToken);
            return provinces.FirstOrDefault(p => p.Code == provinceCode);
        }

        public async Task<CityDto?> GetCityByCodeAsync(string cityCode, CancellationToken cancellationToken = default)
        {
            // Not needed for the current form, but implementing for interface completeness
            return await Task.FromResult((CityDto?)null);
        }

        public async Task<BarangayDto?> GetBarangayByCodeAsync(string barangayCode, CancellationToken cancellationToken = default)
        {
            // Not needed for the current form, but implementing for interface completeness
            return await Task.FromResult((BarangayDto?)null);
        }
    }
}
