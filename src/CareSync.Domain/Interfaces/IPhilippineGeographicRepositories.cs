using CareSync.Domain.Entities;

namespace CareSync.Domain.Interfaces;

public interface IProvinceRepository
{
    public Task<Province?> GetByIdAsync(Guid id);
    public Task<Province?> GetByCodeAsync(string code);
    public Task<List<Province>> GetByRegionAsync(string region);
    public Task<IEnumerable<Province>> GetAllAsync();
    public Task AddAsync(Province province);
    public Task UpdateAsync(Province province);
    public Task DeleteAsync(Guid id);
}

public interface ICityRepository
{
    public Task<City?> GetByIdAsync(Guid id);
    public Task<City?> GetByCodeAsync(string code);
    public Task<List<City>> GetByProvinceIdAsync(Guid provinceId);
    public Task<List<City>> GetByProvinceCodeAsync(string provinceCode);
    public Task<IEnumerable<City>> GetAllAsync();
    public Task AddAsync(City city);
    public Task UpdateAsync(City city);
    public Task DeleteAsync(Guid id);
}

// Note: IBarangayRepository removed - using API-first approach with PSGC Cloud API
