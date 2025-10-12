using CareSync.Domain.Entities;

namespace CareSync.Domain.Interfaces;

public interface IDoctorRepository
{
    public Task<Doctor?> GetByIdAsync(Guid id);
    public Task<Doctor?> GetByEmailAsync(string email);
    public Task<IEnumerable<Doctor>> GetAllAsync();
    public Task<(IReadOnlyList<Doctor> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? searchTerm,
        IReadOnlyDictionary<string, string?> filters,
        CancellationToken cancellationToken = default);
    public Task AddAsync(Doctor doctor);
    public Task UpdateAsync(Doctor doctor);
    public Task DeleteAsync(Guid id);
    public Task<bool> HasRelatedDataAsync(Guid id);
}
