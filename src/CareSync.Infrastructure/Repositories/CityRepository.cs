using CareSync.Domain.Entities;
using CareSync.Domain.Interfaces;
using CareSync.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CareSync.Infrastructure.Repositories;

public class CityRepository : ICityRepository
{
    private readonly CareSyncDbContext _context;

    public CityRepository(CareSyncDbContext context)
    {
        _context = context;
    }

    public async Task<City?> GetByIdAsync(Guid id)
    {
        return await _context.Cities
            .Include(c => c.Province)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<City>> GetAllAsync()
    {
        return await _context.Cities
            .Include(c => c.Province)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<City?> GetByCodeAsync(string code)
    {
        return await _context.Cities
            .Include(c => c.Province)
            .FirstOrDefaultAsync(c => c.Code == code.ToUpper());
    }

    public async Task<List<City>> GetByProvinceIdAsync(Guid provinceId)
    {
        return await _context.Cities
            .Include(c => c.Province)
            .Where(c => c.ProvinceId == provinceId)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<List<City>> GetByProvinceCodeAsync(string provinceCode)
    {
        return await _context.Cities
            .Include(c => c.Province)
            .Where(c => c.Province.Code == provinceCode.ToUpper())
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task AddAsync(City entity)
    {
        await _context.Cities.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(City entity)
    {
        _context.Cities.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var city = await GetByIdAsync(id);
        if (city != null)
        {
            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();
        }
    }
}
