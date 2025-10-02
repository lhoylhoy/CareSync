using CareSync.Domain.Entities;
using CareSync.Domain.Interfaces;
using CareSync.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CareSync.Infrastructure.Repositories;

public class ProvinceRepository(CareSyncDbContext context) : IProvinceRepository
{
    public async Task<Province?> GetByIdAsync(Guid id)
    {
        return await context.Provinces.FindAsync(id);
    }

    public async Task<IEnumerable<Province>> GetAllAsync()
    {
        return await context.Provinces
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<Province?> GetByCodeAsync(string code)
    {
        return await context.Provinces
            .FirstOrDefaultAsync(p => p.Code == code.ToUpper());
    }

    public async Task<List<Province>> GetByRegionAsync(string region)
    {
        return await context.Provinces
            .Where(p => p.Region == region)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task AddAsync(Province entity)
    {
        await context.Provinces.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Province entity)
    {
        context.Provinces.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var province = await GetByIdAsync(id);
        if (province != null)
        {
            context.Provinces.Remove(province);
            await context.SaveChangesAsync();
        }
    }
}
