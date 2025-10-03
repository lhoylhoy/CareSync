using System.Linq.Expressions;

namespace CareSync.Domain.Interfaces;

public interface IGenericRepository<TEntity> where TEntity : class
{
    public Task<TEntity?> GetByIdAsync(Guid id);
    public Task<IEnumerable<TEntity>> GetAllAsync();
    public Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    public Task AddAsync(TEntity entity);
    public Task AddRangeAsync(IEnumerable<TEntity> entities);
    public Task UpdateAsync(TEntity entity);
    public Task DeleteAsync(Guid id);
    public Task<int> CountAsync();
}
