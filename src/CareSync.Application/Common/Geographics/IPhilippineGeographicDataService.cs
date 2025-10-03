namespace CareSync.Application.Common.Geographics;

public interface IPhilippineGeographicDataService
{
    Task<IEnumerable<ProvinceDto>> GetProvincesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<CityDto>> GetCitiesByProvinceCodeAsync(string provinceCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<BarangayDto>> GetBarangaysByCityCodeAsync(string cityCode, CancellationToken cancellationToken = default);
    Task<ProvinceDto?> GetProvinceByCodeAsync(string provinceCode, CancellationToken cancellationToken = default);
    Task<CityDto?> GetCityByCodeAsync(string cityCode, CancellationToken cancellationToken = default);
    Task<BarangayDto?> GetBarangayByCodeAsync(string barangayCode, CancellationToken cancellationToken = default);
}
