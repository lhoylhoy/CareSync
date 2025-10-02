using CareSync.Domain.Entities;

namespace CareSync.Domain.Interfaces;

public interface IProvinceRepository
{
    Task<Province?> GetByIdAsync(Guid id);
    Task<Province?> GetByCodeAsync(string code);
    Task<List<Province>> GetByRegionAsync(string region);
    Task<IEnumerable<Province>> GetAllAsync();
    Task AddAsync(Province province);
    Task UpdateAsync(Province province);
    Task DeleteAsync(Guid id);
}

public interface ICityRepository
{
    Task<City?> GetByIdAsync(Guid id);
    Task<City?> GetByCodeAsync(string code);
    Task<List<City>> GetByProvinceIdAsync(Guid provinceId);
    Task<List<City>> GetByProvinceCodeAsync(string provinceCode);
    Task<IEnumerable<City>> GetAllAsync();
    Task AddAsync(City city);
    Task UpdateAsync(City city);
    Task DeleteAsync(Guid id);
}

// Note: IBarangayRepository removed - using API-first approach with PSGC Cloud API
