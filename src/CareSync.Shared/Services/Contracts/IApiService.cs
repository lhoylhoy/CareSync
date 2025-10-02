// NOTE: Generic CRUD abstraction retained; specific entity service interfaces moved to Web.Admin layer.
namespace CareSync.Shared.Services.Contracts;

/// <summary>
///     Base interface for API service contracts - provides common CRUD operations
/// </summary>
/// <typeparam name="TDto">The DTO type for the entity</typeparam>
/// <typeparam name="TCreateDto">The create DTO type</typeparam>
/// <typeparam name="TUpdateDto">The update DTO type</typeparam>
/// <typeparam name="TUpsertDto">The upsert DTO type</typeparam>
public interface IApiService<TDto, in TCreateDto, in TUpdateDto, in TUpsertDto>
    where TDto : class
    where TCreateDto : class
    where TUpdateDto : class
    where TUpsertDto : class
{
    Task<List<TDto>> GetAllAsync();
    Task<TDto> GetByIdAsync(Guid id);
    Task<TDto> CreateAsync(TCreateDto createDto);
    Task UpdateAsync(TUpdateDto updateDto);
    Task<TDto> UpsertAsync(TUpsertDto upsertDto);
    Task DeleteAsync(Guid id);
}

// Entity-specific service interfaces removed from Shared. Define them in the Web.Admin layer or another presentation project.
