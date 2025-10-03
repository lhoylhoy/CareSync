namespace CareSync.Application.Common.Geographics;

public interface IPhilippineGeographicDataService
{
    public Task<IEnumerable<ProvinceDto>> GetProvincesAsync(CancellationToken cancellationToken = default);
    public Task<IEnumerable<CityDto>> GetCitiesByProvinceCodeAsync(string provinceCode, CancellationToken cancellationToken = default);
    public Task<IEnumerable<BarangayDto>> GetBarangaysByCityCodeAsync(string cityCode, CancellationToken cancellationToken = default);
    public Task<ProvinceDto?> GetProvinceByCodeAsync(string provinceCode, CancellationToken cancellationToken = default);
    public Task<CityDto?> GetCityByCodeAsync(string cityCode, CancellationToken cancellationToken = default);
    public Task<BarangayDto?> GetBarangayByCodeAsync(string barangayCode, CancellationToken cancellationToken = default);
}
