using CareSync.Shared.DTOs;

namespace CareSync.Shared.Interfaces;

public interface IPhilippineGeographicDataService
{
    /// <summary>
    ///     Gets all provinces
    /// </summary>
    Task<IEnumerable<ProvinceDto>> GetProvincesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets cities for a specific province by province code
    /// </summary>
    Task<IEnumerable<CityDto>> GetCitiesByProvinceCodeAsync(string provinceCode, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets barangays for a specific city by city code
    /// </summary>
    Task<IEnumerable<BarangayDto>> GetBarangaysByCityCodeAsync(string cityCode, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a province by code
    /// </summary>
    Task<ProvinceDto?> GetProvinceByCodeAsync(string provinceCode, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a city by code
    /// </summary>
    Task<CityDto?> GetCityByCodeAsync(string cityCode, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a barangay by code
    /// </summary>
    Task<BarangayDto?> GetBarangayByCodeAsync(string barangayCode, CancellationToken cancellationToken = default);
}
