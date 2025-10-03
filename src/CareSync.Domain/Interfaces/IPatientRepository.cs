using CareSync.Domain.Entities;

namespace CareSync.Domain.Interfaces;

public interface IPatientRepository
{
    public Task<Patient?> GetByIdAsync(Guid id);
    public Task<Patient?> GetByEmailAsync(string email);
    public Task<IEnumerable<Patient>> GetAllAsync();
    public Task AddAsync(Patient patient);
    public Task UpdateAsync(Patient patient);
    public Task DeleteAsync(Guid id);
    public Task<bool> HasRelatedDataAsync(Guid id);
}
