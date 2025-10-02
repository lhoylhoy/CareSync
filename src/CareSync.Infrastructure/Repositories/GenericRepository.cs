using System.Linq.Expressions;
using CareSync.Domain.Interfaces;
using CareSync.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CareSync.Infrastructure.Repositories;

public abstract class GenericRepository<TEntity>(CareSyncDbContext context) : IGenericRepository<TEntity>
    where TEntity : class
{
    protected readonly CareSyncDbContext Context = context;
    protected DbSet<TEntity> Set => Context.Set<TEntity>();

    public virtual async Task<TEntity?> GetByIdAsync(Guid id)
        => await Set.FindAsync(id);

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        => await Set.AsNoTracking().ToListAsync();

    public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        => await Set.AsNoTracking().Where(predicate).ToListAsync();

    public virtual async Task AddAsync(TEntity entity)
        => await Set.AddAsync(entity);

    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
        => await Set.AddRangeAsync(entities);

    public virtual Task UpdateAsync(TEntity entity)
    {
        Set.Update(entity);
        return Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity is not null)
            Set.Remove(entity);
    }

    public virtual async Task<int> CountAsync() => await Set.CountAsync();
}
